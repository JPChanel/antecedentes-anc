
namespace ANC.Entidades
{
    public class JWTBE
    {
        public string _token { get; set; }
    }

    public class TramiteDocumentarioBE: AntecedentesBE
    {
        public string tamanio_cedula { get; set; }
        public string tamanio_formulario { get; set; }
        public string correo { get; set; }
        public string nro_celular { get; set; }
        public int tipo_antecedente { get; set; }
        public int cod_intmp { get; set; }
        public int cod_mpdf_cedula { get; set; }
        public int cod_mpdf_formulario { get; set; }
        public string accion { get; set; }

        //extras para documento

        public int cod_orig_antec { get; set; }
        public int cod_distrito_ftp { get; set; }
        public string des_departamento { get; set; }
        public string usuario { get; set; }
        public string des_provincia { get; set; }
        public string des_distrito { get; set; }
        public byte[] firma_user { get; set; }
        public string ruta_logo { get; set; }
        public string ruta_escudo { get; set; }
        public string ruta_logo_pj { get; set; }

        public string rs_registro_mp { get; set; }
    }
    public class RptTramiteDocumentarioBE:RptPGBE
    {
        public string rs_registro_mp { get; set; }
        public string rs_anio_mp { get; set; }
        public string rs_ruta_cedula { get; set; }
        public string rs_ruta_formulario { get; set; }


        public string rs_destinatario { get; set; }
        public string rs_fec_doc  { get; set; }
        public string rs_documento { get; set; }
        public string rs_fojas_inicial { get; set; }
        public string rs_distrito_judicial { get; set; }
        public string rs_fec_recepcion { get; set; }
        public string rs_contenido { get; set; }
        public string rs_remitente { get; set; }

        public int rn_cod_intmp { get; set; }
        public int rn_cod_mpdf_cedula { get; set; }
        public int rn_cod_mpdf_formulario { get; set; }


        public string[] ruta_documentos { get; set; }
        public string[] tipo_documentos { get; set; }
    }

    public class MantRegistroPGBE
    {
        public int cod_intmp { get; set; }
        public int cod_intmp_rpta { get; set; }
        public int cod_intper { get; set; }
        public string nro_documento { get; set; }
        public string nro_registro { get; set; }
        public string anio { get; set; }

        public string nombre_oficio { get; set; }

        public int[] cod_antec { get; set; }
        public int[] cod_tipantec { get; set; }
        public string[] correo { get; set; }
        public string[] celular { get; set; }

        public string[] casilla { get; set; }


        public int cod_origantec { get; set; }
        public string cod_usuario { get; set; }
        public string accion { get; set; }
    }
    public class AntecedentesBE
    {
        public string num_documento { get; set; }
        public string ap_paterno { get; set; }
        public string ap_materno { get; set; }
        public string nombres { get; set; }
    }
    public class AntecedentesValidacionBE
    {
        public string cbTipoDocumento { get; set; }
        public string txtNumDoc { get; set; }
        public DateTime FecEmisionDoc { get; set; }
       
        public DateTime FecNacimiento { get; set; }
        public string txtnombrePadre { get; set; }
        public string txtnombreMadre { get; set; }

        public string txtUbigeo { get; set; }
        public string txtCodValid { get; set; }
   
        
        public string cbNacidoPeruano { get; set; }
        public string cbDepartamento { get; set; }
        public string cbProvincia { get; set; }
        public string cbDistrito { get; set; }
        public string txtTelefono { get; set; }
        public string txtEmail { get; set; }
        public string txtEmailconfirm { get; set; }
        public string textCaptcha { get; set; }

     
    }
    public class TipoReporteBE
    {
        public string _radioTipoRepo1 { get; set; }
        public string _radioTipoRepo2 { get; set; }

    }

    public class SesionAntecedentesBE
    {
        public string cod_usuario { get; set; }
        public int cod_intper { get; set; }
        public int cod_tipo_repo { get; set; }
        public string des_tip_repo { get; set; }
        public int cod_subtipo_repo { get; set; }
        public int cod_origantec { get; set; }
        public int cod_distrito_ftp { get; set; }
        public string id_sesion { get; set; }


