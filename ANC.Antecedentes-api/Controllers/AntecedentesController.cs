
using ANC.Comun;
using ANC.Entidades;
using ANC.LogicaNegocio;
using ANC.WebApi.Servicio_Email;
using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Web;


namespace ANC.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AntecedentesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;

        private readonly JwtManager _jwtManager;

        private static string ruta_certificado = "";
        private static string password_certificado = ""; 

        GenericoLN comuln = new GenericoLN();
        AntecedentesLN antecedentesln = new AntecedentesLN();
        Funciones _funciones = new Funciones();
        ServicioEmail emailPJ;
        
        public AntecedentesController(IConfiguration configuration, JwtManager jwtManager, IHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
            _jwtManager = jwtManager;
            emailPJ = new ServicioEmail(_configuration);
            ruta_certificado = _configuration.GetValue<string>("CETIFICADO_FIRMA_DIGITAL");
            password_certificado = _configuration.GetValue<string>("CETIFICADO_FIRMA_DIGITAL_PASSWORD");
        }



        #region API genera token dinamicamente
        private bool VerifyJWT()
        {
            string headerToken = Request.Headers["Authorization"];
            var principal = _jwtManager.ValidateToken(headerToken);

            // Verificar si el token es válido
            if (principal == null)
            {
                return false;
            }

            return true;
        }
        private bool IsTokenValid()
        {

            string headerToken = Request.Headers["Authorization"];

            string configToken = _configuration.GetValue<string>("Token:key");

            return headerToken == configToken;


        }
        [Route("Autenticacion")]
        [HttpGet]
        public ActionResult<JWTBE> Autenticacion()
        {
            if (!IsTokenValid())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }

            JWTBE jwt = new JWTBE();
            jwt._token = _jwtManager.GenerateToken();
            return jwt;
        }
        #endregion
        //[Route("AntecedentesDatosPersonales")]
        //[HttpGet]
        //public ActionResult<ExpedienteAntecedenteBE> AntecedentesDatosPersonales(string codigo_persona)
        //{
        //    if (!VerifyJWT())
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //        return Unauthorized("Token no válido");
        //    }


        //    return antecedentesln.AntecedentesDatosPersonales(codigo_persona);
        //}

        [Route("AntecedentesSanciones")]
        [HttpGet]
        public ActionResult<List<ExpedienteSancionesBE>> AntecedentesSanciones(string codigo_persona, int sub_tipo_antecedente)
        {
            if (!VerifyJWT())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }


            return antecedentesln.AntecedentesSanciones(codigo_persona, sub_tipo_antecedente);
        }
        [Route("AntecedentesExpedientes")]
        [HttpGet]
        public ActionResult<List<ExpedienteSancionesBE>> AntecedentesExpedientes(string codigo_persona, int sub_tipo_antecedente)
        {
            if (!VerifyJWT())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }


            return antecedentesln.AntecedentesExpedientes(codigo_persona, sub_tipo_antecedente);
        }



        //[Route("RecordSancionesTotal")]
        //[HttpGet]
        //public ActionResult<RecordSancionesBE> RecordSancionesTotal(string codigo_persona, int sub_tipo_antecedente)
        //{
        //    if (!VerifyJWT())
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //        return Unauthorized("Token no válido");
        //    }
        //    return antecedentesln.RecordSancionesTotal(codigo_persona, sub_tipo_antecedente);
        //}
        //[Route("RecordExpedientesTotal")]
        //[HttpGet]
        //public ActionResult<RecordExpedientesBE> RecordExpedientesTotal(string codigo_persona, int sub_tipo_antecedente)
        //{
        //    if (!VerifyJWT())
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //        return Unauthorized("Token no válido");
        //    }
        //    return antecedentesln.RecordExpedientesTotal(codigo_persona, sub_tipo_antecedente);
        //}


        //[Route("AntecedentesPiePaginaSanciones")]
        //[HttpGet]
        //public ActionResult<List<PiePagAntecedenteSancionesBE>> AntecedentesPiePaginaSanciones(string codigo_persona, int sub_tipo_antecedente)
        //{
        //    if (!VerifyJWT())
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //        return Unauthorized("Token no válido");
        //    }
        //    return antecedentesln.AntecedentesSancionesPiePagina(codigo_persona, sub_tipo_antecedente);
        //}
        //[Route("AntecedentesPiePaginaExpediente")]
        //[HttpGet]
        //public ActionResult<List<PiePagAntecedenteSancionesBE>> AntecedentesPiePaginaExpediente(string codigo_persona, int sub_tipo_antecedente)
        //{
        //    if (!VerifyJWT())
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //        return Unauthorized("Token no válido");
        //    }
        //    return antecedentesln.AntecedentesPiePaginaExpediente(codigo_persona, sub_tipo_antecedente);
        //}



        [Route("LoginVericar")]
        [HttpPost]
        public ActionResult<RptVerificacionBE> LoginVericar(AntecedentesBE datos)
        {
            if (!VerifyJWT())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }

            return antecedentesln.LoginVericar(datos);
        }
        [Route("VericarDoc")]
        [HttpGet]
        public ActionResult<RptVerificarDocBE> VericarDoc(string cod_usuario)
        {
            if (!VerifyJWT())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }

            return antecedentesln.VericarDoc(cod_usuario);
        }
        //[Route("ActualizarVericaDoc")]
        //[HttpPost]
        //public ActionResult<RptVerificacionBE> ActualizarVericaDoc(ActualizarVerificaDocBE datos)
        //{
        //    if (!VerifyJWT())
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //        return Unauthorized("Token no válido");
        //    }

        //    return antecedentesln.ActualizarVericaDoc(datos);
        //}


        [Route("AuditoriaAntecedentes")]
        [HttpPost]
        public async Task<ActionResult<RptVerificacionBE>> AuditoriaAntecedentes(AuditoriaAntecedentesBE datos)
        {

            if (!VerifyJWT())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }
            return await antecedentesln.AuditoriaAntecedentes(datos);


        }


        [Route("MantMovimiento")]
        [HttpPost]
        public async Task<ActionResult<RptPGBE>> MantMovimiento(AuditoriaAntecedentesBE datos)
        {

            if (!VerifyJWT())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }
            return await antecedentesln.MantMovimiento(datos);
        }

        [Route("VericarAntecedentes")]
        [HttpPost]
        public async Task<ActionResult<RptVerificacionBE>> VericarAntecedentes(VerificaAntecedentesBE datos)
        {
            if (!VerifyJWT())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }
            return await antecedentesln.VericarAntecedentes(datos);
        }

        [Route("GenerarCertificadoPDF")]
        [HttpPost]
        public async Task<ActionResult<RespuestaArchivoBE>> GenerarCertificadoPDF(SesionAntecedentesBE datos)
        {
            RespuestaArchivoBE rpta=new RespuestaArchivoBE();
            if (!VerifyJWT())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }


            var Fondo_certificado = Path.Combine(_env.ContentRootPath, "images/fondo_certificado_anc.jpg");
            var imagen_Firma = Path.Combine(_env.ContentRootPath, "images/imagen_firma_r.png");

            if (string.IsNullOrEmpty(ruta_certificado))
            {

                rpta.codigo = -1;
                rpta.valor = "Especifique la ruta del certificado ";
                return rpta;
            }
            else
            {
                string directorioPadre = System.IO.Path.GetDirectoryName(ruta_certificado);


                if (!string.IsNullOrEmpty(directorioPadre))
                {
                    // Verificar si el directorio padre existe
                    if (!Directory.Exists(directorioPadre))
                    {
                        try
                        {
                            // Intentar crear el directorio padre
                            Directory.CreateDirectory(directorioPadre);

                        }
                        catch (Exception ex)
                        {
                            rpta.codigo = -1;
                            rpta.valor = "No se pudo crear el Directorio Especificado.";
                            return rpta;
                        }
                    }
                }
                else
                {
                    rpta.codigo = -1;
                    rpta.valor = "La ruta proporcionada no es válida.";
                    return rpta;
                }

            }


            if (!System.IO.File.Exists(ruta_certificado))
            {
                rpta.codigo = -1;
                rpta.valor = "No se encotro Archivo pfx en la ruta proporcionada";

                return rpta;
            }


            if (string.IsNullOrEmpty(password_certificado))
            {

                rpta.codigo = -1;
                rpta.valor = "Especifique la clave del certificado ";
                return rpta;
            }

            if (!System.IO.File.Exists(Fondo_certificado))
            {
                rpta.codigo = -1;
                rpta.valor = "No se encotro fondo certifcado en la ruta proporcionada";

                return rpta;
            }
            if (!System.IO.File.Exists(imagen_Firma))
            {
                rpta.codigo = -1;
                rpta.valor = "No se encotro imagen de la Firma Digital en la ruta proporcionada";

                return rpta;
            }

            datos.fondo_certificado = Fondo_certificado;
            datos.ruta_certificado = ruta_certificado;
            datos.password_certificado = password_certificado;

            datos.ruta_imagen_firma = imagen_Firma;
            
            return await antecedentesln.GenerarCertificadoPDF(datos);

        }

        //[Route("GetCredencialesFTP")]
        //[HttpGet]
        //public async Task<ActionResult<CredencialesFTPBE>> GetCredencialesFTP(int coddistri)
        //{

        //    if (!VerifyJWT())
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //        return Unauthorized("Token no válido");
        //    }

        //    return await comuln.GetCredencialesFTP(coddistri);

        //}


        [Route("EnviarAcuseAntecedente")]
        [HttpPost]
        public async Task<ActionResult<RptPGBE>> EnviarAcuseAntecedente(AcuseCorreoBE datos)
        {
            RptPGBE rpt_api = new RptPGBE();
            if (!IsTokenValid())
            {
                rpt_api.rn_codigo = -1;
                rpt_api.rs_valor = "Token no válido";
                return rpt_api;

            }


            RptPGBE rpt_acuse_prod = new RptPGBE();

            RptBE rpt_webService = new RptBE();

            try
            {

                if (string.IsNullOrEmpty(datos.correo_destino))
                {
                    rpt_api.rn_codigo = -1;
                    rpt_api.rs_valor = "Ingrese Correo Destino";
                    return rpt_api;
                }
                // Separar los correos electrónicos y los nombres utilizando el punto y coma como delimitador
                string[] correos = datos.correo_destino.Split(';');
                string[] nombresArray = datos.nombres.Split(';');


                // Verificar que la cantidad de correos y nombres sea la misma
                if (correos.Length != nombresArray.Length)
                {
                    rpt_api.rn_codigo = -1;
                    rpt_api.rs_valor = "Cantidad de nombres y correo no conincide.";
                    return rpt_api;
                }
                if (datos.rs_valor!="TD_WEB")
                {
                    if (datos.ruta_archivo == null || !datos.ruta_archivo.Any(ruta => !string.IsNullOrEmpty(ruta)))
                    {
                        rpt_api.rn_codigo = -1;
                        rpt_api.rs_valor = "No se Proporcionaron rutas de archivo válidas";
                        return rpt_api;
                    }
                }
                
                string Codigos_registro = "";
                if (datos.regitro_antec != null && datos.regitro_antec.Length > 0)
                {
                    for (int j = 0; j < datos.regitro_antec.Length; j++)
                    {
                        string sigla = (datos.rs_valor == "TD_WEB" || datos.rs_valor == "RTD_WEB") ?"T":"C";
                        Codigos_registro += "(" + sigla+ datos.regitro_antec[j] + ")";
                        if (j != datos.regitro_antec.Length - 1)
                        {
                            Codigos_registro += ",";
                        }
                    }
                }

                string[] ruta_pdf = datos.ruta_archivo;


                string[] nombreDocumento = new string[ruta_pdf.Length];
                string[] extensionDocumento = new string[ruta_pdf.Length];
                string[] binarioDocumento = new string[ruta_pdf.Length];
              
                    CredencialesFTPBE credeniales = await comuln.GetCredencialesFTP(datos.cod_distrito);

                    FtpClient FTP = null;

                    
                    for (int i = 0; i < ruta_pdf.Length; i++)
                    {
                        string pdf_ruta = ruta_pdf[i];

                        if (pdf_ruta.StartsWith("ZAV-"))
                        {
                            string[] palabras = pdf_ruta.Split('-');

                            FTP = new FtpClient("ftp://" + credeniales.server_ftp3, credeniales.user_ftp3, credeniales.pass_ftp3);

                            pdf_ruta = palabras[1];
                        }
                        else if (pdf_ruta.StartsWith("ANC-"))
                        {
                            string[] palabras = pdf_ruta.Split('-');

                            FTP = new FtpClient("ftp://" + credeniales.server_ftp, credeniales.user_ftp, credeniales.pass_ftp);

                            pdf_ruta = palabras[1];
                        }
                        else
                        {
                            FTP = new FtpClient("ftp://" + credeniales.server_ftp3, credeniales.user_ftp3, credeniales.pass_ftp3);

                        }




                        byte[] pdfBytes = FTP.downloadBytes(pdf_ruta);

                        if (pdfBytes[0] == 0 || pdfBytes == null || pdfBytes.Length == 0)
                        {
                            rpt_api.rn_codigo = -1;
                            rpt_api.rs_valor = "NO SE ENCONTRO PDF CON LA RUTA PROPORCIONADA: " + pdf_ruta;


                            return rpt_api;
                        }

                    string nombreDoc = "";
                    switch (datos.tipo_documentos[i])
                    {
                        case "CD":
                            nombreDoc = "CERTIFICADO DE ANTECEDENTES DISCIPLINARIOS";
                            break;

                        case "CE":

                            nombreDoc = "CERTIFICADO DE ANTECEDENTES DE EXPEDIENTES";
                            break;

                        case "CO":
                            nombreDoc = "OFICIO";

                            break;
                        case "CC":
                            nombreDoc = "CARGO DE INGRESO ADMINISTRATIVO";

                            break;
                        case "CF":
                            nombreDoc = "FORMULARIO UNICO DE TRÁMITES ADMINISTRATIVOS";

                            break;
                        default:
                            nombreDoc= Path.GetFileNameWithoutExtension(pdf_ruta);
                            break;
                    }


                    string pdfEnBinario = Convert.ToBase64String(pdfBytes);
                        nombreDocumento[i] = nombreDoc;
                        extensionDocumento[i] = "pdf";
                        binarioDocumento[i] = pdfEnBinario;
                    }
                
                bool error = false;
                string error_correo = "";
                // Iterar sobre los correos electrónicos y nombres
                for (int i = 0; i < correos.Length; i++)
                {
                    string correo_envio = correos[i].Trim();
                    string nombre_envio = nombresArray[i].Trim();




                    datos.correo_destino = correo_envio;
                    datos.nombres = nombre_envio;
                    datos.accion = "I";
                    datos.adt_fec_envio = new DateTime();
                    datos.estado_envio = "1";
                    rpt_acuse_prod = await antecedentesln.RegistrarAcuseAntecedentes(datos);
                    // obtenemos el id de registro


                    if (rpt_acuse_prod.rs_valor != "OK")
                    {
                        rpt_api.rn_codigo = -1;
                        rpt_api.rs_valor = rpt_acuse_prod.rs_valor;
                        return rpt_api;
                    }

                    if (!correo_envio.Contains("@"))
                    {
                        // Si el correo no contiene "@", saltar al siguiente correo
                        continue;
                    }

                    string cocat_nom_ruta = "";
                    for (int j = 0; j < datos.ruta_archivo.Length; j++)
                    {
                        cocat_nom_ruta += datos.ruta_archivo[j];
                        if (j != datos.ruta_archivo.Length - 1)
                        {
                            cocat_nom_ruta += "&&";
                        }
                    }
                    string cocat_tipo_doc = "";
                    for (int j = 0; j < datos.tipo_documentos.Length; j++)
                    {
                        cocat_tipo_doc += datos.tipo_documentos[j];
                        if (j != datos.tipo_documentos.Length - 1)
                        {
                            cocat_tipo_doc += "&&";
                        }
                    }
                    string data = rpt_acuse_prod.rn_codigo + "~" + cocat_nom_ruta + "~" + nombre_envio + "~" + correo_envio + "~" + datos.cod_distrito+"~"+ cocat_tipo_doc;
                    string clave = "envio_correo_ant";

                    string correoCifrado = _funciones.cifradoAES(data, clave);
                    if (correoCifrado == "ERROR")
                    {
                        rpt_api.rn_codigo = -1;
                        rpt_api.rs_valor = "Surgio un error al cifrar los datos";
                        return rpt_api;
                    }
                    string anc_URL = _configuration.GetValue<string>("URL_DOMINIO_ANTECEDENTES");

                    string url = anc_URL + "Antecedentes/ReporteAntecedentes?send=" + correoCifrado;

                    WSEmailBE wsEmail = new WSEmailBE();
                    wsEmail.destinatarioEmail = new string[1];
                    wsEmail.destinatarioTipo = new string[1];

                    wsEmail.destinatarioEmail[0] = correo_envio;
                    wsEmail.destinatarioTipo[0] = "TO";

                    wsEmail.nombreCompleto = nombre_envio;
                    wsEmail.ipPc = datos.ipPc;
                    wsEmail.pcNameipPc = datos.pcNameipPc;
                    wsEmail.usuarioSis = datos.usuarioSis;
                    wsEmail.usuarioRed = datos.usuarioRed;
                    wsEmail.nombreSo = datos.nombreSo;
                    wsEmail.titulo = datos.asunto;
                    wsEmail.motivoEmail = "Envio acuse Certificado Digital Electrónico";

                    if (Codigos_registro != "")
                    {
                        
                        if (datos.rs_valor == "TC_WEB")
                        {
                            //Tramite certificado WEB
                            wsEmail.TextoHtml = "<p style='text-align:justify;font-size: 13px;'><b> Se le informa que el CERTIFICADO DE ANTECEDENTES con N° de registro: <strong>" + Codigos_registro + "</strong> correspondiente a los datos solicitados ha sido remitido a su correo electrónico. <br> Para acceder al archivo generado, por favor haga clic en el siguiente enlace: <a href=" + url + " target='_blank'>Archivo Principal</a>.<br><br>  Atentamente,</p></b>";

                           
                        }
                        else if(datos.rs_valor == "TD_WEB")
                        {
                            //Tramite Documentario WEB
                            wsEmail.TextoHtml = "<p style='text-align:justify;font-size: 13px;'><b> Se le informa que se generó su registro de TRÁMITE DOCUMENTARIO con N°: <strong>" + Codigos_registro + "</strong> para su atención  , <br>  La respuesta será enviada a su correo electrónico a la brevedad posible.<br> Para ver los archivos de solicitud en el trámite documentario, por favor haga clic en el siguiente enlace: <a href=" + url + " target='_blank'> Ver Documentos </a>.<br><br>  Atentamente,</p></b>";

                        }
                        else if (datos.rs_valor == "RTD_WEB")
                        {
                            //Tramite Documentario WEB
                            wsEmail.TextoHtml = "<p style='text-align:justify;font-size: 13px;'><b> Se le informa que los registros de TRÁMITE DOCUMENTARIO con N°: <strong>" + Codigos_registro + "</strong> correspondiente a los datos solicitados ha sido remitido a su correo electrónico.  <br>  Para acceder al archivo generado, por favor haga clic en el siguiente enlace: <a href=" + url + " target='_blank'> Ver Documentos </a>.<br><br>  Atentamente,</p></b>";

                        }
                        else
                        {
                            //SISANC 
                            wsEmail.TextoHtml = "<p style='text-align:justify;font-size: 13px;'><b> Se le informa que el CERTIFICADO DE ANTECEDENTES con N° de registro: <strong>" + Codigos_registro + "</strong>  y registro de TRÁMITE DOCUMENTARIO  N°: T" + datos.registro_td + " correspondiente a los datos solicitados ha sido remitido a su correo electrónico. <br> Para acceder al archivo generado, por favor haga clic en el siguiente enlace: <a href=" + url + " target='_blank'>Archivo Principal</a>.<br><br>  Atentamente,</p></b>";

                        }
                    }
                    else
                    {
                        wsEmail.TextoHtml = "<p style='text-align:justify;font-size: 13px;'><b> Se le informa que se generó su registro de trámite documentario para su atención  , <br> Para acceder al archivo generado, por favor haga clic en el siguiente enlace: <a href=" + url + " target='_blank'>Archivo Principal</a>.<br><br>  Atentamente,</p></b>";


                    }


                   

                    rpt_webService = await emailPJ.EnviarCorreoWS(wsEmail);

                    if (rpt_webService.codigo == -1)
                    {
                        error = true;
                        error_correo += correo_envio;
                    }

                    if (datos.correo_coo != null)
                    {


                        string[] correos_coo = datos.correo_coo.Split(';');

                        for (int k = 0; k < correos_coo.Length; k++)
                        {

                            wsEmail.destinatarioEmail = new string[1];
                            wsEmail.destinatarioTipo = new string[1];

                            wsEmail.destinatarioEmail[0] = correos_coo[k].Trim(); ;
                            wsEmail.destinatarioTipo[0] = "TO";

                            wsEmail.titulo = datos.asunto;
                            wsEmail.motivoEmail = "Copia Envio acuse Certificado digital electrónico";
                            if (Codigos_registro != "")
                            {
                                wsEmail.TextoHtml = "<p style='text-align:justify;font-size: 13px;'><b>Por este se pone en conocimiento el detalle del envio de Certificado de Antecedendetes con registro Número:" + Codigos_registro + ".<br><br>La cual se envió a la siguiente dirección proporcionada a continuación:<br> DESTINATARIO: " + correo_envio + "<br><br> Además, adjunto a este correo encontrarás el documento que se envió junto con el correo electrónico mencionado anteriormente.<br><br> Saludos cordiales,</b></p>";
                            }
                            else
                            {
                                wsEmail.TextoHtml = "<p style='text-align:justify;font-size: 13px;'><b> Se le informa que se generó el registro de trámite documentario, <br><br>La cual se envió a la siguiente dirección proporcionada a continuación:<br> DESTINATARIO: " + correo_envio + "<br><br> Además, adjunto a este correo encontrarás el documento que se envió junto con el correo electrónico mencionado anteriormente.<br><br> Saludos cordiales,</b></p>";


                            }
                            if (datos.rs_valor != "TD_WEB")
                            {
                                wsEmail.nombreDocumento = nombreDocumento;
                                wsEmail.extensionDocumento = extensionDocumento;
                                wsEmail.binarioDocumento = binarioDocumento;
                            }
                            rpt_webService = await emailPJ.EnviarCorreoWS(wsEmail);


                        }

                    }

                    if (rpt_webService.codigo == 0)
                    {
                        datos.cod_notif_antec = rpt_acuse_prod.rn_codigo;
                        datos.accion = "E";
                        rpt_acuse_prod = await antecedentesln.RegistrarAcuseAntecedentes(datos);
                        if (rpt_acuse_prod.rs_valor != "OK")
                        {
                            rpt_api.rn_codigo = -1;
                            rpt_api.rs_valor = rpt_acuse_prod.rs_valor;
                            return rpt_api;
                        }
                    }
                }

                if (error)
                {
                    rpt_api.rn_codigo = -1;
                    rpt_api.rs_valor = "No se logro Notificar a: " + error_correo;
                    return rpt_api;
                }




                rpt_api.rs_valor = "El correo fué enviado correctamente.";

            }
            catch (Exception ex)
            {
                rpt_api.rn_codigo = -1;
                rpt_api.rs_valor = "Surgió un error: " + ex;

            }
            return rpt_api;
        }



        [Route("RegistrarAcuseReciboAntecedentes")]
        [HttpPost]
        public async Task<RptPGBE> RegistrarAcuseReciboAntecedentes(AcuseCorreoBE datos)
        {

            RptPGBE rpt = new RptPGBE();
            if (!IsTokenValid())
            {
                rpt.rn_codigo = -1;
                rpt.rs_valor = "Token no válido";
                return rpt;

            }
            return await antecedentesln.RegistrarAcuseAntecedentes(datos);
        }

        [Route("EnviarConfirmacionAcuse")]
        [HttpPost]
        public async Task<RptBE> EnviarConfirmacionAcuse(WSEmailBE data_correo)
        {
            RptBE rpt = new RptBE();
            if (!IsTokenValid())
            {
                rpt.codigo = -1;
                rpt.valor = "Token no válido";
                return rpt;

            }
            return await emailPJ.EnviarCorreoWS(data_correo);
        }

        [Route("EnviarCorreoWS")]
        [HttpPost]
        public async Task<RptBE> EnviarCorreoWS(WSEmailBE data_correo)
        {
            RptBE rpt = new RptBE();
            if (!IsTokenValid())
            {
                rpt.codigo = -1;
                rpt.valor = "Token no válido";
                return rpt;

            }
            return await emailPJ.EnviarCorreoWS(data_correo);
        }

        [Route("ConsultaAntecedentes")]
        [HttpPost]
        public async Task<ActionResult<List<RptConsutaAntecedenteBE>>> ConsultaAntecedentes(ConsultaAntecedenteBE datos)
        {

            if (!VerifyJWT())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }
            return await antecedentesln.ConsultaAntecedentes(datos);
        }
        [Route("RegistroTramiteDocumentario")]
        [HttpPost]
        public async Task<ActionResult<RptTramiteDocumentarioBE>> RegistroTramiteDocumentario(TramiteDocumentarioBE datos)
        {

            if (!VerifyJWT())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }

            datos.ruta_logo = Path.Combine(_env.ContentRootPath, "images/logo.png");
            datos.ruta_escudo = Path.Combine(_env.ContentRootPath, "images/rp.png");
            datos.ruta_logo_pj = Path.Combine(_env.ContentRootPath, "images/pj_logo.png");
            return await antecedentesln.RegistroTramiteDocumentario(datos);
        }

        [Route("RegistrarTDAntecedente")]
        [HttpPost]
        public async Task<ActionResult<RptPGBE>> RegistrarTDAntecedente(MantRegistroPGBE datos)
        {

            if (!VerifyJWT())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }
            return await antecedentesln.RegistrarTDAntecedente(datos);
        }

        [Route("VerificarTDAntecedente")]
        [HttpGet]
        public async Task<ActionResult<RptVerificacionBE>> VerificarTDAntecedente(string num_documento, int cod_tipo_repo)
        {

            if (!VerifyJWT())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }
            return await antecedentesln.VerificarTDAntecedente(num_documento, cod_tipo_repo);
        }

        [Route("VerificarIngreso")]
        [HttpPost]
        public async Task<ActionResult<RptPGBE>> VerificarIngreso(VerificaIngresoBE datos)
        {

            if (!VerifyJWT())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }
            return await antecedentesln.VerificarIngreso(datos);
        }

        [Route("ListarAnio")]
        [HttpGet]
        public async Task<ActionResult<List<ListAnioBE>>> ListarAnio()
        {

            if (!VerifyJWT())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized("Token no válido");
            }
            return await antecedentesln.ListarAnio();
        }


       
    }

}
