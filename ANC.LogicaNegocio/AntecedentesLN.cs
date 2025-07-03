using ANC.AccesoDatos;
using ANC.Comun;
using ANC.Comun.Log4net;
using ANC.Entidades;
using ANC.LogicaNegocio.FirmaElectronica;
using iText.Commons.Utils;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Pdfa;
using iText.StyledXmlParser.Jsoup.Nodes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ZXing;
using ZXing.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Document = iText.Layout.Document;
using Image = iText.Layout.Element.Image;

namespace ANC.LogicaNegocio
{
    public class AntecedentesLN
    {
        AntecedentesAD comunAD = new AntecedentesAD();
        GenericoAD genericoAD = new GenericoAD();
        FirmaDigital _firmaDigital=new FirmaDigital();
        public ExpedienteAntecedenteBE AntecedentesDatosPersonales(string codigo_persona)
        {
            return comunAD.AntecedentesDatosPersonales(codigo_persona);
        }
        public List<ExpedienteSancionesBE> AntecedentesSanciones(string codigo_persona, int sub_tipo_antecedente)
        {
            return comunAD.AntecedentesSanciones(codigo_persona, sub_tipo_antecedente);
        }
        public List<ExpedienteSancionesBE> AntecedentesExpedientes(string codigo_persona, int sub_tipo_antecedente)
        {
            return comunAD.AntecedentesExpedientes(codigo_persona, sub_tipo_antecedente);
        }

        public RecordSancionesBE RecordSancionesTotal(string codigo_persona, int sub_tipo_antecedente)
        {
            return comunAD.RecordSancionesTotal(codigo_persona, sub_tipo_antecedente);
        }
        public RecordExpedientesBE RecordExpedientesTotal(string codigo_persona, int sub_tipo_antecedente)
        {
            return comunAD.RecordExpedientesTotal(codigo_persona, sub_tipo_antecedente);
        }
        public List<PiePagAntecedenteSancionesBE> AntecedentesSancionesPiePagina(string codigo_persona, int sub_tipo_antecedente)
        {
            return comunAD.AntecedentesSancionesPiePagina(codigo_persona, sub_tipo_antecedente);
        }
        public List<PiePagAntecedenteSancionesBE> AntecedentesPiePaginaExpediente(string codigo_persona, int sub_tipo_antecedente)
        {
            return comunAD.AntecedentesPiePaginaExpediente(codigo_persona, sub_tipo_antecedente);
        }
        public RptVerificarDocBE VericarDoc(string cod_usuario)
        {
            return comunAD.VericarDoc(cod_usuario);
        }
        public RptVerificacionBE ActualizarVericaDoc(ActualizarVerificaDocBE datos)
        {
            return comunAD.ActualizarVericaDoc(datos);
        }
        public RptVerificacionBE LoginVericar(AntecedentesBE datos)
        {
            return comunAD.LoginVericar(datos);
        }
        public Task<RptVerificacionBE> AuditoriaAntecedentes(AuditoriaAntecedentesBE datos)
        {
            return comunAD.AuditoriaAntecedentes(datos);
        }
        public async Task<RptVerificacionBE> VericarAntecedentes(VerificaAntecedentesBE datos)
        {
            return await comunAD.VericarAntecedentes(datos);
        }
        public async Task<RptPGBE> RegistrarAcuseAntecedentes(AcuseCorreoBE datos)
        {
            return await comunAD.RegistrarAcuseAntecedentes(datos);
        }
        public async Task<List<RptConsutaAntecedenteBE>> ConsultaAntecedentes(ConsultaAntecedenteBE datos)
        {
            return await comunAD.ConsultaAntecedentes(datos);
        }
       
        public async Task<RptTramiteDocumentarioBE> RegistroTramiteDocumentario(TramiteDocumentarioBE datos)
        {


            RptTramiteDocumentarioBE rpta = await comunAD.RegistroTramiteDocumentario(datos);
            if (rpta.rn_codigo != 0)
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "Surgió un error al generar el registro en el trámite Documentario";
                return rpta;

            }

            string[] remitente = new string[]
{
                rpta.rs_remitente


             };
            DateTime dateTime = DateTime.Now;
            byte[] bytesCedula = CedulaDocAdministrativo(rpta.rs_registro_mp + "-" + rpta.rs_anio_mp, rpta.rs_destinatario, rpta.rs_fec_doc, rpta.rs_documento, rpta.rs_fojas_inicial, rpta.rs_distrito_judicial, rpta.rs_fec_recepcion, rpta.rs_contenido, remitente, dateTime.ToString("dd/MM/yyyy"), dateTime.ToString("HH:mm:ss"), datos.usuario, datos.ruta_logo);

            bool validPDF = EsArchivoPDF(bytesCedula);
            if (!validPDF)
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "Error al generar Cedula del Documento Administrativo";
                return rpta;
            }

            long tamañoEnBytes = bytesCedula.Length;
            double tamañoEnKB = (double)tamañoEnBytes / 1024;
            double tamañoEnMB = tamañoEnKB / 1024;
            string tamanio_cedula = $"{tamañoEnMB:N2}";


            datos.rs_registro_mp = rpta.rs_registro_mp + "-" + rpta.rs_anio_mp;


            RespuestaArchivoBE   respuestaForm = FormularioDocAdministrativo(datos);