        public string[] ruta_doc_ftp { get; set; }
        public string[] tipo_documentos { get; set; }


        public int n_cod_intmp { get; set; }
        
        public string nro_registro { get; set; }
        public string anio_td { get; set; }


        public string cod_tipodoc { get; set; }
        public string num_documento { get; set; }
        public  string nombre_doc { get; set; }
        public string ap_paterno { get; set; }
        public string ap_materno { get; set; }
        public string nombres { get; set; }
        public DateTime? fec_nacimiento { get; set; }
        public DateTime? fec_emisiondoc { get; set; }

      
        public string coddepartamento { get; set; }
        public string codprovincia { get; set; }
        public string coddistrito { get; set; }
        public string des_departamento { get; set; }
        public string des_provincia { get; set; }
        public string des_distrito { get; set; }


        public string correo { get; set; }

        public string[] as_correos { get; set; }

        public string correopj { get; set; }
        public string telefono { get; set; }
        public string[] as_celular { get; set; }
        
        public string direccion_ip { get; set; }
        public string mac_address { get; set; }
        public string computer_name { get; set; }
        public string sistema_operativo { get; set; }

        public byte[] foto { get; set; }
        public byte[] firma_reniec { get; set; }
        public string ruta_logo { get; set; }
        public string fondo_certificado { get; set; }


        public int cod_tipmovantec { get; set; }
        public int rn_cod_antec { get; set; }
        public string rs_cod_antec { get; set; }
        public int[] cod_antec { get; set; }
        public string nro_tramite { get; set; }
        public string fec_expedido { get; set; }
        public string fec_caducidad { get; set; }
        public int rn_fla_vigencia { get; set; }
        public int rn_fla_generado { get; set; }
        //para busqueda
        public int tipo_consulta { get; set; }

        public int cod_estado_validacion { get; set; }

        public string ruta_certificado { get; set; }
        public string password_certificado { get; set; }
        public string ruta_imagen_firma { get; set; }

        public string des_error { get; set; }
        public int estado_error { get; set; }
    }
    public class ExpedienteAntecedenteBE: RptBE
    {
        public string nro_dni { get; set; }
        public string primer_apellido { get; set; }
        public string segundo_apellido { get; set; }
        public string nombres { get; set; }
        public byte[] foto { get; set; }
     
    }
    public class AuditoriaAntecedentesBE
    {
        public string cod_usuario { get; set; }
        public int cod_intper { get; set; }
        public int cod_antec { get; set; }
        public string des_mov_antec { get; set; }
        public string num_documento { get; set; }
        public string ap_paterno { get; set; }
        public string ap_materno { get; set; }

        public string nombres { get; set; }
    
        public int cod_tipmovantec { get; set; }

        public int cod_origantec { get; set; }
        public string des_auditoria { get; set; }
        public string id_sesion { get; set; }

        public string direccion_ip { get; set; }
        public string mac_address { get; set; }
        public string computer_name { get; set; }
        public string sistema_operativo { get; set; }
      


    }

    public class ExpedienteSancionesBE : RptBE
    {


        public string orden { get; set; }
        public string principal { get; set; }
        

        public string origen { get; set; }
        public string expediente { get; set; }
        public string instancia { get; set; }
        public string sancion { get; set; }
        public DateTime fecha { get; set; }
        
        public string estado { get; set; }
       
        public string observaciones { get; set; }
        //adicional record expedientes
        public DateTime fec_apertura { get; set; }
        public string encargado { get; set; }

        public string resolucion { get; set; }
        public string cod_estado_expe { get; set; }
        public string cod_estado_reso { get; set; }
        //fin
        public string cod_distrito_origen { get; set; }
        public string cod_intexp { get; set; }
        public string cod_intper { get; set; }
        public string num_secuen { get; set; }

