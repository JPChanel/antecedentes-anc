using ANC.Comun.Log4net;
using ANC.Comun;
using ANC.Entidades;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using ANC.Modelo;

using System;

namespace ANC.AntecedentesWeb.Controllers
{
    public class AntecedentesController : Controller
    {

        GenericoModel _genericoModel = new GenericoModel();
        AntecedentesModel _antecedentesModel = new AntecedentesModel();

        public async Task<IActionResult> ReporteAntecedentes(string send)
        {
            RespuestaReporteBE respuesta = new RespuestaReporteBE();
           
            respuesta.ArchivosPDF = new List<byte[]>();
            try
            {
                //DESENCRIPTAMOS LOS VALORES
                string clave = "envio_correo_ant";
                byte[] correoBytes = Convert.FromBase64String(send);
                string datosDescifrados = string.Empty;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(clave);
                    aes.IV = new byte[16];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(correoBytes, 0, correoBytes.Length);
                            cs.FlushFinalBlock();
                        }
                        datosDescifrados = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }

                string[] valores = datosDescifrados.Split('~');
                string cod_notif_antec = valores[0];
                string ruta = valores[1];
                string[] ruta_archivos = ruta.Split("&&");
                string nombre_envio = valores[2];
                string correo_envio = valores[3];
                
                int cod_distri = Int32.Parse(valores[4]);
                string tipo_documento = valores[5];
                respuesta.tipo_documentos = tipo_documento.Split("&&");
                //conectarse al ftp
                RptBE retorno = new RptBE();
                AcuseCorreoBE datos = new AcuseCorreoBE();
                byte[] pdfBytes = null;

                string[] binarioDocumento = new string[ruta_archivos.Length];

                try
                {
                    CredencialesFTPBE credeniales = await _genericoModel.GetCredencialesFTP(cod_distri);
                    FtpClient FTP = null;
                  
                    for (int i = 0; i < ruta_archivos.Length; i++)
                    {
                        string pdf_ruta = ruta_archivos[i];
                        if (pdf_ruta.StartsWith("ZAV-"))
                        {
                            string[] palabras = pdf_ruta.Split('-');

                            FTP = new FtpClient("ftp://" + credeniales.server_ftp3, credeniales.user_ftp3, credeniales.pass_ftp3);

                            pdf_ruta = palabras[1];

                        }else if (pdf_ruta.StartsWith("ANC-"))
                        {
                            string[] palabras = pdf_ruta.Split('-');

                            FTP = new FtpClient("ftp://" + credeniales.server_ftp, credeniales.user_ftp, credeniales.pass_ftp);

                            pdf_ruta = palabras[1];
                        }
                        else
                        {
                            FTP = new FtpClient("ftp://" + credeniales.server_ftp3, credeniales.user_ftp3, credeniales.pass_ftp3);

                        }


                        pdfBytes = FTP.downloadBytes(pdf_ruta);
                        if (pdfBytes[0] == 0 || pdfBytes == null || pdfBytes.Length == 0)
                        {

                            respuesta.MensajeError = "No se pudo obtener el PDF desde el servidor FTP.";
                            throw new Exception();
                        }
                        string pdfEnBinario = Convert.ToBase64String(pdfBytes);

                        binarioDocumento[i] = pdfEnBinario;
                    }

                }
                catch (Exception er)
                {

                    respuesta.MensajeError = "Surgió un problema." + respuesta.MensajeError;
                }



                     WSEmailBE wsEmail = new WSEmailBE();
                    wsEmail.destinatarioEmail = new string[1];
                    wsEmail.destinatarioTipo = new string[1];

                    wsEmail.destinatarioEmail[0] = correo_envio;
                    wsEmail.destinatarioTipo[0] = "TO";

                    wsEmail.nombreCompleto = nombre_envio;
                   
                    wsEmail.titulo = "CONFIRMACIÓN DE RECEPCIÓN CERTIFICADO DE ANTECEDENTES ELECTRÓNICO";
                    wsEmail.motivoEmail = "Envio acuse Certificado Antecedentes Electrónico";

                    wsEmail.TextoHtml = "<p style='text-align:justify;font-size: 13px;'><b>Este es un mensaje de confirmacion sobre la recepción del mensaje enviado anteriormente. <br><br>  Es para su registro y confirmación. <br><br> Saludos cordiales,</br></b></p>";
                    retorno = await _antecedentesModel.EnviarConfirmacionAcuse(wsEmail);
                    if (retorno.codigo == -1)
                    {
                        respuesta.MensajeError = "Surgio un problema:" + retorno.valor;
                        throw new Exception();
                    }
                    //si existe pdf, actualizamos la base de datos
                    datos.cod_notif_antec = Int32.Parse(cod_notif_antec);

                    datos.accion = "U";

                    RptPGBE rpta = await _antecedentesModel.RegistrarAcuseReciboAntecedentes(datos);
                    if (rpta.rs_valor == "OK")
                    {

                        // Retornar el PDF como un archivo para que se abra en una nueva ventana del navegador
                        for (int j = 0; j < binarioDocumento.Length; j++)
                        {
                            byte[] pdf_final = Convert.FromBase64String(binarioDocumento[j]);
                            respuesta.ArchivosPDF.Add(pdf_final);
                         

                        }
                        return View(respuesta); 
                    }
                    else
                    {
                        respuesta.MensajeError =  rpta.rs_valor;
                        throw new Exception();


                    }

            }
            catch (Exception ex)
            {
                LogAntecedentes.Logger().Error("Error: " + ex);
                respuesta.MensajeError = "Surgió un problema." + respuesta.MensajeError;

            }
            return View(respuesta); 
        }
      
    }


}
