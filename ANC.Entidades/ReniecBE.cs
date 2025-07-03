using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANC.Entidades
{
    public class ReniecBE
    {
        public string codigo { get; set; }
        public string descripcion { get; set; }

        public string numdni { get; set; }
        public string digverifica { get; set; }
        public string apaterno { get; set; }
        public string amaterno { get; set; }
        public string apcasada { get; set; }
        public string nombres { get; set; }
        public string sexo { get; set; }
        public DateTime fechanac { get; set; }
        public DateTime? fechafallecimiento { get; set; }
        public string estadocivil { get; set; }
        public string estadocivil_detalle { get; set; }
        public string estatura { get; set; }
        public string departamentoubi { get; set; }
        public string departamentoubi_detalle { get; set; }
        public string provinciaubi { get; set; }
        public string provinciaubi_detalle { get; set; }
        public string distritoubi { get; set; }
        public string distritoubi_detalle { get; set; }
        public string departamentonac { get; set; }
        public string departamentonac_detalle { get; set; }
        public string provincianac { get; set; }
        public string provincianac_detalle { get; set; }
        public string distritonac { get; set; }
        public string distritonac_detalle { get; set; }
        public string localidadnac_detalle { get; set; }
        public string localidadubi_detalle { get; set; }
        public byte[] foto { get; set; }
        public byte[] firma { get; set; }
        //adicional perzonalizado

        public string cod_ubigeonac { get; set; }
        public string cod_ubigeodomicilio { get; set; }
        //DIRECCION
        public string direccion { get; set; }
        public string pre_direccion { get; set; }
        public string num_direccion { get; set; }
        public string pref_block { get; set; }
        public string block { get; set; }
        public string pref_des { get; set; }
        public string interior { get; set; }
        public string urbaniza { get; set; }
        public string etapa { get; set; }
        public string manzana { get; set; }
        public string lote { get; set; }

        //GRADO
        public string grado_instruccion { get; set; }
        public string tipdoc_sustento { get; set; }
        public string doc_sustento { get; set; }


        //DATOS PADRE - MADRE
        public string nombre_padre { get; set; }
        public string tipdoc_padre { get; set; }
        public string doc_padre { get; set; }
        public string nombre_madre { get; set; }
        public string tipdoc_madre { get; set; }
        public string doc_madre { get; set; }


        //FECHAS
        public DateTime? fecha_inscripcion { get; set; }
        public DateTime? fecha_expedicion { get; set; }
        public DateTime? fecha_caducidad { get; set; }


        //OTROS
        public string cons_votacion { get; set; }
        public string cons_restricciones { get; set; }
        public string cons_reservado { get; set; }


        //Otros campos PERSONALES
        public int codpersonal { get; set; }
        public string movil { get; set; }
        public string correopj { get; set; }
        public string correo { get; set; }

        //Otros campos LABORALES
        public int codregimenlab { get; set; }
        public int codcargo { get; set; }
        public int codcondicionlab { get; set; }
        public int coddistritojud { get; set; }
        public int codciudad { get; set; }
        public int coddependencia { get; set; }

        //Todo jalo OK
        public string codigo2 { get; set; }


        //DATOS ADICIONAL PERSONAL
        public int COD_REGIMENLABORAL { get; set; }
        public int COD_CARGO { get; set; }
        public int COD_ESTADOLABORAL { get; set; }
        public int COD_DISTRITO { get; set; }
        public int COD_CIUDAD { get; set; }
        public int COD_AREA { get; set; }


    }
    #region Dedicad
    public class responseAuth
    {
        public bool success { get; set; }
        public string message { get; set; }
        public tokenBE obj { get; set; }


    }
    public class tokenBE
    {
        public string token { get; set; }
    }
    public class paramsApiReniec
    {
        public int cod_distrito { get; set; }
        public string modulo { get; set; }
        public RequestReniecModel datosConsulta { get; set; }


    }

    public class RequestReniecModel
    {
        public string formatoRespuesta { get; set; }
        public string consultante { get; set; }
        public string motivo { get; set; }
        public PersonaConsultada personaConsultada { get; set; }
        public Pagination pagination { get; set; }
        public AuditoriaReniecDedicada auditoria { get; set; }
    }

    public class PersonaConsultada
    {
        public string tipoConsulta { get; set; }
        public string nroDocumentoIdentidad { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string nombres { get; set; }
        public string nroDocumentoIdentidadApoderado { get; set; }
        public string vinculoApoderado { get; set; }
    }

    public class Pagination
    {
        public int size { get; set; }
        public int page { get; set; }
    }

    public class AuditoriaReniecDedicada
    {
        public string usuario { get; set; }
        public string nombrePc { get; set; }
        public string numeroIp { get; set; }
        public string direccionMac { get; set; }
    }


    public class ResponseReniecModelType2 : BaseResponseReniec
    {
        public Datos data { get; set; }
    }
    public class Datos
    {

        public string nroDocumentoIdentidad { get; set; }
        public string codigoVerificacion { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string apellidoCasado { get; set; }
        public string nombres { get; set; }
        public string codigoUbigeoDepartamentoDomicilio { get; set; }
        public string codigoUbigeoProvinciaDomicilio { get; set; }
        public string codigoUbigeoDistritoDomicilio { get; set; }
        public string codigoUbigeoLocalidadDomicilio { get; set; }
        public string departamentoDomicilio { get; set; }
        public string provinciaDomicilio { get; set; }
        public string distritoDomicilio { get; set; }
        public string localidadDomicilio { get; set; }
        public string estadoCivil { get; set; }
        public string gradoInstruccion { get; set; }
        public string estatura { get; set; }
        public string sexo { get; set; }
        public string documentoSustentatorioTipoDocumento { get; set; }
        public string documentoSustentatorioNroDocumento { get; set; }
        public string codigoUbigeoDepartamentoNacimiento { get; set; }
        public string codigoUbigeoProvinciaNacimiento { get; set; }
        public string codigoUbigeoDistritoNacimiento { get; set; }
        public string codigoUbigeoLocalidadNacimiento { get; set; }
        public string departamentoNacimiento { get; set; }
        public string provinciaNacimiento { get; set; }
        public string distritoNacimiento { get; set; }
        public string localidadNacimiento { get; set; }
        public string fechaNacimiento { get; set; }
        public string documentoPadreTipDocumento { get; set; }
        public string documentoPadreNumDocumento { get; set; }
        public string nombrePadre { get; set; }
        public string documentoMadreTipoDocumento { get; set; }
        public string documentoMadreNumeroDocumento { get; set; }
        public string nombreMadre { get; set; }
        public string fechaInscripcion { get; set; }
        public string fechaEmision { get; set; }
        public string fechaFallecimiento { get; set; }
        public string constanciaVotacion { get; set; }
        public string fechaCaducidad { get; set; }
        public string restricciones { get; set; }
        public string prefijoDireccion { get; set; }
        public string direccion { get; set; }
        public string nroDireccion { get; set; }
        public string blockOChalet { get; set; }
        public string interior { get; set; }
        public string urbanizacion { get; set; }
        public string etapa { get; set; }
        public string manzana { get; set; }
        public string lote { get; set; }
        public string preBlockOChalet { get; set; }
        public string preDptoPisoInterior { get; set; }
        public string preUrbCondResid { get; set; }
        public string reservado { get; set; }
        public int longitudFoto { get; set; }
        public int longitudFirma { get; set; }
        public int reservadoFotoFirma1 { get; set; }
        public string reservadoFotoFirma2 { get; set; }
        public string foto { get; set; }
        public string firma { get; set; }
    }

    public class ResponseReniecModelType1 : BaseResponseReniec
    {
        public DatosPersona data { get; set; }
    }

    public class DatosPersona
    {
        public List<Personas> personas { get; set; }
    }
    public class Personas
    {

        public string nroDocumentoIdentidad { get; set; }
        public string codigoVerificacion { get; set; }
        public string tipoFichaRegistral { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string nombres { get; set; }
        public string datos { get; set; }
        public string flagImagen { get; set; }

    }

    public class BaseResponseReniec
    {
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public string codigoOperacion { get; set; }
    }
    #endregion dedicada

}