        public string cod_resinf { get; set; }
        public string cod_estado { get; set; }
        public string cod_encarg { get; set; }
        public string num_expedi { get; set; }
        public string num_anios { get; set; }
        public string des_tipexp { get; set; }
        public string des_motivo { get; set; }
        public string detalle_motivo { get; set; }
        public string expe_rel { get; set; }
        public string tipo_rel { get; set; }
        public string fla_tipo { get; set; }


    }
    public class PiePagAntecedenteSancionesBE : RptBE
    {
        public string notas { get; set; }
    }
    public class RptVerificarDocBE: RptBE
    {
        public int vd_id { get; set; }
        public string vd_codigo { get; set; }
        public string vd_clave { get; set; }
        public string ruta_ftp_original { get; set; }
        public string ruta_ftp_publicado { get; set; }

    }
 
    public class ActualizarVerificaDocBE
    {
        public int an_vd_id { get; set; }
        public int an_cod_antec { get; set; }
        public string as_usuario { get; set; }

    }
    public class VerificaIngresoBE
    {
        public string nro_documento { get; set; }
        public string as_direccion_ip { get; set; }  

    }
    public class VerificaAntecedentesBE
    {
        public string as_cod_usuario { get; set; }
        public int an_cod_intper { get; set; }
        public string as_ap_paterno { get; set; }
        public string as_ap_materno { get; set; }
        public string as_nombres { get; set; }
        public string as_num_documento { get; set; }
        public int an_cod_tipantec { get; set; }
        public int an_cod_subtipantec { get; set; }
        public int an_cod_origantec { get; set; }
        public string as_nombre_doc { get; set; }
        public int an_n_vd_id { get; set; }

        public string[] as_correo { get; set; }
        public string[] as_celular { get; set; }


        public int[] an_orden { get; set; }
        public int[] an_principal { get; set; }
        public string[] as_origen { get; set; }
        public string[] as_expediente { get; set; }
        public string[] as_instancia { get; set; }
        public string[] as_sancion { get; set; }
        public DateTime[] ad_fecha { get; set; }
        public string[] as_estado { get; set; }
        public string[] as_observaciones { get; set; }
        public DateTime[] ad_fec_apertura { get; set; }
        public string[] as_encargado { get; set; }
        public string[] as_resolucion { get; set; }
        public int[] an_cod_distrito_origen { get; set; }
        public int[] an_cod_intexp { get; set; }
        public int[] an_num_secuen { get; set; }
        public int[] an_cod_resinf { get; set; }
        public int[] an_cod_estado_expe { get; set; }
        public int[] an_cod_estado_reso { get; set; }
        public int[] an_cod_encarg { get; set; }
        public string[] as_num_expedi { get; set; }
        public string[] as_num_anios { get; set; }
        public string[] as_des_tipexp { get; set; }
        public string[] as_des_motivo { get; set; }
        public string[] as_detalle_motivo { get; set; }
        public string[] as_expe_rel { get; set; }
        public string[] as_tipo_rel { get; set; }
        public string[] as_fla_tipo { get; set; }

        public string as_id_sesion { get; set; }
        public string as_direccion_ip { get; set; }
        public string as_mac_address { get; set; }
        public string as_computer_name { get; set; }
        public string as_sistema_operativo { get; set; }
        public string as_accion { get; set; }
    }

    public class RptVerificacionBE : RptBE
    {
     
        public int rn_codigo { get; set; }
        public string rs_ruta_ftp { get; set; }
        public string rs_valor { get; set; }
        public int cod_intper { get; set; }
        public int rn_cod_antec { get; set; }
        public DateTime rd_fec_vigencia { get; set; }
        public DateTime rs_fec_creacion { get; set; }
        public string rs_nro_registro { get; set; }
        public string rs_anio { get; set; }
        public int cod_distri { get; set; }
       
    }

    public class VerificacionTDAntecedentesBE
    {

    }
    public class RecordSancionesBE : RptBE
    {
        public string amonestaciones { get; set; }
        public string multas { get; set; }
        public string suspensiones { get; set; }
        public string destituciones { get; set; }
        public string sanciones { get; set; }


    }
    public class RecordExpedientesBE : RptBE
    {

        public string quejas { get; set; }
        public string investigaciones { get; set; }
        public string visitas { get; set; }
        public string medidas_cautelares { get; set; }
        public string archivados { get; set; }
        public string en_tramite { get; set; }
        public string rehabilitaciones { get; set; }
        public string expedientes { get; set; }
        public string sanciones { get; set; }
    }