            byte[] bytesFormulario = respuestaForm.ArchivosPDF;
            if (bytesFormulario == null)
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "Surgio una excepcion al generar Documento del Formulario del Documento Administrativo"+ respuestaForm.valor;
                return rpta;
            }

            bool validPDFFormulario = EsArchivoPDF(bytesFormulario);
            if (!validPDFFormulario)
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "Error al generar Documento del Formulario del Documento Administrativo";
                return rpta;
            }

            long tamañoEnBytesForm = bytesFormulario.Length;
            double tamañoEnKBForm = (double)tamañoEnBytesForm / 1024;
            double tamañoEnMBForm = tamañoEnKBForm / 1024;
            string tamanio_formulario = $"{tamañoEnMBForm:N2}";



            //Subimos al FTP
            //optener credenciales de ANC para subir a Zavala 

            CredencialesFTPBE credeniales = await genericoAD.GetCredencialesFTP(datos.cod_distrito_ftp);
            FtpClient FTP = new FtpClient("ftp://" + credeniales.server_ftp, credeniales.user_ftp, credeniales.pass_ftp);

            if (credeniales == null)
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "No se pudo obtener las Credenciales del FTP";
                return rpta;

            }
            string nombreDirectorioCedula = System.IO.Path.GetDirectoryName(rpta.rs_ruta_cedula);


            FTP.createDirectory(nombreDirectorioCedula);

            //subimos la cedula
            RespuestaBE subirFTPCedula = FTP.uploadByte(rpta.rs_ruta_cedula, bytesCedula);
            if (subirFTPCedula.codigo != "0")
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "ERROR al subir el documento de la cedula al FTP: " + subirFTPCedula.valor;
                return rpta;


            }

            string nombreDirectorioFormulario = System.IO.Path.GetDirectoryName(rpta.rs_ruta_formulario);

            FTP.createDirectory(nombreDirectorioFormulario);
            //subimos formulario
            RespuestaBE subirFTPFormulario = FTP.uploadByte(rpta.rs_ruta_formulario, bytesFormulario);
            if (subirFTPFormulario.codigo != "0")
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "ERROR al subir el documento del Formulario al FTP:" + subirFTPFormulario.valor;
                return rpta;

            }
           
            datos.cod_intmp = rpta.rn_cod_intmp;
            datos.cod_mpdf_cedula = rpta.rn_cod_mpdf_cedula;
            datos.cod_mpdf_formulario = rpta.rn_cod_mpdf_formulario;
            datos.tamanio_cedula = tamanio_cedula;
            datos.tamanio_formulario = tamanio_formulario;
            datos.accion = "A";
            RptTramiteDocumentarioBE rpta2 = await comunAD.RegistroTramiteDocumentario(datos);

            if (rpta2.rn_codigo != 0)
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "Surgió un error al actualizar el registro en el trámite Documentario";
                return rpta;

            }
            MantRegistroPGBE mantRegistroPGBE = new MantRegistroPGBE();
            mantRegistroPGBE.cod_intmp = rpta.rn_cod_intmp;
            mantRegistroPGBE.cod_intmp_rpta= rpta.rn_cod_intmp;
            mantRegistroPGBE.cod_intper = 0;
            mantRegistroPGBE.nro_documento = datos.num_documento;
            mantRegistroPGBE.nro_registro = rpta.rs_registro_mp;
            mantRegistroPGBE.anio = rpta.rs_anio_mp;
            
            mantRegistroPGBE.cod_antec = new int[0];
            mantRegistroPGBE.cod_tipantec = new int[]
            {
                datos.tipo_antecedente
            };

            mantRegistroPGBE.correo = new string[]{datos.correo};  
            mantRegistroPGBE.celular = new string[] { datos.nro_celular };
            mantRegistroPGBE.casilla= new string[] {};
            mantRegistroPGBE.cod_origantec=datos.cod_orig_antec;
            mantRegistroPGBE.cod_usuario = datos.num_documento;
            mantRegistroPGBE.accion = "N";

            RptPGBE rpt_mtd =await comunAD.MantenimientoRegistroAntecedentePG(mantRegistroPGBE);

            if (rpt_mtd.rn_codigo!=0)
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "Surgió un error al registrar la informacion el registro en el trámite Documentario PG";
                return rpta;
            }
            string[] ruta_archivo = new string[]
                      {
                          rpta.rs_ruta_cedula,
                          rpta.rs_ruta_formulario
                      };
            rpta.ruta_documentos = ruta_archivo;
            rpta.tipo_documentos = new string[]
                      {
                          "CC",
                          "CF"
                      };

            return rpta;
        }

        public async Task<RptPGBE> RegistrarTDAntecedente(MantRegistroPGBE datos)
        {
            return await comunAD.MantenimientoRegistroAntecedentePG(datos);
        }

        public async Task<RptVerificacionBE> VerificarTDAntecedente(string num_documento,int cod_tipo_repo)
        {
            return await comunAD.VerificarTDAntecedente(num_documento, cod_tipo_repo);
        }

        public async Task<RptPGBE> MantMovimiento(AuditoriaAntecedentesBE datos)
        {
            return await comunAD.MantMovimiento(datos);
        }
        public async Task<RptPGBE> VerificarIngreso(VerificaIngresoBE datos)
        {
            return await comunAD.VerificarIngreso(datos);
        }
        public async Task<List<ListAnioBE>> ListarAnio()
        {
            return await comunAD.ListarAnio();
        }
        public async Task<RespuestaArchivoBE> GenerarCertificadoPDF(SesionAntecedentesBE datos)
        {
            RespuestaArchivoBE rspApi = new RespuestaArchivoBE();


            try
            {

                VerificaAntecedentesBE datos_verifica = new VerificaAntecedentesBE();




                datos_verifica.as_cod_usuario = datos.cod_usuario;
                datos_verifica.an_cod_intper = datos.cod_intper;

                datos_verifica.as_ap_paterno = datos.ap_paterno;
                datos_verifica.as_ap_materno = datos.ap_materno;
                datos_verifica.as_nombres = datos.nombres;
                datos_verifica.as_num_documento = datos.num_documento;

                datos_verifica.an_cod_tipantec = datos.cod_tipo_repo;
                datos_verifica.an_cod_subtipantec = datos.cod_subtipo_repo;
                datos_verifica.an_cod_origantec = datos.cod_origantec;
                datos_verifica.as_nombre_doc = datos.nombre_doc;

                datos_verifica.as_celular = datos.as_celular;                             
                datos_verifica.as_correo = datos.as_correos;

                datos_verifica.an_n_vd_id = 0;


                datos_verifica.as_id_sesion = datos.id_sesion;
                datos_verifica.as_direccion_ip = datos.direccion_ip;
                datos_verifica.as_mac_address = datos.mac_address;
                datos_verifica.as_computer_name = datos.computer_name;
                datos_verifica.as_sistema_operativo = datos.sistema_operativo;
                datos_verifica.as_accion = "V";

                List<string> origen = new List<string>();
                List<string> expediente = new List<string>();
                List<string> instancia = new List<string>();
                List<string> sancionList = new List<string>();
                List<string> estado = new List<string>();
                List<string> observaciones = new List<string>();

                List<int> orden = new List<int>();
                List<int> principal = new List<int>();
                List<int> cod_distrito_origen = new List<int>();
                List<int> cod_intexp = new List<int>();
                List<int> num_secuen = new List<int>();
                List<int> cod_resinf = new List<int>();
                List<int> cod_estado = new List<int>();
                List<int> cod_encarg = new List<int>();

                List<string> num_expedi = new List<string>();
                List<string> num_anios = new List<string>();
                List<string> des_tipexp = new List<string>();
                List<string> des_motivo = new List<string>();
                List<string> detalle_motivo = new List<string>();
                List<string> expe_rel = new List<string>();
                List<string> tipo_rel = new List<string>();
                List<string> fla_tipo = new List<string>();

                List<DateTime> fecha = new List<DateTime>();
                List<DateTime> fec_apertura = new List<DateTime>();

                List<string> encargado = new List<string>();
                List<string> resolucion = new List<string>();


                List<int> cod_estado_expe = new List<int>();
                List<int> cod_estado_reso = new List<int>();



                List<ExpedienteSancionesBE> sanciones = new List<ExpedienteSancionesBE>();
                if (datos.cod_tipo_repo == 1)
                {

                    sanciones = comunAD.AntecedentesSanciones(datos.cod_intper.ToString(), datos.cod_subtipo_repo);

                }
                else if (datos.cod_tipo_repo == 2)
                {

                    sanciones = comunAD.AntecedentesExpedientes(datos.cod_intper.ToString(), datos.cod_subtipo_repo);

                }




                foreach (ExpedienteSancionesBE sancion in sanciones)
                {
                    AsignarValorSiNoNuloOVacio(sancion.origen, valor => origen.Add(valor), "");
                    AsignarValorSiNoNuloOVacio(sancion.expediente, valor => expediente.Add(valor), "");
                    AsignarValorSiNoNuloOVacio(sancion.instancia, valor => instancia.Add(valor), "");
                    AsignarValorSiNoNuloOVacio(sancion.sancion, valor => sancionList.Add(valor), "");
                    AsignarValorSiNoNuloOVacio(sancion.estado, valor => estado.Add(valor), "");
                    AsignarValorSiNoNuloOVacio(sancion.observaciones, valor => observaciones.Add(valor), "");

                    AsignarValorSiNoNuloOVacio(sancion.orden, valor => orden.Add(Int32.Parse(valor)), "1");
                    AsignarValorSiNoNuloOVacio(sancion.principal, valor => principal.Add(Int32.Parse(valor)), "1");
                    AsignarValorSiNoNuloOVacio(sancion.cod_distrito_origen, valor => cod_distrito_origen.Add(Int32.Parse(valor)), "1");
                    AsignarValorSiNoNuloOVacio(sancion.cod_intexp, valor => cod_intexp.Add(Int32.Parse(valor)), "1");
                    AsignarValorSiNoNuloOVacio(sancion.num_secuen, valor => num_secuen.Add(Int32.Parse(valor)), "1");
                    AsignarValorSiNoNuloOVacio(sancion.cod_resinf, valor => cod_resinf.Add(Int32.Parse(valor)), "1");
                    AsignarValorSiNoNuloOVacio(sancion.cod_estado, valor => cod_estado.Add(Int32.Parse(valor)), "1");
                    AsignarValorSiNoNuloOVacio(sancion.cod_encarg, valor => cod_encarg.Add(Int32.Parse(valor)), "1");

                    AsignarValorSiNoNuloOVacio(sancion.num_expedi, valor => num_expedi.Add(valor), "");
                    AsignarValorSiNoNuloOVacio(sancion.num_anios, valor => num_anios.Add(valor), "");
                    AsignarValorSiNoNuloOVacio(sancion.des_tipexp, valor => des_tipexp.Add(valor), "");
                    AsignarValorSiNoNuloOVacio(sancion.des_motivo, valor => des_motivo.Add(valor), "");
                    AsignarValorSiNoNuloOVacio(sancion.detalle_motivo, valor => detalle_motivo.Add(valor), "");
                    AsignarValorSiNoNuloOVacio(sancion.expe_rel, valor => expe_rel.Add(valor), "");
                    AsignarValorSiNoNuloOVacio(sancion.tipo_rel, valor => tipo_rel.Add(valor), "");
                    AsignarValorSiNoNuloOVacio(sancion.fla_tipo, valor => fla_tipo.Add(valor), "");

                    fecha.Add(sancion.fecha);


                    AsignarValorSiNoNuloOVacio(sancion.encargado, valor => encargado.Add(valor), "");
                    AsignarValorSiNoNuloOVacio(sancion.resolucion, valor => resolucion.Add(valor), "");

                    fec_apertura.Add(sancion.fec_apertura);

                    AsignarValorSiNoNuloOVacio(sancion.cod_estado_expe, valor => cod_estado_expe.Add(Int32.Parse(valor)), "1");
                    AsignarValorSiNoNuloOVacio(sancion.cod_estado_reso, valor => cod_estado_reso.Add(Int32.Parse(valor)), "1");


                }

                // Convertir las listas en arrays y asignarlos a las propiedades correspondientes
                datos_verifica.as_origen = origen.ToArray();
                datos_verifica.as_expediente = expediente.ToArray();
                datos_verifica.as_instancia = instancia.ToArray();
                datos_verifica.as_sancion = sancionList.ToArray();
                datos_verifica.as_estado = estado.ToArray();
                datos_verifica.as_observaciones = observaciones.ToArray();

                datos_verifica.an_orden = orden.ToArray();
                datos_verifica.an_principal = principal.ToArray();
                datos_verifica.an_cod_distrito_origen = cod_distrito_origen.ToArray();
                datos_verifica.an_cod_intexp = cod_intexp.ToArray();
                datos_verifica.an_num_secuen = num_secuen.ToArray();
                datos_verifica.an_cod_resinf = cod_resinf.ToArray();
                datos_verifica.an_cod_estado_reso = cod_estado.ToArray();
                datos_verifica.an_cod_encarg = cod_encarg.ToArray();

                datos_verifica.as_num_expedi = num_expedi.ToArray();
                datos_verifica.as_num_anios = num_anios.ToArray();
                datos_verifica.as_des_tipexp = des_tipexp.ToArray();
                datos_verifica.as_des_motivo = des_motivo.ToArray();
                datos_verifica.as_detalle_motivo = detalle_motivo.ToArray();
                datos_verifica.as_expe_rel = expe_rel.ToArray();
                datos_verifica.as_tipo_rel = tipo_rel.ToArray();
                datos_verifica.as_fla_tipo = fla_tipo.ToArray();

                datos_verifica.ad_fecha = fecha.ToArray();


                datos_verifica.as_encargado = encargado.ToArray();
                datos_verifica.as_resolucion = resolucion.ToArray();
                datos_verifica.ad_fec_apertura = fec_apertura.ToArray();
                datos_verifica.an_cod_estado_expe = cod_estado_expe.ToArray();
                datos_verifica.an_cod_estado_reso = cod_estado_reso.ToArray();




                RptVerificacionBE rpta_validacion = await comunAD.VericarAntecedentes(datos_verifica);
                if ((rpta_validacion.rn_codigo != 0 && rpta_validacion.rn_codigo != 1))
                {
                    rspApi.codigo = -1;
                    rspApi.valor = "ERROR API  VericarAntecedentes: " + rpta_validacion.rs_valor;
                    return rspApi;
                }

                if (rpta_validacion.codigo == -1)
                {
                    rspApi.codigo = -1;
                    rspApi.valor = "ERROR API  VericarAntecedentes: " + rpta_validacion.valor;
                    return rspApi;

                }

                //optener credenciales de Zavala para subir a Zavala 

                CredencialesFTPBE credeniales = await genericoAD.GetCredencialesFTP(datos.cod_distrito_ftp);
                FtpClient FTP = new FtpClient("ftp://" + credeniales.server_ftp3, credeniales.user_ftp3, credeniales.pass_ftp3);

                if (credeniales == null)
                {
                    rspApi.codigo = -1;
                    rspApi.valor = "No se pudo obtener las Credenciales del FTP";
                    return rspApi;

                }
                if (rpta_validacion.rn_codigo == 1)
                {
                    //DESCARGA DE FTP y Guardamos el pdf en ruta temporal y retornamos el OK

                    byte[] byte_pdf = FTP.downloadBytes(rpta_validacion.rs_ruta_ftp);



                    if (byte_pdf == null)
                    {
                        rspApi.codigo = -1;
                        rspApi.valor = "Error: El archivo se subió dañado, o no coincide con el la ruta registrada.";
                        return rspApi;

                    }
                    bool esValido = EsArchivoPDF(byte_pdf);

                    if (!esValido)
                    {
                        rspApi.codigo = -1;
                        rspApi.valor = "ERROR al Descargar Archivo del Servidor FTP, Archivo Dañado o No se encuentra en la ruta proporcionada";
                        return rspApi;



                    }

                    rspApi.codigo = 0;
                    rspApi.valor = rpta_validacion.rn_cod_antec.ToString() + "~" + rpta_validacion.rs_ruta_ftp;
                    rspApi.ArchivosPDF = byte_pdf;
                    rspApi.fec_vigencia = rpta_validacion.rd_fec_vigencia.ToString("dd/MM/yyyy");
                    rspApi.fec_expedicion = rpta_validacion.rs_fec_creacion.ToString("dd/MM/yyyy");
                    rspApi.nro_registro = rpta_validacion.rs_nro_registro;
                    rspApi.anio = rpta_validacion.rs_anio;
                    return rspApi;

                }

                //SI EN CASO ES 0 CONTINUA Y SE CONSTRUYE EL PDF
                MemoryStream memoryStream = new MemoryStream();
                string rutaImagenFondo = datos.fondo_certificado;


                try
                {

                    PdfWriter writer = new PdfWriter(memoryStream);


                    PdfDocument pdf = new PdfDocument(writer);

                    iText.Layout.Document document = new iText.Layout.Document(pdf);
                    document.SetBottomMargin(163);
                    document.SetTopMargin(260);






                    PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD); // Por ejemplo, Helvetica



                    float[] columnWidthsSanc = { 10f, 15f, 13f, 12f, 10f, 10f, 30f };
                    float[] columnWidthsExpe = { 5f, 15f, 13f, 12f, 10f, 20f, 25f, 10f };
                    float[] columnWidths = (datos.cod_tipo_repo == 1) ? columnWidthsSanc : columnWidthsExpe;




                    
                    float font_zise = 6f;
                    if (sanciones.Count != 0)
                    {
                        Table table = new Table(columnWidths).UseAllAvailableWidth();
                        table.SetWidth(UnitValue.CreatePercentValue(100f));
                        PdfFont cellFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                        Dictionary<string, string[]> titulosPorTipoReporte = new Dictionary<string, string[]>
                                {
                                    { "1", new string[] { "ORIGEN", "EXPEDIENTE", "INSTANCIA", "SANCIÓN", "FECHA", "ESTADO", "OBSERVACIÓN" } },
                                    { "2", new string[] { "N°", "EXPEDIENTE", "D.J.ORIGEN", "FEC.RECEPCIÓN", "ESTADO", "ENCARGADO", "RESOLUCIÓN", "FEC.RESOLUCIÓN" } }
                                };

                        string[] titulos = titulosPorTipoReporte[datos.cod_tipo_repo.ToString()];

                        for (int i = 0; i < titulos.Length; i++)
                        {
                            Cell cell = new Cell().Add(new Paragraph(titulos[i]).SetFontSize(7f));
                            cell.SetTextAlignment(TextAlignment.CENTER);
                            table.AddCell(cell);

                        }
                        if (datos.cod_tipo_repo == 1)
                        {
                            foreach (ExpedienteSancionesBE sancion in sanciones)
                            {
                                string sfecha;
                                if (sancion.fecha == default(DateTime) || sancion.fecha == null)
                                {
                                    sfecha = "";
                                }
                                else
                                {
                                    sfecha = sancion.fecha.ToString("d/MM/yyyy");
                                }
                                table.AddCell(new Cell().Add(new Paragraph((sancion.origen != null) ? sancion.origen : "").SetFont(font)).SetFontSize(font_zise));
                                table.AddCell(new Cell().Add(new Paragraph((sancion.expediente != null) ? sancion.expediente : "").SetFont(cellFont)).SetFontSize(font_zise));
                                table.AddCell(new Cell().Add(new Paragraph((sancion.instancia != null) ? sancion.instancia : "").SetFont(cellFont)).SetFontSize(font_zise));
                                table.AddCell(new Cell().Add(new Paragraph((sancion.sancion != null) ? sancion.sancion : "").SetFont(cellFont)).SetFontSize(font_zise));
                                table.AddCell(new Cell().Add(new Paragraph((sfecha != null) ? sfecha : "").SetFont(cellFont)).SetFontSize(font_zise));
                                table.AddCell(new Cell().Add(new Paragraph((sancion.estado != null) ? sancion.estado : "").SetFont(cellFont)).SetFontSize(font_zise));
                                table.AddCell(new Cell().Add(new Paragraph((sancion.observaciones != null) ? sancion.observaciones : "").SetFont(cellFont)).SetFontSize(font_zise));

                            }
                        }
                        else if (datos.cod_tipo_repo == 2)
                        {
                            foreach (ExpedienteSancionesBE sancion in sanciones)
                            {

                                string sfec_apertura;
                                if (sancion.fec_apertura == default(DateTime) || sancion.fec_apertura == null)
                                {
                                    sfec_apertura = "";
                                }
                                else
                                {
                                    sfec_apertura = sancion.fec_apertura.ToString("d/MM/yyyy");
                                }

                                // Verifica si la fecha es la fecha por defecto o es nula
                                string sfecha;
                                if (sancion.fecha == default(DateTime) || sancion.fecha == null)
                                {
                                    sfecha = "";
                                }
                                else
                                {
                                    sfecha = sancion.fecha.ToString("d/MM/yyyy");
                                }
                                table.AddCell(new Cell().Add(new Paragraph((sancion.orden != null) ? sancion.orden : "").SetFont(font)).SetFontSize(font_zise));
                                table.AddCell(new Cell().Add(new Paragraph((sancion.expediente != null) ? sancion.expediente : "").SetFont(cellFont)).SetFontSize(font_zise));
                                table.AddCell(new Cell().Add(new Paragraph((sancion.origen != null) ? sancion.origen : "").SetFont(cellFont)).SetFontSize(font_zise));
                                table.AddCell(new Cell().Add(new Paragraph((sfec_apertura != null) ? sfec_apertura : "").SetFont(cellFont)).SetFontSize(font_zise));
                                table.AddCell(new Cell().Add(new Paragraph((sancion.estado != null) ? sancion.estado : "").SetFont(cellFont)).SetFontSize(font_zise));
                                table.AddCell(new Cell().Add(new Paragraph((sancion.encargado != null) ? sancion.encargado : "").SetFont(cellFont)).SetFontSize(font_zise));
                                table.AddCell(new Cell().Add(new Paragraph((sancion.resolucion != null) ? sancion.resolucion : "").SetFont(cellFont)).SetFontSize(font_zise));
                                table.AddCell(new Cell().Add(new Paragraph((sfecha != null) ? sfecha : "").SetFont(cellFont)).SetFontSize(font_zise));

                            }
                        }

                        document.Add(table);

                    }
                    else
                    {
                        Table table = new Table(1);
                        table.SetWidth(UnitValue.CreatePercentValue(100f));
                       
                        Cell emptyCell = new Cell().Add(new Paragraph("NO REGISTRA ANTECEDENTES"));
                        emptyCell.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
                        emptyCell.SetFontSize(11f);
                        emptyCell.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                        emptyCell.SetPadding(10f);
                        emptyCell.SetTextAlignment(TextAlignment.CENTER);
                        emptyCell.SetHeight(250f);


                        table.AddCell(emptyCell);

                        document.Add(table);
                    }


                    RecordSancionesBE recordSanc = new RecordSancionesBE();
                    RecordExpedientesBE recordExpe = new RecordExpedientesBE();
                    if (datos.cod_tipo_repo == 1)
                    {
                        recordSanc = comunAD.RecordSancionesTotal(datos.cod_intper.ToString(), datos.cod_subtipo_repo);
                        if (recordSanc.codigo == -1)
                        {
                            rspApi.codigo = -1;
                            rspApi.valor = "Se produjo un error:" + recordSanc.valor;
                            return rspApi;

                        }
                    }
                    else if (datos.cod_tipo_repo == 2)
                    {
                        recordExpe = comunAD.RecordExpedientesTotal(datos.cod_intper.ToString(), datos.cod_subtipo_repo);
                        if (recordExpe.codigo == -1)
                        {
                            rspApi.codigo = -1;
                            rspApi.valor = "Se produjo un error:" + recordExpe.valor;
                            return rspApi;

                        }
                    }





                    LineSeparator line = new LineSeparator(new SolidLine(1f));
                    line.SetWidth(PageSize.DEFAULT.GetWidth() - 72);
                    Cell lineaCell = new Cell().Add(new Paragraph().Add(line));
                    lineaCell.SetBorder(Border.NO_BORDER);
                    lineaCell.SetPaddingTop(3f);

                    document.Add(lineaCell);

                    if (datos.cod_tipo_repo == 1)
                    {
                        // Crear una tabla para organizar los datos en 4 columnas
                        Table cantidadesTable = new Table(4).UseAllAvailableWidth();

                        // Crear una fuente en negrita
                        PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                        // Definir los nombres de los campos y sus valores correspondientes
                        string[] nombresCampos = { "TOTAL DE AMONESTACIÓN(ES)", "TOTAL DE SUSPENSIÓN(ES)", "TOTAL DE MULTA(S)", "TOTAL DE DESTITUCIÓN(ES)", "TOTAL DE SANCIÓN(ES)" };
                        string[] valoresCampos = { ": " + (string.IsNullOrEmpty(recordSanc.amonestaciones) ? "0" : recordSanc.amonestaciones), ": " + (string.IsNullOrEmpty(recordSanc.suspensiones) ? "0" : recordSanc.suspensiones), ": " + (string.IsNullOrEmpty(recordSanc.multas) ? "0" : recordSanc.multas), ": " + (string.IsNullOrEmpty(recordSanc.destituciones) ? "0" : recordSanc.destituciones), ": " + (string.IsNullOrEmpty(recordSanc.sanciones) ? "0" : recordSanc.sanciones) };

                        // Iterar sobre los datos y agregarlos a la tabla
                        for (int i = 0; i < nombresCampos.Length; i++)
                        {
                            // Agregar el nombre del campo en una celda
                            Cell nombreCell = new Cell().Add(new Paragraph(nombresCampos[i]).SetFont(boldFont));
                            nombreCell.SetBorder(Border.NO_BORDER);
                            nombreCell.SetFontSize(font_zise);
                            cantidadesTable.AddCell(nombreCell);

                            // Agregar el valor del campo en otra celda
                            Cell valorCell = new Cell().Add(new Paragraph(valoresCampos[i]).SetFont(boldFont));
                            valorCell.SetBorder(Border.NO_BORDER);
                            valorCell.SetFontSize(font_zise);
                            cantidadesTable.AddCell(valorCell);
                        }

                        // Agregar la tabla al documento
                        document.Add(cantidadesTable);
                    }
                    else
                    {
                        // Crear una tabla para contener las tres tablas de cantidades acumuladas
                        Table cantidadesTable = new Table(new float[] { 25f, 7f, 25f, 7, 25, 6f }).UseAllAvailableWidth();

                        // Crear una fuente en negrita
                        PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                        // Definir los nombres de los campos y sus valores correspondientes
                        string[] nombresCampos = {
                                "TOTAL DE QUEJA(S)",
                                "TOTAL DE INVESTIGACIÓN(ES)",
                                "TOTAL DE VISITA(S)",
                                "TOTAL MED.CAUTELAR(ES)",
                                "EXPEDIENTES ARCHIVADO(S)",
                                " TOTAL DE EXPEDIENTE(S)",
                                "TOTAL REHABILITACIÓN(ES)",
                                "EXPEDIENTE(S) EN TRÁMITE",
                                "TOTAL DE SANCIÓN(ES)"
                            };
                        string[] valoresCampos = {
                                ": " + (string.IsNullOrEmpty(recordExpe.quejas) ? "0" : recordExpe.quejas),
                                ": " + (string.IsNullOrEmpty(recordExpe.investigaciones) ? "0" : recordExpe.investigaciones),
                                ": " + (string.IsNullOrEmpty(recordExpe.visitas) ? "0" : recordExpe.visitas),
                                ": " + (string.IsNullOrEmpty(recordExpe.medidas_cautelares) ? "0" : recordExpe.medidas_cautelares),
                                ": " + (string.IsNullOrEmpty(recordExpe.archivados) ? "0" : recordExpe.archivados),
                                ": " + (string.IsNullOrEmpty(recordExpe.en_tramite) ? "0" : recordExpe.en_tramite),
                                ": " + (string.IsNullOrEmpty(recordExpe.rehabilitaciones) ? "0" : recordExpe.rehabilitaciones),
                                ": " + (string.IsNullOrEmpty(recordExpe.expedientes) ? "0" : recordExpe.expedientes),
                                ": " + (string.IsNullOrEmpty(recordExpe.sanciones) ? "0" : recordExpe.sanciones)
                            };

                        // Iterar sobre los datos y agregarlos a la tabla
                        for (int i = 0; i < nombresCampos.Length; i++)
                        {
                            // Agregar el nombre del campo en una celda
                            Cell nombreCell = new Cell().Add(new Paragraph(nombresCampos[i]).SetFont(boldFont));
                            nombreCell.SetBorder(Border.NO_BORDER);
                            nombreCell.SetFontSize(font_zise);
                            cantidadesTable.AddCell(nombreCell);

                            // Agregar el valor del campo en otra celda
                            Cell valorCell = new Cell().Add(new Paragraph(valoresCampos[i]).SetFont(boldFont));
                            valorCell.SetBorder(Border.NO_BORDER);
                            valorCell.SetFontSize(font_zise);
                            cantidadesTable.AddCell(valorCell);
                        }

                        // Agregar la tabla al documento
                        document.Add(cantidadesTable);
                    }

                    LineSeparator line2 = new LineSeparator(new SolidLine(1f));
                    line2.SetWidth(PageSize.DEFAULT.GetWidth() - 72);
                    Cell lineaCell2 = new Cell().Add(new Paragraph().Add(line));
                    lineaCell2.SetBorder(Border.NO_BORDER);
                    lineaCell2.SetPaddingTop(3f);

                    document.Add(lineaCell2);

                    List<PiePagAntecedenteSancionesBE> sanciones_pie_pag = new List<PiePagAntecedenteSancionesBE>();


                    if (datos.cod_tipo_repo == 1)
                    {
                        sanciones_pie_pag = comunAD.AntecedentesSancionesPiePagina(datos.cod_intper.ToString(), datos.cod_subtipo_repo);

                    }
                    else if (datos.cod_tipo_repo == 2)
                    {
                        sanciones_pie_pag = comunAD.AntecedentesPiePaginaExpediente(datos.cod_intper.ToString(), datos.cod_subtipo_repo);

                    }
                    if (sanciones_pie_pag.Count == 0)
                    {
                        rspApi.codigo = -1;
                        rspApi.valor = "No se encontro información referente al pie de página";
                        return rspApi;

                    }
                    string footerText = "";

                    for (int i = 0; i < sanciones_pie_pag.Count; i++)
                    {

                        // Obtener el texto actual
                        string currentText = sanciones_pie_pag[i].notas;

                        // Verificar si el texto actual supera los 45 caracteres
                        if (currentText.Length > 125)
                        {
                            // Dividir el texto en dos partes: los primeros 45 caracteres y el resto del texto
                            string firstPart = currentText.Substring(0, 125);
                            string secondPart = currentText.Substring(125);

                            // Agregar la primera parte del texto con un salto de línea al final
                            footerText += firstPart + "\n";

                            // Agregar la segunda parte del texto
                            footerText += secondPart;
                        }
                        else
                        {
                            // Si el texto no supera los 45 caracteres, agregarlo sin cambios
                            footerText += currentText;
                        }

                        // Agregar un salto de línea al final de cada iteración (excepto en la última)
                        if (i < sanciones_pie_pag.Count - 1)
                        {
                            footerText += "\n";
                        }
                    }


                    Paragraph paragraph = new Paragraph(footerText);
                    paragraph.SetFontSize(font_zise);
                    document.Add(paragraph);



                    // Cerrar el documento
                    document.Close();



                }

                catch (Exception ex)
                {
                    rspApi.codigo = -1;
                    rspApi.valor = "Se produjo un error al generar el documento";
                    return rspApi;

                }




                try
                {
                    byte[] pdfBytes = memoryStream.ToArray();


                    byte[] strem = AgregaMarcaAguaImagen(pdfBytes, rutaImagenFondo);

                    if (!EsArchivoPDF(strem))
                    {
                        rspApi.codigo = -1;
                        rspApi.valor = "El arreglo de bytes AMA no representa un archivo PDF válido.";
                        return rspApi;


                    }


                    byte[] doc_encabezado = Encabezado_DatosPersonales(strem, datos.foto, datos.ap_paterno, datos.ap_materno, datos.nombres, datos.num_documento, datos.cod_tipo_repo);

                    if (!EsArchivoPDF(doc_encabezado))
                    {
                        rspApi.codigo = -1;
                        rspApi.valor = "El arreglo de bytes  no representa un archivo PDF válido Encabezado.";
                        return rspApi;


                    }
                    RptVerificarDocBE _codigoVerificacion = comunAD.VericarDoc(datos.cod_intper.ToString());
                    byte[] rptaArchivoSello = SelloVerificador(doc_encabezado, _codigoVerificacion.vd_codigo, _codigoVerificacion.vd_clave);

                    if (!EsArchivoPDF(rptaArchivoSello))
                    {
                        rspApi.codigo = -1;
                        rspApi.valor = "El arreglo de bytes no representa un archivo PDF válido Sello Verificación.";
                        return rspApi;

                    }




                    // actualizamos BD

                    datos_verifica.an_n_vd_id = _codigoVerificacion.vd_id;
                    datos_verifica.as_accion = "N";
                    datos_verifica.as_nombre_doc = System.IO.Path.GetFileName(_codigoVerificacion.ruta_ftp_publicado); 

                    RptVerificacionBE rptinsertAntecedentes = await comunAD.VericarAntecedentes(datos_verifica);
                    if (rptinsertAntecedentes.rn_codigo != 0 && rptinsertAntecedentes.rn_codigo != 1)
                    {
                        rspApi.codigo = -1;
                        rspApi.valor = "ERROR al Registrar los antecedentes: " + rptinsertAntecedentes.rs_valor;
                        return rspApi;
                    }

                    if (rptinsertAntecedentes.codigo == -1)
                    {
                        rspApi.codigo = -1;
                        rspApi.valor = "ERROR al Registrar los antecedentes: " + rptinsertAntecedentes.valor;
                        return rspApi;

                    }

                    ActualizarVerificaDocBE actualizar_zav = new ActualizarVerificaDocBE();

                    actualizar_zav.an_cod_antec = rptinsertAntecedentes.rn_cod_antec;
                    actualizar_zav.an_vd_id = _codigoVerificacion.vd_id;
                    actualizar_zav.as_usuario = datos.cod_usuario.ToString();

                    RptVerificacionBE rpta_zav = comunAD.ActualizarVericaDoc(actualizar_zav);
                    if (rpta_zav.codigo != 0)
                    {
                        rspApi.codigo = -1;
                        rspApi.valor = "ERROR Actualizar Documento Zav: " + rpta_zav.valor;
                        return rspApi;

                    }

                    string operador = (datos.cod_origantec == 1) ? "USRSYS" : datos.cod_usuario;


                    byte[] rptaArchivoSello2 = await FirmaVerificadorPiePagAsync(rptaArchivoSello, operador, rptinsertAntecedentes.rs_fec_creacion.ToString("dd/MM/yyyy"), rptinsertAntecedentes.rs_fec_creacion.ToString("HH:mm:ss"), rptinsertAntecedentes.rd_fec_vigencia.ToString("dd/MM/yyyy"), rptinsertAntecedentes.rs_nro_registro + "-" + rptinsertAntecedentes.rs_anio);
                    if (rptaArchivoSello2 != null)
                    {
                        if (!EsArchivoPDF(rptaArchivoSello2))
                        {
                            rspApi.codigo = -1;
                            rspApi.valor = "El arreglo de bytes no representa un archivo PDF válido pie página.";
                            return rspApi;


                        }
                    }

                    //FIRMAMOS EL DOCUMENTO
                    RespuestaArchivoBE _rptFirmado = _firmaDigital.FirmarDocumento(rptaArchivoSello2, datos.ruta_certificado, datos.password_certificado, datos.ruta_imagen_firma, "PJSigner", "Lima-Perú", "CADe", "Autoridad Nacional \n de Control - PJ", true, rptinsertAntecedentes.rs_fec_creacion);

                    if (_rptFirmado.codigo != 0)
                    {
                        rspApi.codigo = -1;
                        rspApi.valor = "ERROR AL FIRMAR " + _rptFirmado.valor;
                        return rspApi;

                    }
                    else
                    {
                        if (!EsArchivoPDF(_rptFirmado.ArchivosPDF))
                        {
                            rspApi.codigo = -1;
                            rspApi.valor = "El arreglo de bytes no representa un archivo PDF válido pie página.";
                            return rspApi;


                        }
                    }
                    //Subimos al FTP
                    string nombreDirectorio = System.IO.Path.GetDirectoryName(_codigoVerificacion.ruta_ftp_original);


                    FTP.createDirectory(nombreDirectorio);

                    RespuestaBE subirFTPOriginal = FTP.uploadByte(_codigoVerificacion.ruta_ftp_original, _rptFirmado.ArchivosPDF);
                    if (subirFTPOriginal.codigo != "0")
                    {
                        rspApi.codigo = -1;
                        rspApi.valor = "ERROR al subir el documento original al FTP: " + subirFTPOriginal.valor;
                        return rspApi;


                    }

                    RespuestaBE subirFTPpublicado = FTP.uploadByte(_codigoVerificacion.ruta_ftp_publicado, _rptFirmado.ArchivosPDF);
                    if (subirFTPpublicado.codigo != "0")
                    {
                        rspApi.codigo = -1;
                        rspApi.valor = "ERROR al subir el documento original al FTP:" + subirFTPpublicado.valor;
                        return rspApi;


                    }

                    rspApi.ArchivosPDF = _rptFirmado.ArchivosPDF;
                    rspApi.valor = rptinsertAntecedentes.rn_cod_antec.ToString() + "~" + _codigoVerificacion.ruta_ftp_publicado;
                    rspApi.fec_vigencia = rptinsertAntecedentes.rd_fec_vigencia.ToString("dd/MM/yyyy");
                    rspApi.fec_expedicion = rptinsertAntecedentes.rs_fec_creacion.ToString("dd/MM/yyyy");
                    rspApi.nro_registro = rptinsertAntecedentes.rs_nro_registro;
                    rspApi.anio = rptinsertAntecedentes.rs_anio;
                    return rspApi;



                }
                catch (Exception ex)
                {
                    rspApi.codigo = -1;
                    rspApi.valor = "Surgio un problema al validar el certificado Digital";
                    return rspApi;
                }





            }
            catch (Exception ex)
            {
                rspApi.codigo = -1;
                rspApi.valor = "Se produjo un error - Inicializacion de variables:" + ex;
                return rspApi;
            }


            //return rspApi;
        }


        public byte[] Encabezado_DatosPersonales(byte[] bytesDocumento, byte[] foto, string ap_paterno, string ap_materno, string nombres, string num_documento, int tipo_reporte)
        {
            try
            {
                // Verificar si el arreglo de bytes es nulo o está vacío
                if (bytesDocumento == null || bytesDocumento.Length == 0)
                {
                    throw new ArgumentException("El arreglo de bytes del documento está vacío.");
                }

                // Verificar si los bytes del documento representan un archivo PDF
                if (!EsArchivoPDF(bytesDocumento))
                {
                    throw new ArgumentException("El arreglo de bytes no representa un archivo PDF válido.");
                }

                using (MemoryStream inputStream = new MemoryStream(bytesDocumento))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (PdfReader pdfReader = new PdfReader(inputStream))
                        {
                            using (PdfWriter pdfWriter = new PdfWriter(memoryStream))
                            {
                                PdfDocument pdfDocument = new PdfDocument(pdfReader, pdfWriter);
                                string tipo_antecedente = "";
                                if (tipo_reporte == 1)
                                {
                                    tipo_antecedente = "Disciplinarios";
                                }
                                else
                                {
                                    tipo_antecedente = "de Expedientes";
                                }
                                int totalPages = pdfDocument.GetNumberOfPages();

                                float width = PageSize.DEFAULT.GetWidth() - 72;
                                // Crear un nuevo layout para agregar contenido en cada página
                                Document doc = new Document(pdfDocument);
                                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);


                                // Encabezado
                                Table headerTable = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();

                                // Agregar imagen al encabezado (primera columna)
                                Cell logoCell = new Cell().SetBorder(Border.NO_BORDER);
                                logoCell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                                logoCell.SetWidth(UnitValue.CreatePercentValue(20));
                                headerTable.AddHeaderCell(logoCell);

                                // Crear tabla para las filas del medio (segunda columna)
                                Table middleTable = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
                                // Agregar las filas a la tabla del medio
                                middleTable.AddCell(new Cell().Add(new Paragraph("REPÚBLICA DEL PERÚ\r\nAUTORIDAD NACIONAL DE CONTROL").SetTextAlignment(TextAlignment.CENTER).SetFontSize(13).SetFont(font).SetWordSpacing(5)).SetBorder(Border.NO_BORDER));
                                middleTable.AddCell(new Cell().Add(new Paragraph("PODER JUDICIAL").SetTextAlignment(TextAlignment.CENTER).SetFontSize(13)).SetBorder(Border.NO_BORDER));
                                middleTable.AddCell(new Cell().Add(new Paragraph("Certificado de\r\nAntecedentes " + tipo_antecedente).SetTextAlignment(TextAlignment.CENTER).SetFontSize(13).SetFont(font)).SetBorder(Border.NO_BORDER));
                                middleTable.AddCell(new Cell().Add(new Paragraph("Válido por 90 días a partir de su fecha de expedición").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8).SetFont(font).SetFontColor(new DeviceRgb(255, 0, 0))).SetBorder(Border.NO_BORDER));
                                // Agregar la tabla del medio al encabezado (segunda columna)
                                Cell middleTableCell = new Cell().Add(middleTable).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER);
                                middleTableCell.SetWidth(UnitValue.CreatePercentValue(60));
                                headerTable.AddHeaderCell(middleTableCell);

                                // Agregar imagen de la persona al encabezado (tercera columna)
                                iText.Layout.Element.Image personImage = new iText.Layout.Element.Image(ImageDataFactory.Create(foto));

                          
                                personImage.ScaleToFit(130f, 130f);
                                personImage.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                                // Configurar las dimensiones de la celda para que coincidan con las dimensiones de la imagen                                

                                Cell personImageCell = new Cell().Add(personImage).SetBorder(new SolidBorder(new DeviceRgb(254, 189, 87), 0.25f));
                                personImageCell.SetHeight(130f);
                                personImageCell.SetWidth(130f);

                                headerTable.AddHeaderCell(personImageCell);

                                //-----------------------------------------------------------------------------------------------------------------------------//

                                // Crear una tabla para los datos del interesado
                                Table datosInteresadoTable = new Table(6).UseAllAvailableWidth();
                                datosInteresadoTable.SetBorder(Border.NO_BORDER);
                                int fontsize = 8;
                                float paddinBottom = 7f;
                                // Configurar las celdas para que no tengan bordes
                                datosInteresadoTable.AddCell(new Cell().Add(new Paragraph("Tipo de Doc. de Identidad:").SetFont(font)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetPaddingBottom(paddinBottom));
                                datosInteresadoTable.AddCell(new Cell().Add(new Paragraph("DNI")).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetPaddingBottom(paddinBottom));
                                datosInteresadoTable.AddCell(new Cell().Add(new Paragraph("Primer Apellido:").SetFont(font)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetPaddingBottom(paddinBottom));
                                datosInteresadoTable.AddCell(new Cell().Add(new Paragraph(ap_paterno)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetPaddingBottom(paddinBottom));
                                datosInteresadoTable.AddCell(new Cell().Add(new Paragraph("Nombres:").SetFont(font)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetPaddingBottom(paddinBottom));
                                datosInteresadoTable.AddCell(new Cell().Add(new Paragraph(nombres)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetPaddingBottom(paddinBottom));
                                datosInteresadoTable.AddCell(new Cell().Add(new Paragraph("Nro. Doc. de Identidad:").SetFont(font)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetPaddingBottom(paddinBottom));
                                datosInteresadoTable.AddCell(new Cell().Add(new Paragraph(num_documento)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetPaddingBottom(paddinBottom));
                                datosInteresadoTable.AddCell(new Cell().Add(new Paragraph("Segundo Apellido:").SetFont(font)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetPaddingBottom(paddinBottom));
                                datosInteresadoTable.AddCell(new Cell().Add(new Paragraph(ap_materno)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetPaddingBottom(paddinBottom));
                                datosInteresadoTable.AddCell(new Cell().Add(new Paragraph("Motivo").SetFont(font)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetPaddingBottom(paddinBottom));
                                datosInteresadoTable.AddCell(new Cell().Add(new Paragraph("Trámite Administrativo")).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetPaddingBottom(paddinBottom));

                                // Crear una celda que contenga toda la tabla de datos del interesado
                                Cell datosInteresadoCell = new Cell(1, 6).Add(datosInteresadoTable);
                                datosInteresadoCell.SetBorder(Border.NO_BORDER); // Eliminar el borde de la celda interna para que solo haya un borde alrededor de todas las celdas
                                datosInteresadoCell.SetPaddings(5f, 10f, 10f, 10f);



                                Table containerTable = new Table(1).UseAllAvailableWidth();
                                containerTable.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)); // Establecer borde negro de grosor 1 alrededor de la tabla
                                containerTable.SetMarginTop(10f);

                                // Agregar título al contenedor
                                Cell titleCell = new Cell().Add(new Paragraph("SE CERTIFICA QUE:").SetFont(font).SetFontSize(12f).SetTextAlignment(TextAlignment.LEFT));
                                titleCell.SetBorder(Border.NO_BORDER);
                                titleCell.SetPaddings(5f, 10f, 0, 10f);
                                containerTable.AddCell(titleCell);

                                // Agregar la celda que contiene los datos del interesado al contenedor
                                containerTable.AddCell(datosInteresadoCell);



                                for (int pageNumber = 1; pageNumber <= totalPages; pageNumber++)
                                {
                                    PdfPage page = pdfDocument.GetPage(pageNumber);


                                    // Agregar tabla a la página
                                    new Canvas(page, new iText.Kernel.Geom.Rectangle(doc.GetLeftMargin(), 630, width, 200)).Add(headerTable);

                                 

                                    new Canvas(page, new iText.Kernel.Geom.Rectangle(doc.GetLeftMargin(), 485, width, 200)).Add(containerTable);




                                }
                                // Cerrar el documento actual
                                doc.Close();
                                return memoryStream.ToArray();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Surgió un error al generar el documento.");
            }
        }
        public async Task<byte[]> FirmaVerificadorPiePagAsync(byte[] bytesDocumento, string operador, string fecha_expe, string hora, string fecha_caducidad, string nro_tramite)
        {
            try
            {
                // Verificar si el arreglo de bytes es nulo o está vacío
                if (bytesDocumento == null || bytesDocumento.Length == 0)
                {
                    throw new ArgumentException("El arreglo de bytes del documento está vacío.");
                }

                // Verificar si los bytes del documento representan un archivo PDF
                if (!EsArchivoPDF(bytesDocumento))
                {
                    throw new ArgumentException("El arreglo de bytes no representa un archivo PDF válido.");
                }

                using (MemoryStream inputStream = new MemoryStream(bytesDocumento))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (PdfReader pdfReader = new PdfReader(inputStream))
                        {
                            using (PdfWriter pdfWriter = new PdfWriter(memoryStream))
                            {
                                PdfDocument pdfDocument = new PdfDocument(pdfReader, pdfWriter);

                                int totalPages = pdfDocument.GetNumberOfPages();



                                // Crear un nuevo layout para agregar contenido
                                Document doc = new Document(pdfDocument);


                                // Posición de la tabla
                                float posX = doc.GetLeftMargin();
                                float posY = -50f;
                                // Mover a la posición especificada
                                doc.SetFixedPosition(posX, posY, PageSize.DEFAULT.GetWidth());

                                // Crear tabla

                                Table table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth();
                                table.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
                                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD); // Por ejemplo, Helvetica
                                int fontsize = 8;
                                // primera columna
                                Table infoleft = new Table(UnitValue.CreatePercentArray(2));
                                infoleft.AddCell(new Cell().Add(new Paragraph("Operador:").SetFont(font)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetTextAlignment(TextAlignment.LEFT));
                                infoleft.AddCell(new Cell().Add(new Paragraph(operador)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetTextAlignment(TextAlignment.LEFT));

                                infoleft.AddCell(new Cell().Add(new Paragraph("Expedido:").SetFont(font)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetTextAlignment(TextAlignment.LEFT));
                                infoleft.AddCell(new Cell().Add(new Paragraph(fecha_expe).SetFont(font)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetTextAlignment(TextAlignment.LEFT));

                                infoleft.AddCell(new Cell().Add(new Paragraph("Hora:").SetFont(font)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetTextAlignment(TextAlignment.LEFT));
                                infoleft.AddCell(new Cell().Add(new Paragraph(hora)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetTextAlignment(TextAlignment.LEFT));

                                Cell leftTableCell = new Cell().Add(infoleft).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER);
                                leftTableCell.SetWidth(UnitValue.CreatePercentValue(25));

                                table.AddCell(leftTableCell);

                                List<RptListaResponsableBE> RptListaResponsableBE = await comunAD.ListaResponsableAntecedentes();

                                if (RptListaResponsableBE.Count == 0)
                                {
                                    return null;
                                }
                                byte[] firmaBytes = RptListaResponsableBE[0].rbt_firma;

                                if (firmaBytes == null || firmaBytes.Length == 0)
                                {

                                    return null;
                                }


                                // Crear tabla para las filas del medio (segunda columna)
                                Table middleTable = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
                                // Agregar imagen de la persona al encabezado (tercera columna)

                                // Crear la imagen con iText

                                ImageData _firmaImage = ImageDataFactory.Create(firmaBytes);
                                iText.Layout.Element.Image fima_img = new iText.Layout.Element.Image(_firmaImage);
                                fima_img.ScaleToFit(50f, 50f);
                               
                                fima_img.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                                Cell fimaImgCell = new Cell().Add(fima_img).SetBorder(Border.NO_BORDER);

                                middleTable.AddCell(fimaImgCell);
                                // Agregar las filas a la tabla del medio
                                middleTable.AddCell(new Cell().Add(new Paragraph("----------------------------------------------------------------- \n" + RptListaResponsableBE[0].rs_abreviatura_responsable + " " + RptListaResponsableBE[0].rs_nombre_responsable + "\n" + RptListaResponsableBE[0].rs_cargo_responsable + "\n" + RptListaResponsableBE[0].rs_entidad_primera_linea + "\n" + RptListaResponsableBE[0].rs_entidad_segunda_linea)
                                    .SetTextAlignment(TextAlignment.CENTER)
                                    .SetFontSize(6))
                                    .SetBorder(Border.NO_BORDER));


                                Cell middleTableCell = new Cell().Add(middleTable).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER);
                                middleTableCell.SetWidth(UnitValue.CreatePercentValue(50));

                                table.AddCell(middleTableCell);

                                Table inforight = new Table(UnitValue.CreatePercentArray(2));
                                inforight.AddCell(new Cell().Add(new Paragraph("Caduca:").SetFont(font)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize));
                                inforight.AddCell(new Cell().Add(new Paragraph(fecha_caducidad)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize));
                                inforight.AddCell(new Cell().Add(new Paragraph("Nro. Trámite:").SetFont(font)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize));
                                inforight.AddCell(new Cell().Add(new Paragraph("C"+nro_tramite)).SetBorder(Border.NO_BORDER).SetFontSize(fontsize));
                                Cell rightTableCell = new Cell().Add(inforight).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT);
                                rightTableCell.SetWidth(UnitValue.CreatePercentValue(25));

                                table.AddCell(rightTableCell);

                                float width = PageSize.DEFAULT.GetWidth() - 72;



                                for (int pageNumber = 1; pageNumber <= totalPages; pageNumber++)
                                {
                                    PdfPage page = pdfDocument.GetPage(pageNumber);


                                    // Agregar tabla a la página
                                    new Canvas(page, new iText.Kernel.Geom.Rectangle(posX, posY, width, 205)).Add(table);
                                }

                                doc.Close();

                                return memoryStream.ToArray();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Surgió un error al generar el documento.");
            }
        }

     
        public byte[] SelloVerificador(byte[] bytesDocumento, string vercodigo, string verclave)
        {
            try
            {

                // Verificar si el arreglo de bytes es nulo o está vacío
                if (bytesDocumento == null || bytesDocumento.Length == 0)
                {
                    throw new ArgumentException("El arreglo de bytes del documento está vacío.");
                }

                // Verificar si los bytes del documento representan un archivo PDF
                if (!EsArchivoPDF(bytesDocumento))
                {
                    throw new ArgumentException("El arreglo de bytes no representa un archivo PDF válido.");
                }


                #region"Estampar sello verificador"
                using (MemoryStream inputStream = new MemoryStream(bytesDocumento))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (PdfReader pdfReader = new PdfReader(inputStream))
                        {
                            using (PdfWriter pdfWriter = new PdfWriter(memoryStream))
                            {

                                PdfDocument pdfDocument = new PdfDocument(pdfReader, pdfWriter);

                                Document doc = new Document(pdfDocument);


                                float width = PageSize.DEFAULT.GetWidth() - 72;
                                // Posición de la tabla
                                float posX = doc.GetLeftMargin();
                                float posY = -120f;

                                // Mover a la posición especificada
                                doc.SetFixedPosition(posX, posY, PageSize.DEFAULT.GetWidth());


                                int totalPages = pdfDocument.GetNumberOfPages();


                                for (int pageNumber = 1; pageNumber <= totalPages; pageNumber++)
                                {

                                    PdfPage page = pdfDocument.GetPage(pageNumber);
                                    Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
                                    table.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
                                    PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD); // Por ejemplo, Helvetica
                                    int fontsize = 7;

                                    string qrCodeText = $"https://anc.pj.gob.pe/verificadoc?codigo={vercodigo}&clave={verclave}";
                                    byte[] qrCodeData = GenerateQRCode(qrCodeText, 68, 68);
                                    ImageData imageData = ImageDataFactory.Create(qrCodeData);


                                    iText.Layout.Element.Image image_qr = new iText.Layout.Element.Image(imageData);
                                    image_qr.ScaleToFit(40, 40);
                                    image_qr.SetHorizontalAlignment(HorizontalAlignment.LEFT);
                                    Cell fimaImgCell = new Cell().Add(image_qr).SetBorder(Border.NO_BORDER);
                                    fimaImgCell.SetWidth(UnitValue.CreatePercentValue(10));
                                    table.AddCell(fimaImgCell);


                                    Cell infoleft = new Cell().SetBorder(Border.NO_BORDER);





                                    string pagingInfo = $"Página {pageNumber} de {totalPages}";
                                    infoleft.Add(new Paragraph("Esta es una copia auténtica de un documento electrónico archivado en la Autoridad Nacional de Control (ANC-PJ).\r\nSu autenticidad e integridad pueden ser contrastadas en: https://anc.pj.gob.pe/verificadoc\n\n")).SetBorder(Border.NO_BORDER).SetFontSize(fontsize).SetTextAlignment(TextAlignment.CENTER);
                                    infoleft.Add(new Paragraph()
                                            .SetFontColor(ColorConstants.DARK_GRAY)
                                            .SetFontSize(7)
                                            .Add("CÓDIGO: ")
                                            .SetFontColor(ColorConstants.BLACK)
                                            .Add(vercodigo)
                                            .SetFontColor(ColorConstants.DARK_GRAY)
                                            .Add("        CLAVE: ")
                                            .SetFontColor(ColorConstants.BLACK)
                                            .Add(verclave)
                                            .Add("                                                                                                             ")
                                            .Add(pagingInfo));
                                    table.AddCell(infoleft);


                                    new Canvas(page, new iText.Kernel.Geom.Rectangle(posX, posY, width, 185)).Add(table);

                                }

                                pdfDocument.Close();
                            }
                        }
                        return memoryStream.ToArray();
                    }
                }
                #endregion





            }
            catch (Exception ex)
            {
                throw new ArgumentException("Surgio un Error al generar el documento."); ;
            }
        }

        private static byte[] GenerateQRCode(string content, int width, int height)
        {
            // Crea un código QR blanco como fondo
            ZXing.Windows.Compatibility.BarcodeWriter barcodeWriter = new ZXing.Windows.Compatibility.BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            barcodeWriter.Options = new EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = 0,
            };

            // Genera el código QR
            var bitmap = barcodeWriter.Write(content);

            // Crea un fondo blanco
            System.Drawing.Bitmap whiteBackground = new System.Drawing.Bitmap(width, height);



            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(whiteBackground))
            {
                g.Clear(System.Drawing.Color.White);
            }

            // Superpone el código QR en el fondo blanco
            using (MemoryStream stream = new MemoryStream())
            {
                using (System.Drawing.Bitmap combinedBitmap = new System.Drawing.Bitmap(width, height))
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(combinedBitmap))
                using (System.Drawing.Graphics combinedGraphics = System.Drawing.Graphics.FromImage(combinedBitmap))
                {
                    combinedGraphics.DrawImage(whiteBackground, 0, 0);
                    combinedGraphics.DrawImage(bitmap, 0, 0);

                    combinedBitmap.Save(stream, ImageFormat.Png);

                    return stream.ToArray();
                }


            }
        }


        private bool EsArchivoPDF(byte[] bytes)
        {
            try
            {

                // El primer carácter de un archivo PDF es '%PDF'
                if (bytes.Length < 4 || !(bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46))
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }


            return true;
        }


        public byte[] AgregaMarcaAguaImagen(byte[] pdfBytes, string rutaImagen)
        {
            try
            {
                // Crear un MemoryStream a partir del array de bytes del PDF
                using (MemoryStream inputMemoryStream = new MemoryStream(pdfBytes))
                {
                    // Crear un MemoryStream para escribir el PDF resultante
                    using (MemoryStream outputMemoryStream = new MemoryStream())
                    {
                        // Crear un PdfWriter utilizando el MemoryStream de salida
                        using (PdfWriter pdfWriter = new PdfWriter(outputMemoryStream))
                        {
                            // Crear un PdfDocument a partir del PdfWriter
                            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputMemoryStream), pdfWriter))
                            {



                                ImageData imageData = ImageDataFactory.Create(rutaImagen);
                                iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);

                                // Configurar la imagen para que se ajuste al tamaño de la página
                                image.SetAutoScale(true);

                                // Recorrer cada página del documento original
                                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                                {
                                    // Obtener la página actual
                                    PdfPage page = pdfDoc.GetPage(i);

                                    // Crear un nuevo lienzo para la página actual
                                    PdfCanvas canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);

                                    // Agregar la imagen de fondo al lienzo
                                    image.SetFixedPosition(i, 0, 0);
                                    new Canvas(canvas, page.GetPageSize()).Add(image);
                                    // Copiar el contenido de la página original al nuevo lienzo
                                    PdfCanvas over = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);
                                    PdfFormXObject pageCopy = page.CopyAsFormXObject(pdfDoc);
                                    over.AddXObject(pageCopy, 0, 0);
                                }

                                // Cerrar el documento
                                pdfDoc.Close();


                            }
                        }
                        // Devolver el PDF resultante como un array de bytes
                        return outputMemoryStream.ToArray();
                    }


                }
            }
            catch (Exception ex)
            {
                LogAntecedentes.Logger().Error(" => Error: " + ex);

                return new byte[0]; ;
            }
        }
        void AsignarValorSiNoNuloOVacio<T>(T valor, Action<T> asignarAtributo, T valorPredeterminado)
        {
            if (!EqualityComparer<T>.Default.Equals(valor, default(T)) && !string.IsNullOrEmpty(valor?.ToString()))
            {
                asignarAtributo(valor);
            }
            else
            {
                asignarAtributo(valorPredeterminado);
            }
        }


        public byte[] CedulaDocAdministrativo(string escrito, string destinatario, string fecha_doc, string documento, string foja_ini, string distrito_jud, string fecha_recep, string contenido_doc, string[] remitente, string fecha_generacion, string hora_generacion, string usuario, string ruta_logo)
        {
            try
            {

                    MemoryStream memoryStream = new MemoryStream();
                
                    PdfWriter writer = new PdfWriter(memoryStream);
                    PdfDocument pdfDoc = new PdfDocument(writer);
                    Document document = new Document(pdfDoc);


                    // Crear encabezado
                    Table headerTable = new Table(UnitValue.CreatePercentArray(new float[] { 30f, 45f, 25f }));
                    headerTable.SetWidth(UnitValue.CreatePercentValue(100f));

                    Paragraph phrase = new Paragraph();
              
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont font_bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);



                phrase.Add(new iText.Layout.Element.Text("Poder Judicial").SetFont(font_bold).SetFontSize(10f));
                    phrase.Add(new iText.Layout.Element.Text("\n"));
                    phrase.Add(new iText.Layout.Element.Text("Autoridad Nacional de Control del Poder Judicial.").SetFont(font).SetFontSize(8f));

                    Cell textCell = new Cell().Add(phrase).SetBorder(Border.NO_BORDER).SetPaddingBottom(20f);
                    textCell.SetTextAlignment(TextAlignment.LEFT);
                    headerTable.AddCell(textCell);

                    // Agregar imagen de logo
                    ImageData imageData = ImageDataFactory.Create(ruta_logo);
                    Image itextImage = new Image(imageData);
                    itextImage.ScaleToFit(130f, 130f);
                    Cell logoCell = new Cell().Add(itextImage).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER);
                    logoCell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                    logoCell.SetPaddingLeft(40f);
                    headerTable.AddCell(logoCell);

                    document.Add(headerTable);


                    Paragraph SubName = new Paragraph("Autoridad Nacional de Control \n Poder Judicial").SetFont(font).SetFontSize(8f);
                    SubName.SetTextAlignment(TextAlignment.CENTER);

                    document.Add(SubName);




                    // Agregar título
                    Paragraph title = new Paragraph("CARGO DE INGRESO DE DOCUMENTO ADMINISTRATIVO").SetFont(font_bold).SetFontSize(12f);
                    title.SetTextAlignment(TextAlignment.CENTER);
                    title.SetMarginTop(10f);

                    document.Add(title);


                    // Crear un contenedor para los datos del interesado
                    LineSeparator line = new LineSeparator(new SolidLine(1f));
                    line.SetWidth(PageSize.DEFAULT.GetWidth() - 72);
                    Cell lineaCell = new Cell().Add(new Paragraph().Add(line));
                    lineaCell.SetBorder(Border.NO_BORDER);
                    lineaCell.SetPaddingTop(3f);

                    document.Add(lineaCell);



                    // Crear contenedor para datos del interesado
                    Table datosInteresadoTable = new Table(UnitValue.CreatePercentArray(new float[] { 15f, 85f }));
                    datosInteresadoTable.SetWidth(UnitValue.CreatePercentValue(100f));
                    datosInteresadoTable.SetBorder(Border.NO_BORDER);



                    // Agregar datos del interesado
                    string[] names = { "Escrito Nro :", "Destinatario :", "Fec.Doc :", "Documento :", "Foja Inicial :", "Distrito Judicial :", "Fec. Recep. :" };
                    string[] values = { escrito, destinatario, fecha_doc, documento, foja_ini, distrito_jud, fecha_recep };

                    for (int i = 0; i < names.Length; i++)
                    {
                        datosInteresadoTable.AddCell(new Cell().Add(new Paragraph(names[i])).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(10f));
                        datosInteresadoTable.AddCell(new Cell().Add(new Paragraph(values[i])).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10f));
                    }

                    datosInteresadoTable.SetMarginBottom(10f);


                    document.Add(datosInteresadoTable);








                    // Agregar contenido del documento
                    document.Add(new Paragraph("CONTENIDO DEL DOCUMENTO").SetFont(font_bold).SetFontSize(10f));
                    document.Add(new Paragraph(contenido_doc).SetFont(font).SetFontSize(8f));

                    // Agregar remitentes
                    document.Add(new Paragraph("REMITENTE(S)").SetFont(font_bold).SetFontSize(10f));
                    foreach (string remit in remitente)
                    {
                        document.Add(new Paragraph(remit).SetFont(font).SetFontSize(8f));
                    }







                    document.Add(new Paragraph("\n\n\n\n"));

                    // Agregar sección de firmas
                    Table signatureTable = new Table(UnitValue.CreatePercentArray(new float[] { 50f, 50f }));
                    signatureTable.SetWidth(UnitValue.CreatePercentValue(100f));

                    signatureTable.AddCell(new Cell().Add(new Paragraph("-----------------------------------------------\n Emisor").SetFont(font).SetFontSize(10f)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER));
                    signatureTable.AddCell(new Cell().Add(new Paragraph("-----------------------------------------------\n Representante").SetFont(font).SetFontSize(10f)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER));

                    document.Add(signatureTable);

                    document.Close();

                    byte[] CedulaPDF = memoryStream.ToArray();
                    // Generar paginado
                    byte[] CedulaPDF_sellado = GenerarPaginadoCargo(CedulaPDF, fecha_generacion, hora_generacion, usuario);

                    if (CedulaPDF_sellado == null)
                    {
                        return null;
                    }
                    bool valid_sellado = EsArchivoPDF(CedulaPDF_sellado);
                    if (!valid_sellado)
                    {
                        return null;

                    }
                    return CedulaPDF_sellado;
                
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public byte[] GenerarPaginadoCargo(byte[] bytesDocumento, string fecha, string hora, string usuario)
        {


            try
            {
                using (MemoryStream inputStream = new MemoryStream(bytesDocumento))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (PdfReader reader = new PdfReader(inputStream))
                        {
                            using (PdfWriter writer = new PdfWriter(memoryStream))
                            {
                                using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
                                {
                                    int totalPages = pdfDoc.GetNumberOfPages();

                                    Document document = new Document(pdfDoc);



                                    float width = PageSize.DEFAULT.GetWidth() - 72;


                                    for (int i = 1; i <= totalPages; i++)
                                    {
                                        // Agregar página del PDF original al nuevo documento
                                        PdfPage page = pdfDoc.GetPage(i);


                                        // Crear tabla con información de fecha, hora, página y usuario
                                        Table tableRight = new Table(UnitValue.CreatePercentArray(new float[] { 35f, 65f }));
                                        tableRight.SetWidth(UnitValue.CreatePercentValue(25f));
                                        tableRight.SetTextAlignment(TextAlignment.RIGHT);

                                        tableRight.AddCell(new Cell().Add(new Paragraph("Fecha").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10f)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));
                                        tableRight.AddCell(new Cell().Add(new Paragraph(": " + fecha).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10f)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT));

                                        tableRight.AddCell(new Cell().Add(new Paragraph("Hora").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10f)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));
                                        tableRight.AddCell(new Cell().Add(new Paragraph(": " + hora).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10f)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT));

                                        tableRight.AddCell(new Cell().Add(new Paragraph("Página").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10f)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));
                                        tableRight.AddCell(new Cell().Add(new Paragraph(": " + i + " de " + totalPages).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10f)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT));

                                        tableRight.AddCell(new Cell().Add(new Paragraph("Usuario").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10f)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));
                                        tableRight.AddCell(new Cell().Add(new Paragraph(": " + usuario).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10f)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT));

                                        float tableWidth = 100f;
                                        float x = 430;
                                        float y = 720;


                                        Canvas canvas = new Canvas(new PdfCanvas(page), new iText.Kernel.Geom.Rectangle(x, y, width, 100));
                                        canvas.Add(tableRight);

                                    }

                                    document.Close();

                                    return memoryStream.ToArray();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }


        }


        public RespuestaArchivoBE  FormularioDocAdministrativo(TramiteDocumentarioBE datos)
        {
            RespuestaArchivoBE rptFormulario=new RespuestaArchivoBE();
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                
                    PdfWriter writer = new PdfWriter(memoryStream);
                    PdfDocument pdfDoc = new PdfDocument(writer);
                    Document document = new Document(pdfDoc);

                 

            
                     Table containerTable = new Table(1);
                    containerTable.SetWidth(UnitValue.CreatePercentValue(100f));


                    // Crear encabezado
                    Table headerTable = new Table(UnitValue.CreatePercentArray(new float[] { 20f, 60f, 20f }));
                    headerTable.SetWidth(UnitValue.CreatePercentValue(100f));

                    float fontzise = 8f;

                    // Agregar imagen de logo
                    ImageData scudo_peru = ImageDataFactory.Create(datos.ruta_escudo);
                    Image itextScudo = new Image(scudo_peru);
                    itextScudo.ScaleToFit(50f, 50f);
                    Cell logoCellScudo = new Cell().Add(itextScudo).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT);
                    logoCellScudo.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                    logoCellScudo.SetPaddingLeft(40f);
                    headerTable.AddCell(logoCellScudo);

                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont font_bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);


                Paragraph phrase = new Paragraph();
                    phrase.Add(new iText.Layout.Element.Text("FORMULARIO UNICO TRAMITES ADMINISTRATIVOS DEL PODER JUDICIAL\n").SetFont(font_bold).SetFontSize(fontzise));

                    phrase.Add(new iText.Layout.Element.Text("R.A. N° 304-2014-CE-PJ\n").SetFont(font).SetFontSize(6f));

                    phrase.Add(new iText.Layout.Element.Text("DISTRIBUCION GRATUITA").SetFont(font_bold).SetFontSize(fontzise));

                    Cell textCell = new Cell().Add(phrase).SetBorder(Border.NO_BORDER).SetPaddingBottom(20f);
                    textCell.SetTextAlignment(TextAlignment.CENTER);
                    headerTable.AddCell(textCell);

                    // Agregar imagen de logo
                    ImageData imageData = ImageDataFactory.Create(datos.ruta_logo_pj);
                    Image itextImage = new Image(imageData);
                    itextImage.ScaleToFit(50f, 50f);
                    Cell logoCell = new Cell().Add(itextImage).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT);
                    logoCell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                    logoCell.SetPaddingLeft(40f);
                    headerTable.AddCell(logoCell);
                    containerTable.AddCell(headerTable);



                    // Crear un Div con fondo oscuro y letras blancas
                    Div div = new Div();
                    div.SetBackgroundColor(ColorConstants.BLACK); // Establecer el color de fondo como negro
                    // Crear un Paragraph con texto y letras blancas
                    Paragraph paragraph = new Paragraph("I. RESUMEN DEL PEDIDO").SetFontSize(fontzise);
                    paragraph.SetFontColor(ColorConstants.WHITE); // Establecer el color de las letras como blanco
                    // Agregar el Paragraph al Div
                    div.Add(paragraph);
                    containerTable.AddCell(div);



                    Div div2 = new Div();                   
                    div2.SetPaddingBottom(10f);
                    string tipo_reporte = (datos.tipo_antecedente == 1) ? "Reporte de Sanciones Vigentes Y Rehabilitadas" : "Reporte General de Expedientes";              
                    Paragraph paragraph2 = new Paragraph("Solicitud de " + tipo_reporte +"            N° Trámite Documentario: "+datos.rs_registro_mp).SetFontSize(fontzise);                
                    div2.Add(paragraph2);
                    containerTable.AddCell(div2);



                    Div div3 = new Div();
                    div3.SetBackgroundColor(ColorConstants.BLACK); 
                    Paragraph paragraph3 = new Paragraph("II. AUTORIDAD A QUIEN SE DIRIGE").SetFontSize(fontzise);
                    paragraph3.SetFontColor(ColorConstants.WHITE);
                    div3.Add(paragraph3);
                    containerTable.AddCell(div3);



                    Div div4 = new Div();                   
                    div4.SetPaddingBottom(10f);
                    Paragraph paragraph4 = new Paragraph("Jefe de la Autoridad Nacional de Control").SetFontSize(fontzise);               
                    div4.Add(paragraph4);
                    containerTable.AddCell(div4);




                    Div div5 = new Div();
                    div5.SetBackgroundColor(ColorConstants.BLACK);            
                    Paragraph paragraph5 = new Paragraph("III. DATOS DEL SOLICITANTE").SetFontSize(fontzise);
                    paragraph5.SetFontColor(ColorConstants.WHITE);
                    div5.Add(paragraph5);
                    containerTable.AddCell(div5);

                    //REALIZAR TABLA DE DATOS
                    // Crear un título
                    Paragraph title = new Paragraph("Persona Natural");
                    title.SetFontSize(fontzise).SetFont(font_bold);

                    // Agregar el título al documento
                    containerTable.AddCell(title);

                    // Crear una tabla con 1 fila y 6 columnas
                    Table table_datos = new Table(UnitValue.CreatePercentArray(new float[] { 16.67f, 16.67f, 16.67f, 16.67f, 16.67f, 16.67f }));
                    table_datos.SetWidth(UnitValue.CreatePercentValue(100f));
                    table_datos.SetBorder(Border.NO_BORDER);

                    string[] Persona_Natural = new string[]
{
                        "Apellido Paterno:", datos.ap_paterno,
                        "Apellido Materno:",  datos.ap_materno,
                        "Nombres:", datos.nombres

                    };

                    // Agregar celdas a la tabla
                    for (int i = 0; i < Persona_Natural.Length; i++)
                    {
                        Cell cell = new Cell();
                        cell.SetBorder(Border.NO_BORDER);
                        cell.SetTextAlignment(TextAlignment.CENTER);
                        if (i % 2 == 0) { cell.SetBorder(Border.NO_BORDER); } else { cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f)); }
                        // Agregar texto de ejemplo a la celda
                        Paragraph cellContent = new Paragraph(Persona_Natural[i]);
                        cellContent.SetTextAlignment(TextAlignment.CENTER); // Alinear el texto al centro
                        cellContent.SetFontSize(fontzise);
                        cell.Add(cellContent);
                        // Agregar la celda a la tabla
                        table_datos.AddCell(cell);
                    }
                    containerTable.AddCell(table_datos);


                    Paragraph title2 = new Paragraph("Persona Juridica");
                    title.SetFontSize(fontzise).SetFont(font_bold);

                 
                    containerTable.AddCell(title);
               
                    Table table_datos2 = new Table(UnitValue.CreatePercentArray(new float[] { 16.67f, 83.35f }));
                    table_datos2.SetWidth(UnitValue.CreatePercentValue(100f));
                    table_datos2.SetBorder(Border.NO_BORDER);

                    string[] Persona_Juridica = new string[]{
                        "Razon Social:","Poder Judicial"
                    };

                 
                    for (int i = 0; i < Persona_Juridica.Length; i++)
                    {
                        Cell cell = new Cell();
                        if (i % 2 == 0){cell.SetBorder(Border.NO_BORDER);}else{cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f));}
                        Paragraph cellContent = new Paragraph(Persona_Juridica[i]);
                        cellContent.SetTextAlignment(TextAlignment.CENTER);
                        cellContent.SetFontSize(fontzise);
                        cell.Add(cellContent);
                        table_datos2.AddCell(cell);
                    }
                    containerTable.AddCell(table_datos2);


                    Paragraph title3 = new Paragraph("Tipo y Número de Documento");
                    title3.SetFontSize(fontzise).SetFont(font_bold);
                    containerTable.AddCell(title3);
                    Table table_datos3 = new Table(UnitValue.CreatePercentArray(new float[] { 16.67f, 16.67f, 16.67f, 16.67f, 16.67f, 16.67f }));
                    table_datos3.SetWidth(UnitValue.CreatePercentValue(100f));
                    table_datos3.SetBorder(Border.NO_BORDER);

                    string[] Tipo_Documeto = new string[]{
                        "N° de DNI:",datos.num_documento,
                        "N° de RUC:"," ",
                        "C. Extranjería:"," "
                    };


                    for (int i = 0; i < Tipo_Documeto.Length; i++)
                    {
                        Cell cell = new Cell();
                        if (i % 2 == 0) { cell.SetBorder(Border.NO_BORDER); } else { cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f)); }
                        Paragraph cellContent = new Paragraph(Tipo_Documeto[i]);
                        cellContent.SetTextAlignment(TextAlignment.CENTER);
                        cellContent.SetFontSize(fontzise);
                        cell.Add(cellContent);
                        table_datos3.AddCell(cell);
                    }
                    containerTable.AddCell(table_datos3);


                    Div div6 = new Div();
                    div6.SetBackgroundColor(ColorConstants.BLACK);
                    Paragraph paragraph6 = new Paragraph("IV. DIRECCION").SetFontSize(fontzise);
                    paragraph6.SetFontColor(ColorConstants.WHITE);
                    div6.Add(paragraph6);
                    containerTable.AddCell(div6);


                   
                    Table table_datos4 = new Table(UnitValue.CreatePercentArray(new float[] { 25f, 25f, 25f, 25f }));
                    table_datos4.SetWidth(UnitValue.CreatePercentValue(100f));
                    table_datos4.SetBorder(Border.NO_BORDER);

                    string[] correo_electronico = new string[]{
                        "Correo Electrónico 1:",datos.correo,
                        "Correo Electrónico 2:"," ",
                       

                    };


                    for (int i = 0; i < correo_electronico.Length; i++)
                    {
                        Cell cell = new Cell();
                        if (i % 2 == 0) { cell.SetBorder(Border.NO_BORDER); } else { cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f)); }
                        Paragraph cellContent = new Paragraph(correo_electronico[i]);
                        cellContent.SetTextAlignment(TextAlignment.CENTER);
                        cellContent.SetFontSize(fontzise);
                        cell.Add(cellContent);
                        table_datos4.AddCell(cell);
                    }
                    containerTable.AddCell(table_datos4);


                    Table table_datos5 = new Table(UnitValue.CreatePercentArray(new float[] { 25f, 75f }));
                    table_datos5.SetWidth(UnitValue.CreatePercentValue(100f));
                    table_datos5.SetBorder(Border.NO_BORDER);

                    string[] otros_direccion = new string[]{
                     
                        "Tipo y Nombre de la Via:"," ",
                        "Nombre de la Via:"," ",
                        "N° de Inmueble:"," ",
                        "Tipo de Zona:"," ",
                        "Referencia:"," "
                    };


                    for (int i = 0; i < otros_direccion.Length; i++)
                    {
                        Cell cell = new Cell();
                        if (i % 2 == 0) { cell.SetBorder(Border.NO_BORDER); } else { cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f)); }
                        Paragraph cellContent = new Paragraph(otros_direccion[i]);
                        cellContent.SetTextAlignment(TextAlignment.LEFT);
                        cellContent.SetFontSize(fontzise);
                        cell.Add(cellContent);
                        table_datos5.AddCell(cell);
                    }
                    containerTable.AddCell(table_datos5);



                    Table table_datos6 = new Table(UnitValue.CreatePercentArray(new float[] { 16.67f, 16.67f, 16.67f, 16.67f, 16.67f, 16.67f }));
                    table_datos6.SetWidth(UnitValue.CreatePercentValue(100f));
                    table_datos6.SetBorder(Border.NO_BORDER);

                    string[] ubigeo = new string[]{

                        "Distrito:",datos.des_distrito,
                        "Provincia:",datos.des_provincia,
                        "Departamento:",datos.des_departamento
                      
                    };


                    for (int i = 0; i < ubigeo.Length; i++)
                    {
                        Cell cell = new Cell();
                        if (i % 2 == 0) { cell.SetBorder(Border.NO_BORDER); } else { cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f)); }
                        Paragraph cellContent = new Paragraph(ubigeo[i]);
                        cellContent.SetTextAlignment(TextAlignment.LEFT);
                        cellContent.SetFontSize(fontzise);
                        cell.Add(cellContent);
                        table_datos6.AddCell(cell);
                    }
                    containerTable.AddCell(table_datos6);

                   





                    Table table_datos7 = new Table(UnitValue.CreatePercentArray(new float[] { 25f, 25f, 25f, 25f }));
                    table_datos7.SetWidth(UnitValue.CreatePercentValue(100f));
                    table_datos7.SetBorder(Border.NO_BORDER);

                    string[] telefonos = new string[]{
                        "Telefono Fijo:","",
                        "Celular:",datos.nro_celular,


                    };


                    for (int i = 0; i < telefonos.Length; i++)
                    {
                        Cell cell = new Cell();
                        if (i % 2 == 0) { cell.SetBorder(Border.NO_BORDER); } else { cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f)); }
                        Paragraph cellContent = new Paragraph(telefonos[i]);
                        cellContent.SetTextAlignment(TextAlignment.CENTER);
                        cellContent.SetFontSize(fontzise);
                        cell.Add(cellContent);
                        table_datos7.AddCell(cell);
                    }
                    containerTable.AddCell(table_datos7);

                    Div div7 = new Div();
                    div7.SetBackgroundColor(ColorConstants.BLACK);
                    Paragraph paragraph7 = new Paragraph("V. BREVE SUSTENTACION DEL PEDIDO (Resumen)").SetFontSize(fontzise);
                    paragraph7.SetFontColor(ColorConstants.WHITE);
                    div7.Add(paragraph7);
                    containerTable.AddCell(div7);

                    Div div8 = new Div();
                    div8.SetPaddingBottom(10f);
                    Paragraph paragraph8 = new Paragraph("Solicito a su honorable despacho tenga a bien disponer a quien corresponda se me remita al correo "+datos.correo+" el record de "+ tipo_reporte+" del suscrito. Agradeciendo de antemano su atencion a mi solicitud.").SetFontSize(fontzise);
                    div8.Add(paragraph8);
                    containerTable.AddCell(div8);

                    Div div9 = new Div();
                    div9.SetBackgroundColor(ColorConstants.BLACK);
                    Paragraph paragraph9 = new Paragraph("VI. ANEXOS (En orden correlativo) Folios:       En letras  [               ]     En Números   [               ] ").SetFontSize(fontzise);
                    paragraph9.SetFontColor(ColorConstants.WHITE);
                    div9.Add(paragraph9);
                    containerTable.AddCell(div9);

                    Div div10 = new Div();
                    div10.SetHeight(50f);                    
                    containerTable.AddCell(div10);

                    Table table_datos8 = new Table(UnitValue.CreatePercentArray(new float[] { 50f, 50f }));
                    table_datos8.SetWidth(UnitValue.CreatePercentValue(100f));
                    table_datos8.SetBorder(Border.NO_BORDER);

                    DateTime date = DateTime.Now;
                    Paragraph paragraph10 = new Paragraph("DECLARO que la informacion presentada en este formulario tiene caracter de DECLARACIÓN JURADA.\n\n\n");
                    paragraph10.Add("Lugar y Fecha:   Lima " + date.ToString("dd") + " de " + date.ToString("MMMM") + " del " + date.ToString("yyyy"));
                    paragraph10.SetFontSize(fontzise);

                    Cell middleTableCell1 = new Cell().Add(paragraph10).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT);
                    middleTableCell1.SetWidth(UnitValue.CreatePercentValue(80f));

                    table_datos8.AddCell(middleTableCell1);


                    // Crear tabla para las filas del medio (segunda columna)
                    Table middleTable = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();                

                    //ImageData _firmaImage = ImageDataFactory.Create(datos.firma_user);
                    //iText.Layout.Element.Image fima_img = new iText.Layout.Element.Image(_firmaImage);
                    //fima_img.ScaleToFit(90f, 90f);
                    //fima_img.SetPaddingTop(2f);
                    //fima_img.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    //Cell fimaImgCell = new Cell().Add(fima_img).SetBorder(Border.NO_BORDER);

                    //middleTable.AddCell(fimaImgCell);
                    // Agregar las filas a la tabla del medio
                    middleTable.AddCell(new Cell().Add(new Paragraph("-------------------------------------------- \n Firma del Usuario" )
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(6))
                        .SetBorder(Border.NO_BORDER));

                    Cell middleTableCell = new Cell().Add(middleTable).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER);
                    middleTableCell.SetWidth(UnitValue.CreatePercentValue(100f));

                    table_datos8.AddCell(middleTableCell);

                    containerTable.AddCell(table_datos8);

                    document.Add(containerTable);

                    document.Close();

                    byte[] FormularioPDF = memoryStream.ToArray();

                    rptFormulario.ArchivosPDF = FormularioPDF;
                    rptFormulario.codigo=0;
                    rptFormulario.valor = "OK";




            }
            catch (Exception ex)
            {
                rptFormulario.ArchivosPDF = null;
                rptFormulario.codigo = -1;
                rptFormulario.valor =" ERROR: "+ ex.Message;
            }

            return rptFormulario;
        }


    }
}
