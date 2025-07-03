using ANC.Comun;
using ANC.Comun.Log4net;
using ANC.Entidades;
using ANC.LogicaNegocio;
using ANC.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using static System.Net.WebRequestMethods;
namespace ANC.WebApi.Servicio_Email
{
    public class ServicioEmail
    {
        private readonly IConfiguration _configuration;

        public static string ecodigoCLiente = "";
        public static string ecodigoAplicativo = "";
        public static string ecodigoRol = "";
        public static string emisorEmailUsis = "";
        public static string emisorEmail = "";
        public static string tipoalerta = "";
        string endpointUrl = "";
        string username = "";
        string password = "";
        GenericoLN comuln = new GenericoLN();
        Funciones _funciones = new Funciones();
        public static string anio = DateTime.Now.Year.ToString();
   

        public ServicioEmail(IConfiguration configuration)
        {
            _configuration = configuration;

            endpointUrl = _configuration.GetSection("WSEmail")["EndpointUrl"];
            username = _configuration.GetSection("WSEmail")["Username"];
            password = _configuration.GetSection("WSEmail")["Password"];

            ecodigoCLiente = _configuration.GetValue<string>("WS_CLIENTE");
            ecodigoAplicativo = _configuration.GetValue<string>("WS_CODAPLIC");
            ecodigoRol = _configuration.GetValue<string>("WS_CODROL");

            emisorEmailUsis = _configuration.GetValue<string>("EMAIL_CORREOEMISOR_USIS");
            emisorEmail = _configuration.GetValue<string>("EMAIL_CORREOEMISOR");
            tipoalerta = _configuration.GetValue<string>("TIPO_ALERTA");
        }