    public class AcuseCorreoBE: RptPGBE
    {

        public int cod_notif_antec { get; set; }
        public string correo_destino { get; set; }
        public string correo_coo { get; set; }
        public string[] ruta_archivo { get; set; }
        public string[] tipo_documentos { get; set; }

        public string usu_envio { get; set; }

        public string nombres { get; set; }
        public string registro_td { get; set; }
        public string[] regitro_antec { get; set; }
        public int cod_intmp { get; set; }

        public int[] cod_antec { get; set; }
        public int cod_distrito { get; set; }
        public string accion { get; set; }



        public string ipPc { get; set; }
        public string pcNameipPc { get; set; }
        public string usuarioSis { get; set; }
        public string usuarioRed { get; set; }
        public string nombreSo { get; set; }
        public string macadress { get; set; }
        public string asunto { get; set; }


        public DateTime adt_fec_envio { get; set; }
        public DateTime adt_fec_apertura { get; set; }
        public string estado_envio { get; set; }

        public string token_generico { get; set; }



    }
    public class RptListaResponsableBE
    {
        public string rs_abreviatura_responsable { get; set; }

        public string rs_nombre_responsable { get; set; }
        public string rs_cargo_responsable { get; set; }
        public string rs_entidad_primera_linea { get; set; }
        public string rs_entidad_segunda_linea { get; set; }
        public byte[] rbt_firma { get; set; }
    }
    public class ConsultaAntecedenteBE {



        public string num_documento { get; set; }

        public string cbTipoDocumento { get; set; }
        public int cod_intper { get; set; }
     
        public string nro_registro { get; set; }
        public string anio { get; set; }
        public int tipo_consulta { get; set; }
        public int cod_tipantec { get; set; }
        public int cod_subtipantec { get; set; }
        public int cod_origantec { get; set; }
        public int fla_vigencia { get; set; }

        public string textCaptcha { get; set; }

        public int cod_antec { get; set; }
        public int an_tipmov_antec { get; set; }
        public string des_movantec { get; set; }
    }

    public class RptConsutaAntecedenteBE: RptBE
    {
        public int rn_cod_antec { get; set; }
        public string rs_nro_registro { get; set; }
        public string rs_anio { get; set; }
        public int rn_cod_tipantec { get; set; }
        public string rs_des_tipantec { get; set; }
        public int rn_cod_subtipantec { get; set; }
        public string rs_des_subtipantec { get; set; }

        public int rn_cod_origantec { get; set; }
        public string rs_des_origantec { get; set; }
        public string rs_nombre_doc { get; set; }
       
        public int rn_vd_id { get; set; }
        public int rn_fla_vigencia { get; set; }
        public string rs_des_vigencia { get; set; }

        public DateTime rd_fec_vigencia { get; set; }
        public DateTime rd_fec_generacion { get; set; }

        public int rn_cod_intmp { get; set; }

        public string rs_nro_registro_td { get; set; }
        public string rs_anio_td { get; set; }
        public int rn_fla_generado { get; set; }
        public string rs_des_generado { get; set; }
        public int rn_fla_existe { get; set; }
        public string rs_des_existe { get; set; }
        public string rs_nombre_oficio { get; set; }
    }

    public class RespuestaReporteBE
    {
        public List<byte[]> ArchivosPDF { get; set; }
        public string MensajeError { get; set; }
        public string[] tipo_documentos { get; set; }
    }
    public class RespuestaArchivoBE: RptBE
    {
        public byte[] ArchivosPDF { get; set; }
        public string fec_vigencia { get; set; }
        public string fec_expedicion { get; set; }
        public string nro_registro { get; set; }
        public string anio { get; set; }
    }
    public class ListAnioBE
    {

        public int rn_anio { get; set; }
        public string rs_denominacion { get; set; }


    }
    public class RptPGBE
    {
        public int rn_codigo { get; set; }

        public string rs_valor { get; set; }
    }
    public class RptBE
    {

        public int codigo { get; set; }
        public string valor { get; set; }
       

    }

    
}
