using ANC.AntecedentesWeb.Api_Tokens;
using ANC.AntecedentesWeb.UtilesPC;
using ANC.Comun;
using ANC.Comun.Log4net;
using ANC.Entidades;
using ANC.Modelo;

using Microsoft.AspNetCore.Mvc;
using SRVTextToImage;
using System.Drawing.Imaging;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ANC.AntecedentesWeb.Controllers
{
    public class AutenticacionController : Controller
    {
        Funciones fn = new Funciones();
        GenericoModel genericoModel = new GenericoModel();
        AntecedentesModel _antecedentesModel = new AntecedentesModel();

        util _utilSession = new util();

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly JwtManager _jwtManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static string correo_copia_uti = "", estado_envio = "";
        public AutenticacionController(IConfiguration configuration, IWebHostEnvironment hostEnvironment, JwtManager jwtManager, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _jwtManager = jwtManager;
            _httpContextAccessor = httpContextAccessor;
            correo_copia_uti = _configuration.GetValue<string>("EMAIL_CORREO_UTI_COPIA");
            estado_envio = _configuration.GetValue<string>("ESTADO_ENVIO");
        }
        public ActionResult<double> GenerarTokenSesion()
        {
            if (HttpContext.Session.GetString("ssgenerico") == null)
            {
                return RedirectToAction("Salir", "Home");
            }

            var ssgenerico = HttpContext.Session.GetString("ssgenerico");

            var (token, expiresInMinutes) = _jwtManager.GenerateTokenUserID(ssgenerico);


            HttpContext.Session.SetString("TokenSesion", token);

            return expiresInMinutes;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("ssgenerico") == null)
            {
                return RedirectToAction("Salir", "Home");
            }

         



            var tiempoRestante = GenerarTokenSesion();


            ViewBag.tiempoRestante = tiempoRestante.Value;
            List<PetTipoDocBE> lstTipoDoc = await genericoModel.ObtenerTipoDoc();
            ViewBag.lstTipoDoc = lstTipoDoc;

            List<DepartamentoBE> lstDepartamento = await genericoModel.ObtenerDepartamento();
            ViewBag.lstDepartamento = lstDepartamento;


            return View();
        }

        public async Task<ActionResult> ObtenerProvincia(string IdDepartamento)
        {
            List<ProvinciaBE> lstProvincia = await genericoModel.ObtenerProvincia(IdDepartamento);

            return Json(lstProvincia, new Newtonsoft.Json.JsonSerializerSettings());
        }
        public async Task<ActionResult> ObtenerDistrito(string IdDepartamento, string IdProvincia)
        {
            List<DistritoBE> lstDistrito = await genericoModel.ObtenerDistrito(IdDepartamento, IdProvincia);

            return Json(lstDistrito, new Newtonsoft.Json.JsonSerializerSettings());
        }

        [HttpPost]
        public async Task<IActionResult> ValidarUsuario([FromBody] TipoReporteBE data)
        {
            if (HttpContext.Session.GetString("ssgenerico") == null)
            {
                return RedirectToAction("Index", "Home");
            }


            var ssglobal = HttpContext.Session.GetString("ssglobal");
            var sesionGlobal = Newtonsoft.Json.JsonConvert.DeserializeObject<SesionAntecedentesBE>(ssglobal);

            RptBE rpta = new RptBE();
            try
            {
                var TokenSesion = HttpContext.Session.GetString("TokenSesion");

                var tiempoRestante = _jwtManager.ValidateTokenUserID(TokenSesion);

                if (tiempoRestante == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.tiempoRestante = tiempoRestante;


                List<int> cod_repo = new List<int>();




                if (!string.IsNullOrEmpty(data._radioTipoRepo1))
                {
                    int tip_01 = Int32.Parse(data._radioTipoRepo1);
                    cod_repo.Add(tip_01);
                }
              
                if (!string.IsNullOrEmpty(data._radioTipoRepo2))
                {
                    int tip_02 = Int32.Parse(data._radioTipoRepo2);
                    cod_repo.Add(tip_02);
                }


                int[] cod_tiprepo = cod_repo.ToArray();




                if (cod_tiprepo.Length==0)
                {
                    rpta.codigo = -1;
                    rpta.valor = "Seleccione al menos un tipo de Certificado a solicitar";
                    return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
                }


                bool estado_verificacion = true;
                string valor_verificacion = "";
                int count = 1;
               

                sesionGlobal.cod_subtipo_repo = 1;

                sesionGlobal.cod_origantec = 1;
                sesionGlobal.nro_registro = "";

                //VALIDACION DE DATOS CON PROCEDIMIENTO 
                AntecedentesBE _datosAntecedentes = new AntecedentesBE();

                _datosAntecedentes.num_documento = sesionGlobal.num_documento;
                _datosAntecedentes.ap_paterno = sesionGlobal.ap_paterno;
                _datosAntecedentes.ap_materno = sesionGlobal.ap_materno;
                _datosAntecedentes.nombres = sesionGlobal.nombres;

                RptVerificacionBE rptVerificacion = await _antecedentesModel.LoginVericar(_datosAntecedentes);
               
               
            
               
                if (rptVerificacion.codigo != 0)
                {
                    sesionGlobal.cod_distrito_ftp = rptVerificacion.cod_distri;

                    foreach (int row in cod_tiprepo)
                    {
                        sesionGlobal.cod_tipo_repo = row;
                        string titulo_correo = (sesionGlobal.cod_tipo_repo == 1) ? "DISCIPLINARIOS" : "DE EXPEDIENTES";
                        RptVerificacionBE rptVerificacionTDAntec = await _antecedentesModel.VerificarTDAntecedente(sesionGlobal.num_documento, sesionGlobal.cod_tipo_repo);

                        if (rptVerificacionTDAntec.rn_codigo != 0)
                        {
                            estado_verificacion = false;

                            if (count == 2)
                            {
                                valor_verificacion += "; DEL MISMO MODO, " + rptVerificacionTDAntec.rs_valor + " PARA EL CERTIFICADO DE ANTECEDENTES " + titulo_correo;

                            }
                            else
                            {
                                valor_verificacion += rptVerificacionTDAntec.rs_valor + " PARA EL CERTIFICADO DE ANTECEDENTES " + titulo_correo;

                            }



                        }

                        count++;
                    }
                    if (!estado_verificacion)
                    {
                        rpta.codigo = -1;
                        rpta.valor = valor_verificacion;
                        return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
                    }

                    //se inicializa nuevamente las variables.
                    count = 1;
                    estado_verificacion = true;
                    valor_verificacion = "";
                    foreach (int row in cod_tiprepo)
                    {

                        sesionGlobal.cod_tipo_repo = row;


                        TramiteDocumentarioBE DatosTramiteDoc = new TramiteDocumentarioBE();

                        DatosTramiteDoc.ap_paterno = sesionGlobal.ap_paterno;
                        DatosTramiteDoc.ap_materno = sesionGlobal.ap_materno;
                        DatosTramiteDoc.nombres = sesionGlobal.nombres;
                        DatosTramiteDoc.num_documento = sesionGlobal.num_documento;
                        DatosTramiteDoc.correo = sesionGlobal.correo;
                        DatosTramiteDoc.tipo_antecedente = sesionGlobal.cod_tipo_repo;
                        DatosTramiteDoc.nro_celular = sesionGlobal.telefono;
                        DatosTramiteDoc.des_departamento = sesionGlobal.des_departamento;
                        DatosTramiteDoc.des_provincia = sesionGlobal.des_provincia;
                        DatosTramiteDoc.des_distrito = sesionGlobal.des_distrito;
                        DatosTramiteDoc.firma_user = sesionGlobal.firma_reniec;
                        DatosTramiteDoc.cod_distrito_ftp = sesionGlobal.cod_distrito_ftp;
                        DatosTramiteDoc.cod_orig_antec = sesionGlobal.cod_origantec;
                        DatosTramiteDoc.usuario = "USRSYS";
                        DatosTramiteDoc.accion = "N";

                        RptTramiteDocumentarioBE RspTramiteDoc = await _antecedentesModel.RegistroTramiteDocumentario(DatosTramiteDoc);

                        if (RspTramiteDoc.rn_codigo != 0)
                        {
                            estado_verificacion = false;
                            if (count == 2)
                            {

                                valor_verificacion += " ; " + RspTramiteDoc.rs_valor;
                            }
                            else
                            {

                                valor_verificacion += RspTramiteDoc.rs_valor;

                            }

                            rpta.codigo = -1;

                            rpta.valor = (sesionGlobal.cod_tipo_repo == 1) ? "SURGIO UN ERROR AL SOLICITAR REGISTRO DE EN EL TRÁMITE DOCUMENTARIO PARA LA GENERACION DE  CERTIFICADO DE ANTECEDENTES DISCIPLINARIOS" : "SURGIO UN ERROR AL SOLICITAR REGISTRO DE EN EL TRÁMITE DOCUMENTARIO PARA LA GENERACION DE CERTIFICADO DE ANTECEDENTES DISCIPLINARIOS DE EXPEDIENTES"; 
                            LogAntecedentes.Logger().Error(" Error al Generar Registro en tramite documentario Usuario:"+ sesionGlobal.num_documento+"=====> ERROR : " + valor_verificacion);
                            return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());


                        }


                        //auditoria REGISTRA TRAMITE DOCUMENTARIO
                        AuditoriaAntecedentesBE audit_antecedentes = new AuditoriaAntecedentesBE();
                        audit_antecedentes.cod_intper = sesionGlobal.cod_intper;
                        audit_antecedentes.num_documento = sesionGlobal.num_documento;
                        audit_antecedentes.cod_usuario = sesionGlobal.cod_usuario;
                        audit_antecedentes.ap_paterno = sesionGlobal.ap_paterno;
                        audit_antecedentes.ap_materno = sesionGlobal.ap_materno;
                        audit_antecedentes.nombres = sesionGlobal.nombres;

                        audit_antecedentes.cod_tipmovantec = 5;
                        audit_antecedentes.cod_origantec = 1;
                        audit_antecedentes.des_auditoria = "N°" + RspTramiteDoc.rs_registro_mp + RspTramiteDoc.rs_anio_mp;


                        audit_antecedentes.id_sesion = sesionGlobal.id_sesion;




                        audit_antecedentes.direccion_ip = sesionGlobal.direccion_ip;
                        audit_antecedentes.mac_address = sesionGlobal.mac_address;
                        audit_antecedentes.computer_name = sesionGlobal.computer_name;
                        audit_antecedentes.sistema_operativo = sesionGlobal.sistema_operativo;





                        RptVerificacionBE rptAuditoria = await _antecedentesModel.AuditoriaAntecedentes(audit_antecedentes);
                        if (rptAuditoria.rn_codigo != 0)
                        {
                            estado_verificacion = false;
                            if (count == 2)
                            {
                                valor_verificacion += " ; " + rptAuditoria.rs_valor;
                            }
                            else
                            {
                                valor_verificacion += rptAuditoria.rs_valor;

                            }
                        
                          
                        }

                        AcuseCorreoBE datos = new AcuseCorreoBE();


                        string[] regitro_antec = new string[]
                          {
                             RspTramiteDoc.rs_registro_mp + "-" + RspTramiteDoc.rs_anio_mp
                          };
                        int[] cod_antec = new int[0];





                        datos.ruta_archivo = RspTramiteDoc.ruta_documentos;
                        datos.tipo_documentos = RspTramiteDoc.tipo_documentos;
                        datos.usu_envio = sesionGlobal.num_documento;
                        datos.correo_destino = sesionGlobal.correo;
                        datos.nombres = sesionGlobal.nombres + " " + sesionGlobal.ap_paterno + " " + sesionGlobal.ap_materno; ;
                        datos.registro_td = "";
                        datos.regitro_antec = regitro_antec;

                        datos.cod_intmp = RspTramiteDoc.rn_cod_intmp;
                        datos.cod_antec = cod_antec;
                        datos.cod_distrito = sesionGlobal.cod_distrito_ftp;

                        datos.ipPc = sesionGlobal.direccion_ip;
                        datos.pcNameipPc = sesionGlobal.computer_name;
                        datos.usuarioSis = sesionGlobal.num_documento;
                        datos.usuarioRed = "";
                        datos.nombreSo = sesionGlobal.sistema_operativo;
                        datos.macadress = sesionGlobal.mac_address;
                        if (estado_envio == "1")
                        {
                            datos.correo_coo = correo_copia_uti;
                        }
                        string titulo_correo = (sesionGlobal.cod_tipo_repo == 1) ? "DISCIPLINARIOS" : "DE EXPEDIENTES";

                        datos.asunto = "NOTIFICACIÓN DE ANTECEDENTES " + titulo_correo + " - REG.TRÁMITE DOCUMENTARIO " + "T" + RspTramiteDoc.rs_registro_mp + "-" + RspTramiteDoc.rs_anio_mp + " FECHA NOTIF (" + DateTime.Now.ToString("dd/MM/yyyy") + ")";
                        datos.rs_valor = "TD_WEB";

                        RptPGBE rpta_email = await _antecedentesModel.EnviarAcuseAntecedente(datos);
                        if (rpta_email.rn_codigo != 0)
                        {
                            estado_verificacion = false;
                            if (count == 2)
                            {
                                valor_verificacion += " ; " + rpta_email.rs_valor;
                            }
                            else
                            {
                                valor_verificacion += rpta_email.rs_valor;

                            }



                        }

                        if (count == 2)
                        {
                            sesionGlobal.nro_registro += " ; " + "T"+ RspTramiteDoc.rs_registro_mp + "-" + RspTramiteDoc.rs_anio_mp;

                        }
                        else
                        {
                            sesionGlobal.nro_registro += "T"+RspTramiteDoc.rs_registro_mp + "-" + RspTramiteDoc.rs_anio_mp;


                        }

                        count++;
                    }


                    if (!estado_verificacion)
                    {
                        sesionGlobal.estado_error = -1;
                        sesionGlobal.des_error = valor_verificacion;
                 
                    }

                    sesionGlobal.cod_estado_validacion = -1; //significa q no se encontraron registros 
                    HttpContext.Session.SetString("ssglobal", Newtonsoft.Json.JsonConvert.SerializeObject(sesionGlobal));


                    return Json(new { redirectTo = Url.Action("TramiteCertificado", "Autenticacion") }, new Newtonsoft.Json.JsonSerializerSettings());



                }

                sesionGlobal.cod_estado_validacion = 1; //significa q si se encontraron registros y esta registrado en nuestra BD
                sesionGlobal.cod_intper = rptVerificacion.cod_intper;
                sesionGlobal.cod_distrito_ftp = rptVerificacion.cod_distri;





                try
                {
                  string des_tip_repo="", nro_registro="", fec_expedido = "", fec_caducidad = "", cod_usuario,ruta_doc = "", rn_cod_antec="";
               

                    foreach (int row in cod_tiprepo)
                    {
                        sesionGlobal.cod_tipo_repo = row;
                        string msg_error = "";
                        RespuestaArchivoBE recordAntecedentes = await _antecedentesModel.GenerarCertificadoPDF(sesionGlobal);

                        if (recordAntecedentes.codigo == -1)
                        {
                            estado_verificacion = false;
                            if (count == 2)
                            {
                               valor_verificacion += " ; " + recordAntecedentes.valor;
                            }
                            else
                            {
                               
                                valor_verificacion += recordAntecedentes.valor;

                            }
                            msg_error = (sesionGlobal.cod_tipo_repo == 1) ? "SURGIO UN ERROR AL GENERAR CERTIFICADO DE ANTECEDENTES DISCIPLINARIOS" : "SURGIO UN ERROR AL GENERAR CERTIFICADO DE ANTECEDENTES DISCIPLINARIOS DE EXPEDIENTES";

                            rpta.codigo = -1;

                            rpta.valor = msg_error;
                            LogAntecedentes.Logger().Error(" Error al Generar Certificado  Usuario:"+ sesionGlobal.num_documento+"=====> ERROR :" + valor_verificacion);
                            return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());


                        }
                     
                        AcuseCorreoBE datos = new AcuseCorreoBE();
                        string[] rpt_api = recordAntecedentes.valor.Split('~');
                        string[] ruta_archivo = new string[]
                           {
                        rpt_api[1]
                           };
                        string[] regitro_antec = new string[]
                          {
                       recordAntecedentes.nro_registro+"-"+ recordAntecedentes.anio
                          };
                        int[] cod_antec = new int[]
                        {
                     Int32.Parse(rpt_api[0])
                         };


                        sesionGlobal.nombre_doc = rpt_api[1];
                        var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(sesionGlobal);
                        HttpContext.Session.SetString("ssglobal", jsonString);
                        datos.ruta_archivo = ruta_archivo;

                        datos.tipo_documentos = new string[]
                           {
                        (sesionGlobal.cod_tipo_repo == 1) ? "CD" : "CE"
                         };

                        datos.usu_envio = sesionGlobal.num_documento;
                        datos.correo_destino = sesionGlobal.correo;
                        datos.nombres = sesionGlobal.nombres + " " + sesionGlobal.ap_paterno + " " + sesionGlobal.ap_materno; ;
                        datos.registro_td = "";
                        datos.regitro_antec = regitro_antec;

                        datos.cod_intmp = 0;
                        datos.cod_antec = cod_antec;
                        datos.cod_distrito = sesionGlobal.cod_distrito_ftp;

                        datos.ipPc = sesionGlobal.direccion_ip;
                        datos.pcNameipPc = sesionGlobal.computer_name;
                        datos.usuarioSis = sesionGlobal.cod_intper.ToString();
                        datos.usuarioRed = "";
                        datos.nombreSo = sesionGlobal.sistema_operativo;
                        datos.macadress = sesionGlobal.mac_address;
                        if (estado_envio == "1")
                        {
                            datos.correo_coo = correo_copia_uti;
                        }
                        string titulo_correo = (sesionGlobal.cod_tipo_repo == 1) ? "DISCIPLINARIOS" : "DE EXPEDIENTES";

                        datos.asunto = "NOTIFICACIÓN DE ANTECEDENTES " + titulo_correo + " - REG.CERTIFICADO " + "C" + recordAntecedentes.nro_registro + "-" + recordAntecedentes.anio + " FECHA NOTIF (" + DateTime.Now.ToString("dd/MM/yyyy") + ")";

                        datos.rs_valor = "TC_WEB";


                        RptPGBE rpta_email = await _antecedentesModel.EnviarAcuseAntecedente(datos);

                        if (rpta_email.rn_codigo != 0)
                        {
                            estado_verificacion = false;
                            if (count == 2)
                            {

                                valor_verificacion += " ; " + rpta_email.rs_valor;
                            }
                            else
                            {

                                valor_verificacion += rpta_email.rs_valor;

                            }

                            //rpta.codigo = -1;
                            //rpta.valor = rpta_email.rs_valor;
                            //return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());


                        }
                        if (count == 2)
                        {

                            des_tip_repo += " - CERTIFICADO DE ANTECEDENTES " + titulo_correo;
                            ruta_doc += "~" + rpt_api[1];
                            rn_cod_antec += "~" + rpt_api[0];
                            //sesionGlobal.cod_antec += cod_antec;
                            nro_registro += " ; "+ "C" + recordAntecedentes.nro_registro + "-" + recordAntecedentes.anio;
                            fec_expedido +="~" +recordAntecedentes.fec_expedicion;
                            fec_caducidad += "~" + recordAntecedentes.fec_vigencia;

                           

                        }
                        else
                        {

                            des_tip_repo += "CERTIFICADO DE ANTECEDENTES " + titulo_correo;
                            ruta_doc += rpt_api[1];
                            rn_cod_antec += rpt_api[0];
                            //sesionGlobal.cod_antec += cod_antec;
                            nro_registro += "C" + recordAntecedentes.nro_registro + "-" + recordAntecedentes.anio;
                            fec_expedido += recordAntecedentes.fec_expedicion;
                            fec_caducidad += recordAntecedentes.fec_vigencia;

                           


                        }

                        count++;

                    }
                    if (!estado_verificacion)
                    {
                        sesionGlobal.estado_error = -1;
                        sesionGlobal.des_error = valor_verificacion;
                        //rpta.codigo = -1;
                        //rpta.valor = valor_verificacion;
                        //return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
                    }
                    sesionGlobal.des_tip_repo = des_tip_repo;
                    sesionGlobal.rs_cod_antec = rn_cod_antec;
                    sesionGlobal.ruta_certificado = ruta_doc;
                    //sesionGlobal.cod_antec += cod_antec;
                    sesionGlobal.nro_registro = nro_registro;
                    sesionGlobal.fec_expedido = fec_expedido;
                    sesionGlobal.fec_caducidad = fec_caducidad;
                    var addSesion = Newtonsoft.Json.JsonConvert.SerializeObject(sesionGlobal);


                    HttpContext.Session.SetString("ssglobal", addSesion);


                    return Json(new { redirectTo = Url.Action("TramiteCertificado", "Autenticacion") }, new Newtonsoft.Json.JsonSerializerSettings());


                }
                catch (Exception ex)
                {
                    LogAntecedentes.Logger().Error("Parametros: => Error al Generar Certificado: " + ex);
                    rpta.codigo = -1;
                    rpta.valor = "Surgio un error Inesperado";
                }



            }
            catch (Exception ex)
            {
                LogAntecedentes.Logger().Error("Parametros: => Se genero un Error : " + ex);
                rpta.codigo = -1;
                rpta.valor = "Surgio un error Inesperado";
            }

            return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
        }
        public async Task<IActionResult> TramiteCertificado()
        {
            if (HttpContext.Session.GetString("ssgenerico") == null)
            {
                return RedirectToAction("Index", "Home");
            }


            var ssglobal = HttpContext.Session.GetString("ssglobal");
            var sesionGlobal = Newtonsoft.Json.JsonConvert.DeserializeObject<SesionAntecedentesBE>(ssglobal);

            try
            {


                if (sesionGlobal.cod_tipmovantec == 1)
                {


                    var TokenSesion = HttpContext.Session.GetString("TokenSesion");

                    var tiempoRestante = _jwtManager.ValidateTokenUserID(TokenSesion);

                    if (tiempoRestante == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    ViewBag.tiempoRestante = tiempoRestante;


                    ViewBag.nombresCompletos = sesionGlobal.nombres + " " + sesionGlobal.ap_paterno + " " + sesionGlobal.ap_materno;
                    ViewBag.estado_error = sesionGlobal.estado_error;
                    ViewBag.des_error = sesionGlobal.des_error;
                    if (sesionGlobal.cod_estado_validacion == -1)
                    {


                        ViewBag.mensaje_estado = "NO SE ENCONTRARON REGISTRO EN NUESTRO SISTEMAS, PERO SU SOLICITUD FUÉ REGISTRADA";
                        ViewBag.nro_tramite =sesionGlobal.nro_registro;
                        ViewBag.tipo_tramite = "TRÁMITE DOCUMENTARIO";
                        ViewBag.correo_estado = "La respuesta será enviada a su correo electrónico a la brevedad posible";
                        ViewBag.fec_expedido = "";
                        ViewBag.fec_caducidad = "";
                        ViewBag.ViewOptions = 3;
                        ViewBag.rn_fla_vigencia = 1;


                        return View();

                    }
                    else
                    {
                       
                        ViewBag.tipo_tramite = sesionGlobal.des_tip_repo;
                        ViewBag.mensaje_estado = "SOLICITUD GENERADA";
                        ViewBag.nro_tramite =  sesionGlobal.nro_registro;
                        ViewBag.ruta_certificado = sesionGlobal.ruta_certificado;
                        ViewBag.cod_antec=sesionGlobal.rs_cod_antec;
                        ViewBag.correo_estado = "Esta información fué enviada a su correo electrónico";
                        ViewBag.fec_expedido = sesionGlobal.fec_expedido;
                        ViewBag.fec_caducidad = sesionGlobal.fec_caducidad;
                        ViewBag.ViewOptions = sesionGlobal.cod_tipmovantec;
                        ViewBag.rn_fla_vigencia = 1;


                        return View();
                    }

                }
                else
                {
                    GenerarTokenSesion();

                    var TokenSesion = HttpContext.Session.GetString("TokenSesion");

                    var tiempoRestante = _jwtManager.ValidateTokenUserID(TokenSesion);

                    if (tiempoRestante == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    ViewBag.tiempoRestante = tiempoRestante;


                    ViewBag.nombresCompletos = sesionGlobal.nombres + " " + sesionGlobal.ap_paterno + " " + sesionGlobal.ap_materno;

                    ViewBag.tipo_consulta = sesionGlobal.tipo_consulta;
                    ViewBag.mensaje_estado = "CONSULTA DE TRÁMITE";
                    if (sesionGlobal.tipo_consulta == 1)
                    {
                        string titulo_tramite = (sesionGlobal.cod_tipo_repo == 1) ? "DISCIPLINARIOS" : "DE EXPEDIENTES";

                        ViewBag.nro_tramite = (sesionGlobal.nro_tramite != null) ? "C" + sesionGlobal.nro_tramite+"-" + sesionGlobal.anio_td : "C" + sesionGlobal.nro_registro + "-" + sesionGlobal.anio_td;
                        ViewBag.tipo_tramite = "CERTIFICADO DE ANTECEDENTES " + titulo_tramite;
                        ViewBag.correo_estado = "La solicitud ha sido atendida";
                        ViewBag.fec_expedido = sesionGlobal.fec_expedido;
                        ViewBag.fec_caducidad = sesionGlobal.fec_caducidad;
                        ViewBag.ViewOptions = sesionGlobal.cod_tipmovantec;
                        ViewBag.rn_fla_vigencia = sesionGlobal.rn_fla_vigencia;
                        ViewBag.fla_genrado = sesionGlobal.rn_fla_generado;
                        ViewBag.cod_antec = sesionGlobal.rs_cod_antec;
                        ViewBag.ruta_certificado = sesionGlobal.ruta_certificado;
                    }
                    else
                    {
                        ConsultaAntecedenteBE datos = new ConsultaAntecedenteBE();


                        datos.cod_intper = sesionGlobal.cod_intper;


                        datos.num_documento = sesionGlobal.num_documento;
                        datos.nro_registro = sesionGlobal.nro_registro;
                        datos.anio = sesionGlobal.anio_td;
                        datos.cod_tipantec = 0;

                        datos.cod_subtipantec = 0;

                        datos.cod_origantec = 0;
                        datos.fla_vigencia = 9999;


                        List<RptConsutaAntecedenteBE> rptConsultaAntecedente = await _antecedentesModel.ConsultaAntecedentes(datos);

                        if (rptConsultaAntecedente.Count > 0)
                        {

                            ViewBag.tipo_tramite = "TRÁMITE DOCUMENTARIO";

                            ViewBag.nro_tramite = "T" + rptConsultaAntecedente[0].rs_nro_registro_td + "-" + rptConsultaAntecedente[0].rs_anio_td;
                            ViewBag.fla_existe = rptConsultaAntecedente[0].rn_fla_existe;
                            ViewBag.fla_genrado = rptConsultaAntecedente[0].rn_fla_generado;
                            ViewBag.rn_fla_vigencia = 1;
                            if (ViewBag.fla_existe == 0 && ViewBag.fla_genrado == 1)
                            {
                                ViewBag.correo_estado = "La solicitud ha sido Atendida.";

                                string[] archivo_ruta = new string[]
                             {
                                     rptConsultaAntecedente[0].rs_nombre_oficio
                             };
                                sesionGlobal.ruta_doc_ftp = archivo_ruta;

                                sesionGlobal.tipo_documentos = new string[]
                                  {
                                        "CO"
                                  };


                            }
                            else if(ViewBag.fla_genrado==0)
                            {
                                ViewBag.correo_estado = "La respuesta será enviada a su correo electrónico a la brevedad posible";

                            }
                            else
                            {
                                ViewBag.correo_estado = "Su solicitud fué atendida";
                                List<string> archivo_ruta = new List<string>();
                                List<string> tipo_documentos = new List<string>();




                                for (int i = 0; i < rptConsultaAntecedente.Count + 1; i++)
                                {




                                    if (i == rptConsultaAntecedente.Count)
                                    {
                                        archivo_ruta.Add(rptConsultaAntecedente[0].rs_nombre_oficio);
                                        tipo_documentos.Add("CO");

                                    }
                                    else
                                    {
                                        if (rptConsultaAntecedente[i].rn_fla_vigencia == 1)
                                        {
                                            archivo_ruta.Add(rptConsultaAntecedente[i].rs_nombre_doc);

                                            if (rptConsultaAntecedente[i].rn_cod_tipantec == 1)
                                            {


                                                tipo_documentos.Add("CD");
                                            }
                                            else
                                            {
                                                tipo_documentos.Add("CE");
                                            }

                                        }


                                    }

                                }
                                sesionGlobal.ruta_doc_ftp = archivo_ruta.ToArray();
                                sesionGlobal.tipo_documentos = tipo_documentos.ToArray();
                            }

                            ViewBag.ConsultaAntecedente = rptConsultaAntecedente;




                            ViewBag.ViewOptions = sesionGlobal.cod_tipmovantec;


                            var addSesion = Newtonsoft.Json.JsonConvert.SerializeObject(sesionGlobal);


                            HttpContext.Session.SetString("ssglobal", addSesion);

                        }
                        else
                        {
                            //en casos excepcionales ya q ya se valido anteriormente la informacion
                            ViewBag.ErrorMessage = "No se encontraron registros";
                            return RedirectToAction("Error", "Home");



                        }


                    }

                    return View();

                }
            }
            catch (Exception ex)
            {
                LogAntecedentes.Logger().Error("Parametros: => Se genero un Error : " + ex);
                ViewBag.ErrorMessage = "No se encontraron registros";

                return RedirectToAction("Error", "Home");

            }





        }
        [RequestFormLimits(ValueCountLimit = int.MaxValue)]
        [HttpPost]
        public async Task<ActionResult> ValidarDatos([FromBody] AntecedentesValidacionBE datos)
        {

            if (HttpContext.Session.GetString("ssgenerico") == null)
            {
                return RedirectToAction("Index", "Home");
            }


            RespuestaBE rpta = new RespuestaBE();
            ReniecBE reniecBE = new ReniecBE();
            SesionAntecedentesBE SessionBE = new SesionAntecedentesBE();
            try
            {
                if (HttpContext.Session.GetString("CaptchaImageText").ToUpper() == datos.textCaptcha.ToUpper())
                {
                    if (datos.txtEmail != datos.txtEmailconfirm)
                    {
                        rpta.codigo = "-1";
                        rpta.valor = "El correo ingresado y el correo de confirmación no coinciden, verificar";

                        return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
                    }
                    //verificar con login de erp
                    Util util = new Util(_httpContextAccessor);


                    SessionBE.direccion_ip = util.GetIPAddress();
                    SessionBE.mac_address = Util.GetMACAddress();
                    SessionBE.computer_name = Environment.MachineName;
                    var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();

                    // Asigna el sistema operativo a tu objeto SessionBE
                    SessionBE.sistema_operativo = Util.GetOperatingSystem(userAgent);

                    if (datos.txtNumDoc.Length == 8)
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
                                nroDocumentoIdentidad = datos.txtNumDoc

                            },
                            pagination = new Pagination
                            {
                                size = 30,
                                page = 0
                            },
                            auditoria = new AuditoriaReniecDedicada
                            {
                                usuario = "43719662",
                                nombrePc= SessionBE.computer_name,
                                direccionMac= SessionBE.mac_address,
                                numeroIp=SessionBE.direccion_ip

                            }
                        };
                        reniecBE = await genericoModel.ConsultarReniecDNI(reniec);

                        if (reniecBE.codigo == "-1")
                        {
                            rpta.codigo = reniecBE.codigo;
                            rpta.valor = reniecBE.descripcion;

                            return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
                        }
                        else if (reniecBE.codigo == "-2")
                        {
                            rpta.codigo = reniecBE.codigo;
                            rpta.valor = reniecBE.descripcion;
                            return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
                        }

                        if (reniecBE.fechafallecimiento != null)
                        {
                            rpta.codigo = "-1";
                            rpta.valor = "No se puede generar registro,¡Persona registrada como fallecida!";

                            return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
                        }

                        DateTime fechaNacimiento;
                        if (DateTime.TryParseExact(reniecBE.fechanac.ToString(), "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out fechaNacimiento))
                        {
                      
                        DateTime fechaActual = DateTime.Today;

                        int edad = fechaActual.Year - fechaNacimiento.Year;

                            if ((fechaNacimiento.Month > fechaActual.Month) || (fechaNacimiento.Month == fechaActual.Month && fechaNacimiento.Day > fechaActual.Day))
                            {
                                edad--;


                            }
                             
                            if (edad < 18)
                            {
                                rpta.codigo = "-1";
                                rpta.valor = "No se puede generar registro,¡Persona registrada como Menor de edad!";

                                return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
                            }
                        }

                       


                        //Verificar si usuario supero el limite de intentos
                        VerificaIngresoBE _verificaIngreso = new VerificaIngresoBE();
                        _verificaIngreso.nro_documento = reniecBE.numdni;
                        _verificaIngreso.as_direccion_ip = SessionBE.direccion_ip;

                        RptPGBE rptVerificaIngreso = await _antecedentesModel.VerificarIngreso(_verificaIngreso);

                        if (rptVerificaIngreso.rn_codigo != 0)
                        {
                            rpta.codigo = "-1";
                            rpta.valor = rptVerificaIngreso.rs_valor;

                            return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());

                        }

                        //VALIDACION DE DATOS CON RENIEC
                        var condiciones = new Dictionary<string, Func<bool>>
                        {
                            { "Fecha de emisión del documento", () => datos.FecEmisionDoc == reniecBE.fecha_expedicion },
                            { "Fecha de nacimiento", () => datos.FecNacimiento == reniecBE.fechanac },

                            { "Ubigeo", () => datos.txtUbigeo ==reniecBE.cod_ubigeonac},
                            { "Codigo Validación", () => datos.txtCodValid == reniecBE.digverifica},

                            { "Departamento de nacimiento", () => datos.cbDepartamento == reniecBE.departamentonac_detalle },
                            { "Provincia de nacimiento", () => datos.cbProvincia == reniecBE.provincianac_detalle },
                            { "Distrito de nacimiento", () => datos.cbDistrito == reniecBE.distritonac_detalle }
                        };


                        SessionBE.num_documento = reniecBE.numdni;
                        SessionBE.cod_tipodoc = datos.cbTipoDocumento;
                        SessionBE.cod_usuario = SessionBE.num_documento;

                        SessionBE.ap_paterno = reniecBE.apaterno;
                        SessionBE.ap_materno = reniecBE.amaterno;
                        SessionBE.nombres = reniecBE.nombres;

                        SessionBE.fec_emisiondoc = reniecBE.fecha_expedicion;
                        SessionBE.fec_nacimiento = reniecBE.fechanac;


                        SessionBE.coddepartamento = reniecBE.departamentonac;
                        SessionBE.codprovincia = reniecBE.provincianac;
                        SessionBE.coddistrito = reniecBE.distritonac;
                        SessionBE.id_sesion = HttpContext.Session.GetString("ssgenerico").ToString();

                        // Verificar cada condición
                        foreach (var condicion in condiciones)
                        {
                            if (!condicion.Value())
                            {
                                rpta.codigo = "-1";
                                rpta.valor = condicion.Key + " Ingresados no coinciden con los datos de Reniec";
                                //AUDITORIA INGRESO AL SISTEMA
                                AuditoriaAntecedentesBE audit_intentos = new AuditoriaAntecedentesBE();
                                audit_intentos.cod_intper = SessionBE.cod_intper;
                                audit_intentos.num_documento = SessionBE.num_documento;
                                audit_intentos.cod_usuario = SessionBE.cod_usuario;
                                audit_intentos.ap_paterno = SessionBE.ap_paterno;
                                audit_intentos.ap_materno = SessionBE.ap_materno;
                                audit_intentos.nombres = SessionBE.nombres;

                                audit_intentos.cod_tipmovantec = 13;
                                audit_intentos.cod_origantec = 1;
                                audit_intentos.des_auditoria = "";


                                audit_intentos.id_sesion = SessionBE.id_sesion;


                                audit_intentos.direccion_ip = SessionBE.direccion_ip;
                                audit_intentos.mac_address = SessionBE.mac_address;
                                audit_intentos.computer_name = SessionBE.computer_name;
                                audit_intentos.sistema_operativo = SessionBE.sistema_operativo;





                                RptVerificacionBE rptAuditoria_intentos = await _antecedentesModel.AuditoriaAntecedentes(audit_intentos);
                                if (rptAuditoria_intentos.rn_codigo != 0)
                                {
                                    LogAntecedentes.Logger().Error("Error al registrar intentos de acceso antecedentes: => : " + rptAuditoria_intentos.rs_valor);

                                }

                                return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());

                            }
                        }







                        
                        SessionBE.des_departamento = reniecBE.departamentonac_detalle;
                        SessionBE.des_distrito = reniecBE.distritonac_detalle;
                        SessionBE.des_provincia = reniecBE.provincianac_detalle;

                        string[] as_celular = new string[]
                       {
                        datos.txtTelefono
                       };

                        SessionBE.as_celular = as_celular;

                        SessionBE.telefono = datos.txtTelefono;

                        SessionBE.correo = datos.txtEmail;

                        string[] as_correos = new string[]
                         {
                        datos.txtEmail
                         };
                        SessionBE.as_correos = as_correos;

                        SessionBE.firma_reniec = reniecBE.firma;
                      
                        


                        SessionBE.foto = reniecBE.foto;


                        SessionBE.cod_tipmovantec = 1;

                        

                        HttpContext.Session.Remove("ssglobal");
                        var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(SessionBE);
                        HttpContext.Session.SetString("ssglobal", jsonString);


                        //AUDITORIA INGRESO AL SISTEMA
                        AuditoriaAntecedentesBE audit_antecedentes = new AuditoriaAntecedentesBE();
                        audit_antecedentes.cod_intper = SessionBE.cod_intper;
                        audit_antecedentes.num_documento = SessionBE.num_documento;
                        audit_antecedentes.cod_usuario = SessionBE.cod_usuario;
                        audit_antecedentes.ap_paterno = SessionBE.ap_paterno;
                        audit_antecedentes.ap_materno = SessionBE.ap_materno;
                        audit_antecedentes.nombres = SessionBE.nombres;

                        audit_antecedentes.cod_tipmovantec = SessionBE.cod_tipmovantec;
                        audit_antecedentes.cod_origantec = 1;
                        audit_antecedentes.des_auditoria = "USUARIO CON DNI: " + SessionBE.num_documento + " PASO LA VALIDACION RENIEC E INGRESO AL SISTEMA";


                        audit_antecedentes.id_sesion = SessionBE.id_sesion;




                        audit_antecedentes.direccion_ip = SessionBE.direccion_ip;
                        audit_antecedentes.mac_address = SessionBE.mac_address;
                        audit_antecedentes.computer_name = SessionBE.computer_name;
                        audit_antecedentes.sistema_operativo = SessionBE.sistema_operativo;





                        RptVerificacionBE rptAuditoria = await _antecedentesModel.AuditoriaAntecedentes(audit_antecedentes);
                        if (rptAuditoria.rn_codigo != 0)
                        {
                            rpta.codigo = "-1";
                            rpta.valor = "Surgio un error inesperado: " + rptAuditoria.rs_valor;
                            return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
                        }
                        return Json(new { redirectTo = Url.Action("TipoReporte") }, new Newtonsoft.Json.JsonSerializerSettings());


                    }
                }
                else
                {
                    rpta.codigo = "-1";
                    rpta.valor = "El código Captcha ingresado es incorrecto.";
                }

            }
            catch (Exception ex)
            {
                LogAntecedentes.Logger().Error("Parametros: => Error: " + ex);

                rpta.codigo = "-1";
            }

            return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
        }


        public IActionResult TipoReporte()
        {
            if (HttpContext.Session.GetString("ssgenerico") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var TokenSesion = HttpContext.Session.GetString("TokenSesion");

            var tiempoRestante = _jwtManager.ValidateTokenUserID(TokenSesion);

            if (tiempoRestante == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.tiempoRestante = tiempoRestante;


            return View();
        }

        public async Task<IActionResult> EnviarCorreo([FromBody] AntecedentesValidacionBE datos)
        {

            if (HttpContext.Session.GetString("ssgenerico") == null)
            {
                return RedirectToAction("Index", "Home");
            }


            RptBE rpta = new RptBE();
            AcuseCorreoBE data = new AcuseCorreoBE();
            var ssglobal = HttpContext.Session.GetString("ssglobal");
            var sesionGlobal = Newtonsoft.Json.JsonConvert.DeserializeObject<SesionAntecedentesBE>(ssglobal);
            if (string.IsNullOrEmpty(datos.txtEmail))
            {
                rpta.codigo = -1;
                rpta.valor = "Ingrese Correo Destino";
                return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
            }
            try
            {


                string[] regitro_antec = new string[]
                         {
                      sesionGlobal.nro_registro+"-"+sesionGlobal.anio_td
                };


                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(sesionGlobal);
                HttpContext.Session.SetString("ssglobal", jsonString);
                data.ruta_archivo = sesionGlobal.ruta_doc_ftp;
                data.tipo_documentos = sesionGlobal.tipo_documentos;
                data.usu_envio = sesionGlobal.num_documento;
                data.correo_destino = datos.txtEmail;
                data.nombres = sesionGlobal.nombres + " " + sesionGlobal.ap_paterno + " " + sesionGlobal.ap_materno;
                data.registro_td = sesionGlobal.nro_registro + "-" + sesionGlobal.anio_td;
                data.regitro_antec = regitro_antec;

                data.cod_intmp = sesionGlobal.n_cod_intmp;
                data.cod_antec = sesionGlobal.cod_antec;
                data.cod_distrito = sesionGlobal.cod_distrito_ftp;

                data.ipPc = sesionGlobal.direccion_ip;
                data.pcNameipPc = sesionGlobal.computer_name;
                data.usuarioSis = sesionGlobal.num_documento;
                data.usuarioRed = "";
                data.nombreSo = sesionGlobal.sistema_operativo;
                data.macadress = sesionGlobal.mac_address;

                if (estado_envio == "1")
                {
                    data.correo_coo = correo_copia_uti;
                }
                string titulo_correo = "", titulo_tip_reg = "", sigla = "";
                if (sesionGlobal.tipo_consulta == 1)
                {
                    titulo_correo = (sesionGlobal.cod_tipo_repo == 1) ? "DISCIPLINARIOS" : "DE EXPEDIENTES";
                    titulo_tip_reg = " REG.CERTIFICADO ";
                    data.rs_valor = "TC_WEB";
                    sigla = "C";

                }
                else
                {
                    titulo_correo = " TRÁMITE DOCUMENTARIO ";
                    titulo_tip_reg = " REG.TRÁMITE DOCUMENTARIO ";
                    data.rs_valor = "RTD_WEB";
                    sigla = "T";
                }




                data.asunto = "NOTIFICACIÓN DE ANTECEDENTES " + titulo_correo + " - " + titulo_tip_reg + sigla + data.registro_td + " FECHA NOTIF (" + DateTime.Now.ToString("dd/MM/yyyy") + ")";




                RptPGBE rpta_email = await _antecedentesModel.EnviarAcuseAntecedente(data);
                if (rpta_email.rn_codigo != 0)
                {
                    rpta.codigo = -1;
                    rpta.valor = rpta_email.rs_valor;
                    return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());

                }
                else
                {
                    rpta.codigo = 0;
                    rpta.valor = rpta_email.rs_valor;

                    AuditoriaAntecedentesBE audit_antecedentes = new AuditoriaAntecedentesBE();
                    audit_antecedentes.cod_intper = sesionGlobal.cod_intper;
                    audit_antecedentes.num_documento = sesionGlobal.num_documento;
                    audit_antecedentes.cod_usuario = sesionGlobal.cod_usuario;
                    audit_antecedentes.ap_paterno = sesionGlobal.ap_paterno;
                    audit_antecedentes.ap_materno = sesionGlobal.ap_materno;
                    audit_antecedentes.nombres = sesionGlobal.nombres;

                    audit_antecedentes.cod_tipmovantec = 11;
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
                    if (data.tipo_documentos[0] != "CO")
                    {
                        foreach (var item in sesionGlobal.cod_antec)
                        {
                            audit_antecedentes.cod_antec = item;

                            audit_antecedentes.des_mov_antec = "N°" + sesionGlobal.nro_registro + sesionGlobal.anio_td;

                            RptPGBE rptAuditoriaMov = await _antecedentesModel.AuditoriaMantMovimiento(audit_antecedentes);

                            if (rptAuditoriaMov.rn_codigo != 0)
                            {

                                return StatusCode(StatusCodes.Status500InternalServerError, rptAuditoriaMov.rs_valor);

                            }

                        }
                    }
                   





                }




            }
            catch (Exception ex)
            {

                rpta.codigo = -1;
                rpta.valor = "Surgio un Error";
                return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());


            }



            return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
        }

        public ActionResult CargarCaptchaImage()
        {
            RespuestaBE rpta = new RespuestaBE();

            try
            {
                CaptchaRandomImage CI = new CaptchaRandomImage();
                HttpContext.Session.SetString("CaptchaImageText", fn.GenerarCapcha(5));

                CI.GenerateImage(HttpContext.Session.GetString("CaptchaImageText"), 300, 75, System.Drawing.Color.Black, System.Drawing.Color.White);
                MemoryStream stream = new MemoryStream();
                CI.Image.Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);

                byte[] imageByteData = stream.ToArray();

                if (imageByteData != null)
                {
                    string imageBase64Data = Convert.ToBase64String(imageByteData);
                    string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                    rpta.codigo = "0";
                    rpta.valor = imageDataURL;
                }
                else
                {
                    rpta.codigo = "-1";
                }
            }
            catch (Exception ex)
            {
                rpta.codigo = "-1";
            }
            return Json(rpta, new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}
