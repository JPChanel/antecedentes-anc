using System.Xml.Serialization;

namespace ANC.WebApi.Services
{

    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class WSEnvelope
    {
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public WSBody Body { get; set; }
    }

    public class WSBody
    {
        [XmlElement(ElementName = "enviarEmailResponse", Namespace = "http://ws.emailWS.pj.gob.pe")]
        public EnviarEmailResponse EnviarEmailResponse { get; set; }
    }
    public class EnviarEmailResponse
    {
        [XmlElement(ElementName = "responseEnviarEmail", Namespace = "")]
        public ResponseEnviarEmail ResponseEnviarEmail { get; set; }
    }
    public class ResponseEnviarEmail
    {
        [XmlElement(ElementName = "mensaje", Namespace = "")]
        public WSMensaje Mensaje { get; set; }
    }
    public class WSMensaje
    {
        [XmlElement(ElementName = "codigo", Namespace = "")]
        public string Codigo { get; set; }
        [XmlElement(ElementName = "descripcion", Namespace = "")]
        public string Descripcion { get; set; }
    }



}
