using System.Xml.Serialization;

namespace ANC.WebApi.Services
{

    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(Envelope));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (Envelope)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "consultaReniecResponse", Namespace = "http://www.pj.gob.pe/consultaReniecService")]
    public class ConsultaReniecResponse
    {

        [XmlElement(ElementName = "res_trama", Namespace = "http://www.pj.gob.pe/consultaReniecService")]
        public object ResTrama;

        [XmlElement(ElementName = "res_codigo", Namespace = "http://www.pj.gob.pe/consultaReniecService")]
        public string ResCodigo;

        [XmlElement(ElementName = "res_descripcion", Namespace = "http://www.pj.gob.pe/consultaReniecService")]
        public string ResDescripcion;

        [XmlElement(ElementName = "res_totalRegistros", Namespace = "http://www.pj.gob.pe/consultaReniecService")]
        public object ResTotalRegistros;

        [XmlElement(ElementName = "res_persona", Namespace = "http://www.pj.gob.pe/consultaReniecService")]
        public string ResPersona;

        [XmlElement(ElementName = "res_foto", Namespace = "http://www.pj.gob.pe/consultaReniecService")]
        public byte[] ResFoto;

        [XmlElement(ElementName = "res_firma", Namespace = "http://www.pj.gob.pe/consultaReniecService")]
        public byte[] ResFirma;

        [XmlElement(ElementName = "res_listaPersonas", Namespace = "http://www.pj.gob.pe/consultaReniecService")]
        public string ResListaPersonas;

        [XmlAttribute(AttributeName = "NS1", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string NS1;

        [XmlText]
        public string Text;
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Body
    {

        [XmlElement(ElementName = "consultaReniecResponse", Namespace = "http://www.pj.gob.pe/consultaReniecService")]
        public ConsultaReniecResponse ConsultaReniecResponse;

        [XmlElement(ElementName = "administrarUsuarioResponse", Namespace = "http://ws.usuariosReniecWS.pj.gob.pe")]
        public AdministrarUsuarioResponse AdministrarUsuarioResponse { get; set; }
    }

    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope
    {

        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Body Body;

        [XmlAttribute(AttributeName = "soa-penv", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Soapenv;

        [XmlText]
        public string Text;
    }


    //administrativo
    //[XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    //public class Envelope
    //{
    //    [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    //    public Body Body { get; set; }
    //}

 

    public class AdministrarUsuarioResponse
    {
        [XmlElement(ElementName = "responseAdministrarUsuario", Namespace = "")]
        public ResponseAdministrarUsuario ResponseAdministrarUsuario { get; set; }
    }

    public class ResponseAdministrarUsuario
    {
        [XmlElement(ElementName = "mensaje", Namespace = "")]
        public Mensaje Mensaje { get; set; }

        [XmlElement(ElementName = "detalle", Namespace = "")]
        public Detalle Detalle { get; set; }
    }

    public class Mensaje
    {
        [XmlElement(ElementName = "codigo", Namespace = "")]
        public string Codigo { get; set; }

        [XmlElement(ElementName = "descripcion", Namespace = "")]
        public string Descripcion { get; set; }
    }

    public class Detalle
    {
        [XmlElement(ElementName = "nroDocumento", Namespace = "")]
        public string NroDocumento { get; set; }

        [XmlElement(ElementName = "msjRespuesta", Namespace = "")]
        public string MsjRespuesta { get; set; }

        [XmlElement(ElementName = "valNroDocumento", Namespace = "")]
        public string ValNroDocumento { get; set; }

        [XmlElement(ElementName = "valNombres", Namespace = "")]
        public string ValNombres { get; set; }

        [XmlElement(ElementName = "valApePaterno", Namespace = "")]
        public string ValApePaterno { get; set; }

        [XmlElement(ElementName = "valApeMaterno", Namespace = "")]
        public string ValApeMaterno { get; set; }

        [XmlElement(ElementName = "valFecNacimiento", Namespace = "")]
        public string ValFecNacimiento { get; set; }
    }

}
