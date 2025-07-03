using ANC.Entidades;
using Microsoft.AspNetCore.Mvc;
using ANC.Comun;
using ANC.Comun.Log4net;
using System.Drawing.Imaging;
using ANC.Modelo;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;
using ANC.AntecedentesWeb.UtilesPC;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace ANC.AntecedentesWeb.Controllers
{
    public class ConsultaTramiteController : Controller
    {
        GenericoModel genericoModel = new GenericoModel();
        AntecedentesModel _antecedentesModel = new AntecedentesModel();
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly JwtManager _jwtManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ConsultaTramiteController(IConfiguration configuration, IWebHostEnvironment hostEnvironment, JwtManager jwtManager, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _jwtManager = jwtManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> ConsultaTramite()
        {

            List<PetTipoDocBE> lstTipoDoc = await genericoModel.ObtenerTipoDoc();
            ViewBag.lstTipoDoc = lstTipoDoc;
            List<ListAnioBE> lstAnio= await _antecedentesModel.ListarAnio();
            ViewBag.lstAnio = lstAnio;


            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ConsultarRegistro([FromBody] ConsultaAntecedenteBE datos)
        {

            SesionAntecedentesBE SessionBE = new SesionAntecedentesBE();
            var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();

            // Asigna el sistema operativo a tu objeto SessionBE
            SessionBE.sistema_operativo = Util.GetOperatingSystem(userAgent);

            Util util = new Util(_httpContextAccessor);
            SessionBE.direccion_ip = util.GetIPAddress();
            SessionBE.mac_address = Util.GetMACAddress();
            SessionBE.computer_name = Environment.MachineName;
            RptBE rpta = new RptBE();
            try
            {

                int longitudClaveBytes = 32;

                // Generar la clave aleatoria
                byte[] claveAleatoria = GenerarClaveAleatoria(longitudClaveBytes);

                // Convertir la clave aleatoria a una cadena hexadecimal para su fácil visualización
                string claveAleatoriaHex = BitConverter.ToString(claveAleatoria).Replace("-", "");

                HttpContext.Session.Remove("ssgenerico");

                HttpContext.Session.SetString("ssgenerico", claveAleatoriaHex);


                char tipo_consulta = datos.nro_registro[0]; // Accede al primer carácter de la cadena
          
                tipo_consulta = char.ToUpper(tipo_consulta); // Opcional: convertir a mayúscula

                datos.tipo_consulta = (tipo_consulta.ToString() == "C") ?1:2;
                datos.nro_registro = datos.nro_registro.Substring(1);

                if (datos.nro_registro == "000000")
                {
                    rpta.codigo = -1;
                    rpta.valor = "El numero de Trámite no puede ser 0";
                    return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
                }

                if (HttpContext.Session.GetString("CaptchaImageText").ToUpper() == datos.textCaptcha.ToUpper())
                {

                    paramsApiReniec reniec = new paramsApiReniec();
                    reniec.cod_distrito = 50;
                    reniec.modulo = "MOD_RENIEC_WEB";
                    reniec.datosConsulta = new RequestReniecModel
                    {
                        formatoRespuesta = "json",
                        consultante = "43719662",
                        motivo = "09998-20026/INVESTIGACION DEFINITIVA/ANCASH DE ORIGEN ANC",
                        personaConsultada = new PersonaConsultada
                        {
                            tipoConsulta = "2",
                            nroDocumentoIdentidad = datos.num_documento,

                        },
                        pagination = new Pagination
                        {
                            size = 30,
                            page = 0
                        },
                        auditoria = new AuditoriaReniecDedicada
                        {
                            usuario = "43719662",
                            nombrePc = SessionBE.computer_name,
                            direccionMac = SessionBE.mac_address,
                            numeroIp = SessionBE.direccion_ip

                        }
                    };
                    ReniecBE reniecBE = await genericoModel.ConsultarReniecDNI(reniec);



                    AntecedentesBE _datosAntecedentes = new AntecedentesBE();

                    _datosAntecedentes.num_documento = reniecBE.numdni;
                    _datosAntecedentes.ap_paterno = reniecBE.apaterno;
                    _datosAntecedentes.ap_materno = reniecBE.apaterno;
                    _datosAntecedentes.nombres = reniecBE.nombres;

                    RptVerificacionBE rptVerificacion = await _antecedentesModel.LoginVericar(_datosAntecedentes);

                    if (rptVerificacion.codigo != 0)
                    {

                        datos.cod_intper = 0;


                    }
                    else
                    {
                        datos.cod_intper = rptVerificacion.cod_intper;
                    }


                    SessionBE.cod_intper = datos.cod_intper;
                    datos.cod_tipantec = 0;

                    datos.cod_subtipantec = 0;

                    datos.cod_origantec = 0;
                    datos.fla_vigencia = 9999;


                    List<RptConsutaAntecedenteBE> rptConsultaAntecedente = await _antecedentesModel.ConsultaAntecedentes(datos);

                    if (rptConsultaAntecedente.Count > 0)
                    {
                        SessionBE.nro_registro = datos.nro_registro;
                        SessionBE.anio_td = datos.anio;
                        SessionBE.cod_intper = rptVerificacion.cod_intper;
                        SessionBE.num_documento = reniecBE.numdni;
                        SessionBE.cod_tipodoc = datos.cbTipoDocumento;

                        SessionBE.ap_paterno = reniecBE.apaterno;
                        SessionBE.ap_materno = reniecBE.amaterno;
                        SessionBE.nombres = reniecBE.nombres;

                        SessionBE.fec_emisiondoc = reniecBE.fecha_expedicion;
                        SessionBE.fec_nacimiento = reniecBE.fechanac;

                        SessionBE.coddepartamento = reniecBE.departamentonac;
                        SessionBE.codprovincia = reniecBE.provincianac;
                        SessionBE.coddistrito = reniecBE.distritonac;
                        SessionBE.telefono = "";


                        //SessionBE.correo = datos.txtEmail;
                        SessionBE.id_sesion = HttpContext.Session.GetString("ssgenerico").ToString();

                        SessionBE.cod_usuario = SessionBE.num_documento;


                        SessionBE.foto = reniecBE.foto;
                        SessionBE.cod_distrito_ftp = rptVerificacion.cod_distri;



                        //servira para saber si es registro o consulta
                        SessionBE.cod_tipmovantec = 2;
                        //agregar tipo de consulta

                        SessionBE.tipo_consulta = datos.tipo_consulta;

                        if (datos.tipo_consulta == 1)
                        {
                            SessionBE.cod_tipo_repo = rptConsultaAntecedente[0].rn_cod_tipantec;
                            SessionBE.des_tip_repo = rptConsultaAntecedente[0].rs_des_tipantec;
                            SessionBE.nro_registro = rptConsultaAntecedente[0].rs_nro_registro;
                            
                            SessionBE.anio_td = rptConsultaAntecedente[0].rs_anio;
                            SessionBE.fec_caducidad = rptConsultaAntecedente[0].rd_fec_vigencia.ToString("dd/MM/yyyy");
                            SessionBE.fec_expedido = rptConsultaAntecedente[0].rd_fec_generacion.ToString("dd/MM/yyyy");
                            SessionBE.nombre_doc = rptConsultaAntecedente[0].rs_nombre_doc;
                            SessionBE.ruta_certificado = rptConsultaAntecedente[0].rs_nombre_doc;
                            SessionBE.ruta_doc_ftp = new string[]
                              {
                                     rptConsultaAntecedente[0].rs_nombre_doc
                              };
                            SessionBE.rn_cod_antec = rptConsultaAntecedente[0].rn_cod_antec;
                            SessionBE.cod_antec = new int[] { rptConsultaAntecedente[0].rn_cod_antec };
                            SessionBE.rs_cod_antec = rptConsultaAntecedente[0].rn_cod_antec.ToString();
                            SessionBE.rn_fla_vigencia = rptConsultaAntecedente[0].rn_fla_vigencia;
                            SessionBE.rn_fla_generado = rptConsultaAntecedente[0].rn_fla_generado;
                            SessionBE.tipo_documentos = new string[]
                            {
                            (rptConsultaAntecedente[0].rn_cod_tipantec == 1) ? "CD" : "CE"
                              };
                        }
                        else
                        {


                            foreach (RptConsutaAntecedenteBE row in rptConsultaAntecedente)
                            {
                                SessionBE.cod_antec = new int[] { row.rn_cod_antec };

                            }
                            SessionBE.n_cod_intmp = rptConsultaAntecedente[0].rn_cod_intmp;

                        }

                     
                   

                        HttpContext.Session.Remove("ssglobal");
                        var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(SessionBE);
                        HttpContext.Session.SetString("ssglobal", jsonString);


                        //consulta antecedente
                        AuditoriaAntecedentesBE audit_antecedentes = new AuditoriaAntecedentesBE();
                        audit_antecedentes.cod_intper = SessionBE.cod_intper;
                        audit_antecedentes.num_documento = SessionBE.num_documento;
                        audit_antecedentes.cod_usuario = SessionBE.cod_usuario;
                        audit_antecedentes.ap_paterno = SessionBE.ap_paterno;
                        audit_antecedentes.ap_materno = SessionBE.ap_materno;
                        audit_antecedentes.nombres = SessionBE.nombres;

                        audit_antecedentes.cod_tipmovantec = (datos.tipo_consulta == 1) ? 2:  3;
                        audit_antecedentes.cod_origantec = 1;
                      
                        audit_antecedentes.des_auditoria = "N°" + SessionBE.nro_registro + SessionBE.anio_td; 

                        audit_antecedentes.id_sesion = SessionBE.id_sesion;

                        audit_antecedentes.direccion_ip = SessionBE.direccion_ip;
                        audit_antecedentes.mac_address = SessionBE.mac_address;
                        audit_antecedentes.computer_name = SessionBE.computer_name;
                        audit_antecedentes.sistema_operativo = SessionBE.sistema_operativo;



                        RptVerificacionBE rptAuditoria = await _antecedentesModel.AuditoriaAntecedentes(audit_antecedentes);
                        if (rptAuditoria.rn_codigo != 0)
                        {
                            rpta.codigo = -1;
                            rpta.valor = "Surgio un error inesperado: " + rptAuditoria.rs_valor;
                            return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
                        }


                        return Json(new { redirectTo = Url.Action("TramiteCertificado", "Autenticacion") }, new Newtonsoft.Json.JsonSerializerSettings());
                    }
                    else
                    {
                        rpta.codigo = -1;
                        rpta.valor = "No se encontró información para los Datos ingresados";
                        return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
                    }



                }
                else
                {
                    rpta.codigo = -1;
                    rpta.valor = "El código Captcha ingresado es incorrecto.";
                }
            }
            catch (Exception ex)
            {
                LogAntecedentes.Logger().Error("Parametros: => Error: " + ex);
                rpta.codigo = -1;
                rpta.valor = "Surgio un error Inesperado";
            }

            return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
        }
        public async Task<IActionResult> DescargarCetificadoPDF()
        {
            if (HttpContext.Session.GetString("ssgenerico") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var TokenSesion = HttpContext.Session.GetString("TokenSesion");

            var tiempoRestante = _jwtManager.ValidateTokenUserID(TokenSesion);

            if (tiempoRestante == null)
            {
                // Manejar caso donde no hay token en la cookie
                return RedirectToAction("Index", "Home");
            }


            var ssglobal = HttpContext.Session.GetString("ssglobal");
            var sesionGlobal = Newtonsoft.Json.JsonConvert.DeserializeObject<SesionAntecedentesBE>(ssglobal);
            try
            {
                //BUSCAMOS EN FTP ZAVALA
                CredencialesFTPBE credeniales = await genericoModel.GetCredencialesFTP(sesionGlobal.cod_distrito_ftp);
                FtpClient FTP = new FtpClient("ftp://" + credeniales.server_ftp3, credeniales.user_ftp3, credeniales.pass_ftp3);

                if (credeniales == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "No se pudo obtener las Credenciales del FTP");


                }

                byte[] byte_pdf = FTP.downloadBytes(sesionGlobal.nombre_doc);

                if (byte_pdf == null)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError, "Error: El archivo se subió dañado, o no coincide con el la ruta registrada.");


                }
                bool esValido = EsArchivoPDF(byte_pdf);

                if (!esValido)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "ERROR al Descargar Archivo del Servidor FTP, Archivo Dañado o No se encuentra en la ruta proporcionada.");

                }

                //auditoria REGISTRA TRAMITE DOCUMENTARIO
                AuditoriaAntecedentesBE audit_antecedentes = new AuditoriaAntecedentesBE();
                audit_antecedentes.cod_intper = sesionGlobal.cod_intper;
                audit_antecedentes.num_documento = sesionGlobal.num_documento;
                audit_antecedentes.cod_usuario = sesionGlobal.cod_usuario;
                audit_antecedentes.ap_paterno = sesionGlobal.ap_paterno;
                audit_antecedentes.ap_materno = sesionGlobal.ap_materno;
                audit_antecedentes.nombres = sesionGlobal.nombres;

                audit_antecedentes.cod_tipmovantec = 6;
                audit_antecedentes.cod_origantec = 1;
                audit_antecedentes.des_auditoria = "N°" + sesionGlobal.nro_registro + sesionGlobal.anio_td;

                audit_antecedentes.id_sesion = sesionGlobal.id_sesion;


                audit_antecedentes.direccion_ip = sesionGlobal.direccion_ip;
                audit_antecedentes.mac_address = sesionGlobal.mac_address;
                audit_antecedentes.computer_name = sesionGlobal.computer_name;
                audit_antecedentes.sistema_operativo = sesionGlobal.sistema_operativo;





                RptVerificacionBE rptAuditoria = await _antecedentesModel.AuditoriaAntecedentes(audit_antecedentes);
                if (rptAuditoria.rn_codigo != 0)
                {
               
                    return StatusCode(StatusCodes.Status500InternalServerError, rptAuditoria.rs_valor);

                }


                audit_antecedentes.cod_antec = sesionGlobal.cod_antec[0];
               
                audit_antecedentes.des_mov_antec = "N°"+sesionGlobal.nro_registro+ sesionGlobal.anio_td;






                RptPGBE rptAuditoriaMov = await _antecedentesModel.AuditoriaMantMovimiento(audit_antecedentes);

                if (rptAuditoriaMov.rn_codigo != 0)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError, rptAuditoriaMov.rs_valor);

                }

                return File(byte_pdf, "application/pdf", sesionGlobal.des_tip_repo+".pdf");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Se produjo un error:" + ex);
            }
        }

        public async Task<ActionResult> ObtenerCertificadoFTP(string nombre_archivo,int cod_antec, int tipmov_antec,string des_movantec)
        {
            if (HttpContext.Session.GetString("ssgenerico") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var TokenSesion = HttpContext.Session.GetString("TokenSesion");

            var tiempoRestante = _jwtManager.ValidateTokenUserID(TokenSesion);

            if (tiempoRestante == null)
            {
                // Manejar caso donde no hay token en la cookie
                return RedirectToAction("Index", "Home");
            }

            var ssglobal = HttpContext.Session.GetString("ssglobal");
            var sesionGlobal = Newtonsoft.Json.JsonConvert.DeserializeObject<SesionAntecedentesBE>(ssglobal);

            try
            {
                //BUSCAMOS EN FTP ZAVALA
                CredencialesFTPBE credeniales = await genericoModel.GetCredencialesFTP(sesionGlobal.cod_distrito_ftp);
                FtpClient FTP = new FtpClient("ftp://" + credeniales.server_ftp3, credeniales.user_ftp3, credeniales.pass_ftp3);

                if (credeniales == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "No se pudo obtener las Credenciales del FTP");


                }

                byte[] byte_pdf = FTP.downloadBytes(nombre_archivo);

                if (byte_pdf == null)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError, "Error: El archivo se subió dañado, o no coincide con el la ruta registrada.");


                }
                bool esValido = EsArchivoPDF(byte_pdf);

                if (!esValido)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "ERROR al Descargar Archivo del Servidor FTP, Archivo Dañado o No se encuentra en la ruta proporcionada.");

                }

                AuditoriaAntecedentesBE audit_antecedentes = new AuditoriaAntecedentesBE();
                audit_antecedentes.cod_intper = sesionGlobal.cod_intper;
                audit_antecedentes.num_documento = sesionGlobal.num_documento;
                audit_antecedentes.cod_usuario = sesionGlobal.cod_usuario;
                audit_antecedentes.ap_paterno = sesionGlobal.ap_paterno;
                audit_antecedentes.ap_materno = sesionGlobal.ap_materno;
                audit_antecedentes.nombres = sesionGlobal.nombres;

                audit_antecedentes.cod_tipmovantec = tipmov_antec;
                audit_antecedentes.cod_origantec = 1;
                audit_antecedentes.des_auditoria = "N°" + sesionGlobal.nro_registro + sesionGlobal.anio_td;

                audit_antecedentes.id_sesion = sesionGlobal.id_sesion;


                audit_antecedentes.direccion_ip = sesionGlobal.direccion_ip;
                audit_antecedentes.mac_address = sesionGlobal.mac_address;
                audit_antecedentes.computer_name = sesionGlobal.computer_name;
                audit_antecedentes.sistema_operativo = sesionGlobal.sistema_operativo;


                RptVerificacionBE rptAuditoria = await _antecedentesModel.AuditoriaAntecedentes(audit_antecedentes);
                if (rptAuditoria.rn_codigo != 0)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError, rptAuditoria.rs_valor);

                }



                if (tipmov_antec != 7)
                {

                  
                   
                    audit_antecedentes.cod_antec = cod_antec;
                    
                    audit_antecedentes.des_mov_antec = des_movantec;
                
                    RptPGBE rptAuditoriaMov = await _antecedentesModel.AuditoriaMantMovimiento(audit_antecedentes);

                    if (rptAuditoriaMov.rn_codigo != 0)
                    {

                        return StatusCode(StatusCodes.Status500InternalServerError, rptAuditoriaMov.rs_valor);

                    }
                }
                string nombre_pdf = System.IO.Path.GetFileName(sesionGlobal.nombre_doc);

                return File(byte_pdf, "application/pdf", "Certificado Antecedentes " + nombre_pdf);

               

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Se produjo un error:" + ex);
            }

        }
        private bool EsArchivoPDF(byte[] bytes)
        {
            // El primer carácter de un archivo PDF es '%PDF'
            if (bytes.Length < 4 || !(bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46))
            {
                return false;
            }

            return true;
        }

        static byte[] GenerarClaveAleatoria(int longitud)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] claveAleatoria = new byte[longitud];
                rng.GetBytes(claveAleatoria);
                return claveAleatoria;
            }
        }

    
    }
}