        public async Task<RptBE> EnviarCorreoWS(WSEmailBE email)
        {
            RptBE respuesta = new RptBE();
            try
            {
                if(string.IsNullOrWhiteSpace(email.titulo)) {
                    respuesta.codigo = -1;
                    respuesta.valor = "Ingrese el Titulo del Correo Electronico.";
                    return respuesta;
                }
                if ( string.IsNullOrWhiteSpace(email.motivoEmail))
                {
                    respuesta.codigo = -1;
                    respuesta.valor = "Ingrese el Motivo del Correo Electronico.";
                    return respuesta;
                }

                bool _validDestinatario = _funciones.ValidarLongitudesIguales(email.destinatarioEmail, email.destinatarioTipo);
               

                if (!_validDestinatario)
                {
                    respuesta.codigo = -1;
                    respuesta.valor = "Cantidad de destinatarios y tipos de destinatarios no conincide.";
                    return respuesta;
                }
                bool _validArraydestinatario = _funciones.ValidarNullOEmptyArray(email.destinatarioEmail, email.destinatarioTipo);
                if (!_validArraydestinatario)
                {
                    respuesta.codigo = -1;
                    respuesta.valor = "Debe ingresar al menos un destinatario.";
                    return respuesta;
                }

                bool _validDocumento = _funciones.ValidarLongitudesIguales(email.nombreDocumento, email.extensionDocumento);
              

                if (!_validDocumento)
                {
                    respuesta.codigo = -1;
                    respuesta.valor = "Cantidad de nombre de documento, extensión de documento y binario de documento no conincide.";
                    return respuesta;
                }
                if(email.binarioDocumento != null)
                {
                    if (email.binarioDocumento.Length != email.nombreDocumento.Length)
                    {
                        respuesta.codigo = -1;
                        respuesta.valor = "Cantidad de nombre de documento, extensión de documento y binario de documento no conincide.";
                        return respuesta;
                    }
                }
                    
                string ipPc = (!String.IsNullOrEmpty(email.ipPc)) ? email.ipPc : "172.18.4.20";
                string macAddressPc = "00:00:00:00:00:00";
                string pcName = (!String.IsNullOrEmpty(email.pcNameipPc)) ? email.pcNameipPc : "SRVOCMA_WEBZ-JNJ";
                string usuarioSis = (!String.IsNullOrEmpty(email.usuarioSis)) ? email.usuarioSis : "ANC";
                string usuarioRed = (!String.IsNullOrEmpty(email.usuarioRed)) ? email.usuarioRed : "ANC";
                string nombreSo = (!String.IsNullOrEmpty(email.nombreSo)) ? email.nombreSo : "WINDOWS 10";

                /*Cuerpo del mensaje*/

                string cuerpoHtml = "<body style='margin:0;padding:0;'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tr><td style='padding:10px 0 30px 0;'><table align='center' border='0' cellpadding='0' cellspacing='0' width='600' style='border:1px solid #cccccc;border-collapse:collapse;'><tr><td align='center' bgcolor='#262b3e' style='padding:25px 0;color:#153643;font-size:28px;font-weight:bold;font-family:Arial,sans-serif;'><img src='https://anc.pj.gob.pe/erpanc/images/h5.png' width='200' height='200' style='display:block;' /><br><b style='margin:40px;color:#fff;font-family:Arial,sans-serif;font-size:26px;'>ERP ANC</b><br><b style='color:#fff;font-family:Arial,sans-serif;font-size:20px;'> EMAIL</b></td></tr><tr><td bgcolor='#ffffff' style='padding:40px 30px 40px 30px;'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tr><td style='color:#153643;font-family:Arial,sans-serif;font-size:24px;'><b>Estimado Señor/a:  " + email.nombreCompleto +"</b></td></tr><tr><td style='padding:20px 0 30px 0;color:#153643;font-family:Arial,sans-serif;font-size:16px;line-height:20px;'><p>" + email.TextoHtml + "</p></td></tr></table></td></tr><tr><td bgcolor='#dddddd' style='padding:30px 30px 30px 30px;'><table border='0' cellpadding='0' cellspacing='0' width='100%'><tr><td style='color:#000;font-family:Arial,sans-serif;font-size:14px;' width='85%'>&reg; ERP ANC - " + anio + "<br />Unidad de Tecnologia de la Información - <a href='https://anc.pj.gob.pe/' style='color:#ffffff;'><font color='#000'>ANC</font></a></td><td align='right' width='25%'><table border='0' cellpadding='0' cellspacing='0'><tr><p  style='font-family:Arial,sans-serif;color:#000;font-weight:600;padding:0px 0 10px;font-size:14px;padding-right:40px'> SÍGUENOS EN </p></tr></table><table border='0' cellpadding='0' cellspacing='0'><tr><td style='font-family:Arial,sans-serif;font-size:12px;font-weight:bold;'><a href='https://www.facebook.com/ANCPJ' style='color:#ffffff;'><img src='https://anc.pj.gob.pe/erpanc/images/redes/facebook.png' alt='Facebook' width='38' height='38' style='display:block;background:white;border-radius:50%;' border='0' /></a></td><td style='font-family:Arial,sans-serif;font-size:12px;font-weight:bold;'><a href='https://twitter.com/ANC_PJ_Oficial' style='color:#ffffff;'><img src='https://anc.pj.gob.pe/erpanc/images/redes/twitter.png' alt='Twitter' width='38' height='38' style='display:block;background:white;border-radius:50%;' border='0' /></a></td><td style='font-family:Arial,sans-serif;font-size:12px;font-weight:bold;'><a href='https://www.instagram.com/anc_pj_oficial' style='color:#ffffff;'><img src='https://anc.pj.gob.pe/erpanc/images/redes/instagram.png' alt='instagram' width='38' height='38' style='display:block;background:white;border-radius:50%;' border='0' /></a></td><td style='font-family:Arial,sans-serif;font-size:12px;font-weight:bold;'><a href='https://www.youtube.com/@ANC_PJ_Oficial' style='color:#ffffff;'><img src='https://anc.pj.gob.pe/erpanc/images/redes/youtube.png' alt='youtube' width='38' height='38' style='display:block;background:white;border-radius:50%;' border='0' /></a></td><td style='font-family:Arial,sans-serif;font-size:12px;font-weight:bold;'><a href='https://www.tiktok.com/@anc_pj_oficial' style='color:#ffffff;'><img src='https://anc.pj.gob.pe/erpanc/images/redes/tik-tok.png' alt='tik-tok' width='38' height='38' style='display:block;background:white;border-radius:50%;' border='0' /></a></td></tr></table><table border='0' cellpadding='0' cellspacing='0'><tr><p align='center' style='font-family:Arial,sans-serif;color:#000;padding:5px 0px 0px;font-size:14px;'>Descarga Nuestra App Servicios ANC - PJ </p></tr></table><table border='0' cellpadding='0' cellspacing='0'><tr><td style='font-family:Arial,sans-serif;font-size:12px;font-weight:bold;padding-right:40px'><a href='https://play.google.com/store/apps/details?id=com.anc.quejas.client' target='_blank'><img alt='Disponible en Google Play' src='https://play.google.com/intl/es-419/badges/static/images/badges/es-419_badge_web_generic.png' width='100' height='50' /></a></td></tr></table></td></tr></table></td></tr><tr><td style='padding:25px 25px 25px 25px;font-family:Arial,sans-serif;font-size:12px;font-style:normal;text-align:justify;font-weight:700;'><b style='font-weight:bolder;'>IMPORTANTE:</b> Este correo ha sido automáticamente generado, <b style='color:red;text-decoration:underline red;font-weight:bolder;'>la dirección de correo no se encuentra habilitada para recibir mensajes.</b> Para cualquier otra consulta deberá comunicarse al número 01-4101010 - anexo 11173 o apersonarse a las instalaciones de la Autoridad Nacional de Control del Poder Judicial, también puede visitar la página web: <a href='https://anc.pj.gob.pe' target='_blank'>https://anc.pj.gob.pe</a>.<br><br>Autoridad Nacional de Control del Poder Judicial.<br>Poder Judicial<br>Lima-Perú</td></tr></table></td></tr></table></body>";



                if (string.IsNullOrEmpty(email.cuerpoTexto))
                {
                    email.cuerpoTexto = ".";
                }

                if (!string.IsNullOrEmpty(email.cuerpoHtml))
                {
                    cuerpoHtml = email.cuerpoHtml;
                }


                string tipo = tipoalerta == "D" ? " - Desarrollo" : "";
                email.titulo = email.titulo + tipo;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                var client = new HttpClient();
                {
                    XElement listadoDestinatarios = new XElement("listadoDestinatarios");


                    if (_validArraydestinatario)
                    {
                    
                        // Agregar destinatarios dinámicamente
                        for (int i = 0; i < email.destinatarioEmail.Length; i++)
                        {
                            XElement destinatario = new XElement("destinatario",
                                new XElement("destinatarioEmail", email.destinatarioEmail[i]),
                                new XElement("destinatarioTipo", email.destinatarioTipo[i])  //Tipo de receptor. TO: hacia, CC: Con copia, BCC: Con copia oculta 
                            );
                            listadoDestinatarios.Add(destinatario);
                        }
                    }

                    XElement listadoAdjuntos = new XElement("listadoAdjuntos"); 
                    bool _validArraydocumento = _funciones.ValidarNullOEmptyArray(email.nombreDocumento);

                   

                    if (_validArraydocumento)
                    {                        
                        // Agregar destinatarios dinámicamente
                        for (int i = 0; i < email.nombreDocumento.Length; i++)
                        {
                            XElement adjunto = new XElement("adjunto",
                                new XElement("nombreDocumento", email.nombreDocumento[i]),
                                new XElement("extensionDocumento", email.extensionDocumento[i]),
                                new XElement("binarioDocumento", email.binarioDocumento[i])
                            );
                            listadoAdjuntos.Add(adjunto);
                        }
                    }
                  

                    // Crear un documento XML
                    XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
                    XNamespace ws = "http://ws.emailWS.pj.gob.pe";
                    XNamespace wsse = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
                    XNamespace wsu = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";

                    XDocument xmlDocument = new XDocument(
                        new XDeclaration("1.0", "utf-8", "yes"), // Declaración XML
                        new XElement(soapenv + "Envelope", // Elemento raíz
                            new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                            new XAttribute(XNamespace.Xmlns + "ws", ws),
                            new XElement(soapenv + "Header",
                                new XElement(wsse + "Security",
                                    new XAttribute(soapenv + "mustUnderstand", 1),
                                    new XAttribute(XNamespace.Xmlns + "wsse", wsse),
                                    new XAttribute(XNamespace.Xmlns + "wsu", wsu),
                                    new XElement(wsse + "UsernameToken",
                                        new XAttribute(wsu + "Id", "UsernameToken-1"),
                                        new XElement(wsse + "Username", username),
                                        new XElement(wsse + "Password",
                                            new XAttribute("Type", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"),
                                            password
                                        )
                                    )
                                )
                            ),
                            new XElement(soapenv + "Body",
                                new XElement(ws + "enviarEmail",
                                    new XElement("requestSeguridad",
                                        new XElement("codigoCliente", ecodigoCLiente),
                                        new XElement("codigoAplicativo", ecodigoAplicativo),
                                        new XElement("codigoRol", ecodigoRol)
                                    ),
                                    new XElement("requestAuditoria",
                                        new XElement("ipPc", ipPc),
                                        new XElement("macAddressPc", macAddressPc),
                                        new XElement("pcName", pcName),
                                        new XElement("usuarioSis", usuarioSis),
                                        new XElement("usuarioRed", usuarioRed),
                                        new XElement("nombreSo", nombreSo)
                                    ),
                                    new XElement("requestEnviarEmail",
                                        new XElement("emisorEmail", emisorEmail),
                                            listadoDestinatarios,
                                        new XElement("titulo", email.titulo),
                                        new XElement("cuerpoTexto", email.cuerpoTexto),
                                        new XElement("cuerpoHtml", cuerpoHtml),
                                        listadoAdjuntos,
                                        new XElement("motivoEmail", email.motivoEmail)
                                    )
                                )
                            )
                        )
                    );


                    string xml = xmlDocument.ToString();

                    var requestContent = new StringContent(xml, Encoding.UTF8, "text/xml");
                    var result = await client.PostAsync(endpointUrl, requestContent);


                    var eresponseContent = await result.Content.ReadAsStringAsync();

                    XmlSerializer serializer = new XmlSerializer(typeof(WSEnvelope));
                    using (StringReader reader = new StringReader(eresponseContent))
                    {
                        WSEnvelope envelope = (WSEnvelope)serializer.Deserialize(reader);
                        // Accede a los datos deserializados
                        string rsp_codigo = envelope.Body.EnviarEmailResponse.ResponseEnviarEmail.Mensaje.Codigo;
                        respuesta.valor = envelope.Body.EnviarEmailResponse.ResponseEnviarEmail.Mensaje.Descripcion;
                        respuesta.codigo = (rsp_codigo == "0000") ?0:-1;
                       
                    
                    }

                }
            }
            catch (Exception ex)
            {
                LogAntecedentes.Logger().Error("Error: " + ex.Message);
                respuesta.codigo = -1;
                respuesta.valor = "Error al enviar el correo.";
            }
            return respuesta;
        }

       
    }


}
