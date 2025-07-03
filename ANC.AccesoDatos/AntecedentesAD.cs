using ANC.Comun.Log4net;
using ANC.Conexion;
using ANC.Entidades;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ANC.AccesoDatos
{
    public class AntecedentesAD
    {
        public  OdbcConnection con;
        public NpgsqlConnection con_pg;
        public OdbcConnection con_zav;
        private void connection()
        {
            string constr = ConexionBD.ConnectionString;
            con = new OdbcConnection(constr);
        }

        private async Task<NpgsqlConnection> connection_pg()
        {
            var connection = new NpgsqlConnection(ConexionBD.ConnectionStringPostgres);
            await connection.OpenAsync();
            return connection;
        }

        private void connection_zav()
        {
            string constr = ConexionBD.ConnectionStringZavala;
            con_zav = new OdbcConnection(constr);
        }
        public ExpedienteAntecedenteBE AntecedentesDatosPersonales(string codigo_persona)
        {
            ExpedienteAntecedenteBE rpta = new ExpedienteAntecedenteBE();
            try
            {
                connection();
                con.Open();
                int cod_persona = Int32.Parse(codigo_persona);
                var parameters = new DynamicParameters();
                parameters.Add("AN_CODPERSONA", cod_persona, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                rpta = SqlMapper.Query<ExpedienteAntecedenteBE>(
                con, "OCMAERP.SP_ANTECEDENTES_RECORD_SANCIONES_DATOS_PERSONALES  :AN_CODPERSONA", parameters, commandType: System.Data.CommandType.StoredProcedure).SingleOrDefault();


                con.Close();
            }
            catch (Exception er)
            {
                rpta.codigo = -1;
                rpta.valor = "Error: " + codigo_persona + er.Message;
                con.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }

            return rpta;
        }
        public List<ExpedienteSancionesBE> AntecedentesSanciones(string codigo_persona, int sub_tipo_antecedente)
        {
            List<ExpedienteSancionesBE> lista = new List<ExpedienteSancionesBE>();
            try
            {

                connection();
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("AN_CODPERSONA", Int32.Parse(codigo_persona), System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("ANC_COD_SUBTIPANTEC", sub_tipo_antecedente, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                IList<ExpedienteSancionesBE> rpta = SqlMapper.Query<ExpedienteSancionesBE>(
                   con, "OCMAERP.SP_ANTECEDENTES_RECORD_SANCIONES_HISTORIAL  :AN_CODPERSONA,:ANC_COD_SUBTIPANTEC", parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();
                con.Close();

                lista = rpta.ToList();

            }
            catch (Exception er)
            {
                lista[0].codigo = -1;
                lista[0].valor = "Error: " + er.Message;
                con.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }

            return lista;
        }
        public List<ExpedienteSancionesBE> AntecedentesExpedientes(string codigo_persona, int sub_tipo_antecedente)
        {
            List<ExpedienteSancionesBE> lista = new List<ExpedienteSancionesBE>();
            try
            {

                connection();
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("AN_CODPERSONA", Int32.Parse(codigo_persona), System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("ANC_COD_SUBTIPANTEC", sub_tipo_antecedente, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                IList<ExpedienteSancionesBE> rpta = SqlMapper.Query<ExpedienteSancionesBE>(
                   con, "OCMAERP.SP_ANTECEDENTES_RECORD_EXPEDIENTES_HISTORIAL  :AN_CODPERSONA,:ANC_COD_SUBTIPANTEC", parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();
                con.Close();

                lista = rpta.ToList();


            }
            catch (Exception er)
            {
                lista[0].codigo = -1;
                lista[0].valor = "Error: " + er.Message;
                con.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }

            return lista;
        }

        public RecordSancionesBE RecordSancionesTotal(string codigo_persona, int sub_tipo_antecedente)
        {
            RecordSancionesBE rpta = new RecordSancionesBE();
            try
            {
                connection();
                con.Open();

                var parameters = new
                {
                    AN_CODPERSONA = codigo_persona,
                    AN_COD_SUBTIPANTEC = sub_tipo_antecedente
                };
                rpta = SqlMapper.Query<RecordSancionesBE>(
                   con, "OCMAERP.SP_ANTECEDENTES_RECORD_SANCIONES_TOTALES  :AN_CODPERSONA,:AN_COD_SUBTIPANTEC", parameters, commandType: System.Data.CommandType.StoredProcedure).SingleOrDefault();

                con.Close();

            }
            catch (Exception er)
            {
                rpta.codigo = -1;
                rpta.valor = "Error: " + er.Message;
                con.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }
            return rpta;
        }
        public RecordExpedientesBE RecordExpedientesTotal(string codigo_persona, int sub_tipo_antecedente)
        {
            RecordExpedientesBE rpta = new RecordExpedientesBE();
            try
            {
                connection();
                con.Open();

                var parameters = new
                {
                    AN_CODPERSONA = codigo_persona,
                    AN_COD_SUBTIPANTEC = sub_tipo_antecedente
                };
                rpta = SqlMapper.Query<RecordExpedientesBE>(
                   con, "OCMAERP.SP_ANTECEDENTES_RECORD_EXPEDIENTES_TOTALES  :AN_CODPERSONA,:AN_COD_SUBTIPANTEC", parameters, commandType: System.Data.CommandType.StoredProcedure).SingleOrDefault();

                con.Close();
            }
            catch (Exception er)
            {
                rpta.codigo = -1;
                rpta.valor = "Error: " + er.Message;
                con.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }
            return rpta;
        }
        public List<PiePagAntecedenteSancionesBE> AntecedentesSancionesPiePagina(string codigo_persona, int sub_tipo_antecedente)
        {
            List<PiePagAntecedenteSancionesBE> lista = new List<PiePagAntecedenteSancionesBE>();
            try
            {
                connection();
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("AN_CODPERSONA", codigo_persona, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("AN_COD_SUBTIPANTEC", sub_tipo_antecedente, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                IList<PiePagAntecedenteSancionesBE> rpta = SqlMapper.Query<PiePagAntecedenteSancionesBE>(
                   con, "OCMAERP.SP_ANTECEDENTES_RECORD_SANCIONES_PIE_PAGINA  :AN_CODPERSONA,:AN_COD_SUBTIPANTEC", parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();
                con.Close();

                lista = rpta.ToList();


            }
            catch (Exception er)
            {
                lista[0].codigo = -1;
                lista[0].valor = "Error: " + er.Message;
                con.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }

            return lista;
        }
        public List<PiePagAntecedenteSancionesBE> AntecedentesPiePaginaExpediente(string codigo_persona, int sub_tipo_antecedente)
        {
            List<PiePagAntecedenteSancionesBE> lista = new List<PiePagAntecedenteSancionesBE>();
            try
            {
                connection();
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("AN_CODPERSONA", codigo_persona, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("AN_COD_SUBTIPANTEC", sub_tipo_antecedente, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                IList<PiePagAntecedenteSancionesBE> rpta = SqlMapper.Query<PiePagAntecedenteSancionesBE>(
                   con, "OCMAERP.SP_ANTECEDENTES_RECORD_EXPEDIENTES_PIE_PAGINA  :AN_CODPERSONA,:AN_COD_SUBTIPANTEC", parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();
                con.Close();

                lista = rpta.ToList();


            }
            catch (Exception er)
            {
                lista[0].codigo = -1;
                lista[0].valor = "Error: " + er.Message;
                con.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }

            return lista;
        }
        //CONEXION ZAVALA
        public RptVerificarDocBE VericarDoc(string cod_usuario)
        {
            RptVerificarDocBE rpta = new RptVerificarDocBE();
            try
            {
                connection_zav();
                con_zav.Open();

                var parameters = new
                {
                    AS_USUARIO = cod_usuario
                };

                rpta = con_zav.QueryFirstOrDefault<RptVerificarDocBE>("OCMAERP.SP_ANTECEDENTES_VERIFICADOR_CODIGO_CLAVE :AS_USUARIO ", parameters, commandType: System.Data.CommandType.StoredProcedure);
                con_zav.Close();
            }
            catch (Exception er)
            {
                rpta.codigo = -1;
                rpta.valor = "Error: " + er.Message;
                con_zav.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }
            return rpta;
        }
        //CONEXION ZAVALA
        public RptVerificacionBE ActualizarVericaDoc(ActualizarVerificaDocBE datos)
        {
            RptVerificacionBE rpta = new RptVerificacionBE();
            try
            {
                connection_zav();
                con_zav.Open();
                var parameters = new DynamicParameters();
                parameters.Add("AN_VD_ID", datos.an_vd_id, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("AN_COD_ANTEC", datos.an_cod_antec, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("AS_USUARIO", datos.as_usuario, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                rpta = con_zav.QueryFirstOrDefault<RptVerificacionBE>("OCMAERP.SP_ANTECEDENTES_VERIFICADOR_ACTUALIZAR :AN_VD_ID,:AN_COD_ANTEC ,:AS_USUARIO", parameters, commandType: System.Data.CommandType.StoredProcedure);
                con_zav.Close();

            }
            catch (Exception er)
            {
                rpta.codigo = -1;
                rpta.valor = "Error: " + er.Message;
                con_zav.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }

            return rpta;
        }

        //Conexion ANC
        public RptVerificacionBE LoginVericar(AntecedentesBE datos)
        {
            RptVerificacionBE rpta = new RptVerificacionBE();
            try
            {
                connection();
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("AS_NUM_DOCUMENTO", datos.num_documento, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("AS_AP_PATERNO", datos.ap_paterno, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("AS_AP_MATERNO", datos.ap_materno, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("AS_NOMBRES", datos.nombres, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                rpta = con.QueryFirstOrDefault<RptVerificacionBE>("OC_OCMA.SP_ANTECEDENTES_LOGIN_VERIFICAR :AS_NUM_DOCUMENTO,:AS_AP_PATERNO ,:AS_AP_MATERNO,:AS_NOMBRES", parameters, commandType: System.Data.CommandType.StoredProcedure);
                con.Close();

            }
            catch (Exception er)
            {
                rpta.codigo = -1;
                rpta.valor = "Error: " + er.Message;
                con.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }

            return rpta;
        }
        public async Task<RptTramiteDocumentarioBE> RegistroTramiteDocumentario(TramiteDocumentarioBE datos)
        {
            RptTramiteDocumentarioBE rpta = new RptTramiteDocumentarioBE();
            try
            {
                connection();
                con.Open();
                var parameters = new DynamicParameters();
                parameters.Add("AN_COD_INTMP", datos.cod_intmp, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("AN_COD_MPDF_CEDULA", datos.cod_mpdf_cedula, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("AN_COD_MPDF_FORMULARIO", datos.cod_mpdf_formulario, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                parameters.Add("AS_PATERNO", datos.ap_paterno, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("AS_MATERNO", datos.ap_materno, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("AS_NOMBRE", datos.nombres, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("AS_NRODNI", datos.num_documento, System.Data.DbType.String, System.Data.ParameterDirection.Input);

                parameters.Add("AS_TAMANIO_CEDULA", datos.tamanio_cedula, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("AS_TAMANIO_FORMULARIO", datos.tamanio_formulario, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("AS_CORREO", datos.correo, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("AN_COD_TIPANTEC", datos.tipo_antecedente, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                parameters.Add("AS_ACCION", datos.accion, System.Data.DbType.String, System.Data.ParameterDirection.Input);

                rpta = con.QueryFirstOrDefault<RptTramiteDocumentarioBE>("OCMAERP.SP_ANTECEDENTES_DOCUMENTO_MP :AN_COD_INTMP, :AN_COD_MPDF_CEDULA, :AN_COD_MPDF_FORMULARIO, :AS_PATERNO ,:AS_MATERNO ,:AS_NOMBRE ,:AS_NRODNI ,:AS_TAMANIO_CEDULA ,:AS_TAMANIO_FORMULARIO ,:AS_CORREO ,:AN_COD_TIPANTEC,:AS_ACCION", parameters, commandType: System.Data.CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception er)
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "Error: " + er.Message;
                con.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }

            return rpta;
        }
        //Conexion POSTGRES
        public async Task<RptVerificacionBE> AuditoriaAntecedentes(AuditoriaAntecedentesBE datos)
        {
            RptVerificacionBE rpta = new RptVerificacionBE();
            try
            {
                con_pg = await connection_pg();

                var parameters = new DynamicParameters();
                parameters.Add("as_cod_usuario", datos.cod_usuario, DbType.String, ParameterDirection.Input);
                parameters.Add("an_cod_intper", datos.cod_intper, DbType.Int32, ParameterDirection.Input);

                parameters.Add("as_ap_paterno", datos.ap_paterno, DbType.String, ParameterDirection.Input);
                parameters.Add("as_ap_materno", datos.ap_materno, DbType.String, ParameterDirection.Input);
                parameters.Add("as_nombres", datos.nombres, DbType.String, ParameterDirection.Input);
                parameters.Add("as_nro_documento", datos.num_documento, DbType.String, ParameterDirection.Input);
                
                parameters.Add("an_cod_tipmovantec", datos.cod_tipmovantec, DbType.Int32, ParameterDirection.Input);
                parameters.Add("an_cod_origantec", datos.cod_origantec, DbType.Int32, ParameterDirection.Input);
                parameters.Add("as_des_auditoria", datos.des_auditoria, DbType.String, ParameterDirection.Input);
                parameters.Add("as_id_sesion", datos.id_sesion, DbType.String, ParameterDirection.Input);
                parameters.Add("as_direccion_ip", datos.direccion_ip, DbType.String, ParameterDirection.Input);

                parameters.Add("as_mac_address", datos.mac_address, DbType.String, ParameterDirection.Input);
                parameters.Add("as_computer_name", datos.computer_name, DbType.String, ParameterDirection.Input);
                parameters.Add("as_sistema_operativo", datos.sistema_operativo, DbType.String, ParameterDirection.Input);

                parameters.Add("rn_codigo", dbType: DbType.String, direction: ParameterDirection.Output);
                parameters.Add("rs_valor", dbType: DbType.Boolean, direction: ParameterDirection.Output);


                rpta = con_pg.QuerySingleOrDefault<RptVerificacionBE>("antecedentes.sp_antec_auditoria_mant", parameters, commandType: CommandType.StoredProcedure);


                con_pg.Close();

            }
            catch (Exception er)
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "Surgio un error inesperado";
                rpta.codigo = -1;
                rpta.valor = "Surgio un error inesperado";
                con_pg.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }


            return rpta;
        }

        public async Task<RptVerificacionBE> VericarAntecedentes(VerificaAntecedentesBE datos)
        {
            RptVerificacionBE rpta = new RptVerificacionBE();
            try
            {
                con_pg = await connection_pg();


                List<int> an_orden = new List<int>(datos.an_orden ?? new int[0]);
                if (datos.an_orden == null || datos.an_orden.Length == 0){an_orden.Add(1);}
               
                List<int> an_principal = new List<int>(datos.an_principal ?? new int[0]);
                if (datos.an_principal == null || datos.an_principal.Length == 0) { an_principal.Add(1); }

                List<string> as_origen = new List<string>(datos.as_origen?.ToList() ?? new List<string>());
                if (datos.as_origen == null || datos.as_origen.Length == 0) { as_origen.Add(""); }
                List<string> as_expediente = new List<string>(datos.as_expediente?.ToList() ?? new List<string>());
                if (datos.as_expediente == null || datos.as_expediente.Length == 0) { as_expediente.Add(""); }
                List<string> as_instancia = new List<string>(datos.as_instancia?.ToList() ?? new List<string>());
                if (datos.as_instancia == null || datos.as_instancia.Length == 0) { as_instancia.Add(""); }
                List<string> as_sancion = new List<string>(datos.as_sancion?.ToList() ?? new List<string>());
                if (datos.as_sancion == null || datos.as_sancion.Length == 0) { as_sancion.Add(""); }

                List<DateTime> ad_fecha = new List<DateTime>(datos.ad_fecha?.ToList() ?? new List<DateTime>());
                if (datos.ad_fecha == null || datos.ad_fecha.Length == 0) { ad_fecha.Add(new DateTime()); }
                List<string> as_estado = new List<string>(datos.as_estado?.ToList() ?? new List<string>());
                if (datos.as_estado == null || datos.as_estado.Length == 0) { as_estado.Add(""); }
                List<string> as_observaciones = new List<string>(datos.as_observaciones?.ToList() ?? new List<string>());
                if (datos.as_observaciones == null || datos.as_observaciones.Length == 0) { as_observaciones.Add(""); }

                List<DateTime> ad_fec_apertura = new List<DateTime>(datos.ad_fec_apertura?.ToList() ?? new List<DateTime>());
                if (datos.ad_fec_apertura == null || datos.ad_fec_apertura.Length == 0) { ad_fec_apertura.Add(new DateTime()); }
                List<string> as_encargado = new List<string>(datos.as_encargado?.ToList() ?? new List<string>());
                if (datos.as_encargado == null || datos.as_encargado.Length == 0) { as_encargado.Add(""); }
                List<string> as_resolucion = new List<string>(datos.as_resolucion?.ToList() ?? new List<string>());
                if (datos.as_resolucion == null || datos.as_resolucion.Length == 0) { as_resolucion.Add(""); }

                List<int> an_cod_distrito_origen = new List<int>(datos.an_cod_distrito_origen ?? new int[0]);
                if (datos.an_cod_distrito_origen == null || datos.an_cod_distrito_origen.Length == 0) { an_cod_distrito_origen.Add(0); }

                List<int> an_cod_intexp = new List<int>(datos.an_cod_intexp ?? new int[0]);
                if (datos.an_cod_intexp == null || datos.an_cod_intexp.Length == 0) { an_cod_intexp.Add(1); }


                List<int> an_num_secuen = new List<int>(datos.an_num_secuen ?? new int[0]);
                if (datos.an_num_secuen == null || datos.an_num_secuen.Length == 0) { an_num_secuen.Add(1); }
                List<int> an_cod_resinf = new List<int>(datos.an_cod_resinf ?? new int[0]);
                if (datos.an_cod_resinf == null || datos.an_cod_resinf.Length == 0) { an_cod_resinf.Add(1); }
                List<int> an_cod_estado_expe = new List<int>(datos.an_cod_estado_expe ?? new int[0]);
                if (datos.an_cod_estado_expe == null || datos.an_cod_estado_expe.Length == 0) { an_cod_estado_expe.Add(1); }
                List<int> an_cod_estado_reso = new List<int>(datos.an_cod_estado_reso ?? new int[0]);
                if (datos.an_cod_estado_reso == null || datos.an_cod_estado_reso.Length == 0) { an_cod_estado_reso.Add(1); }
                List<int> an_cod_encarg = new List<int>(datos.an_cod_encarg ?? new int[0]);
                if (datos.an_cod_encarg == null || datos.an_cod_encarg.Length == 0) { an_cod_encarg.Add(1); }


                List<string> as_num_expedi = new List<string>(datos.as_num_expedi?.ToList() ?? new List<string>());
                if (datos.as_num_expedi == null || datos.as_num_expedi.Length == 0) { as_num_expedi.Add(""); }
                List<string> as_num_anios = new List<string>(datos.as_num_anios?.ToList() ?? new List<string>());
                if (datos.as_num_anios == null || datos.as_num_anios.Length == 0) { as_num_anios.Add(""); }
                List<string> as_des_tipexp = new List<string>(datos.as_des_tipexp?.ToList() ?? new List<string>());
                if (datos.as_des_tipexp == null || datos.as_des_tipexp.Length == 0) { as_des_tipexp.Add(""); }
                List<string> as_des_motivo = new List<string>(datos.as_des_motivo?.ToList() ?? new List<string>());
                if (datos.as_des_motivo == null || datos.as_des_motivo.Length == 0) { as_des_motivo.Add(""); }
                List<string> as_detalle_motivo = new List<string>(datos.as_detalle_motivo?.ToList() ?? new List<string>());
                if (datos.as_detalle_motivo == null || datos.as_detalle_motivo.Length == 0) { as_detalle_motivo.Add(""); }
                List<string> as_expe_rel = new List<string>(datos.as_expe_rel?.ToList() ?? new List<string>());
                if (datos.as_expe_rel == null || datos.as_expe_rel.Length == 0) { as_expe_rel.Add(""); }
                List<string> as_tipo_rel = new List<string>(datos.as_tipo_rel?.ToList() ?? new List<string>());
                if (datos.as_tipo_rel == null || datos.as_tipo_rel.Length == 0) { as_tipo_rel.Add(""); }
                List<string> as_fla_tipo = new List<string>(datos.as_fla_tipo?.ToList() ?? new List<string>());
                if (datos.as_fla_tipo == null || datos.as_fla_tipo.Length == 0) { as_fla_tipo.Add(""); }

                List<string> as_correo = new List<string>(datos.as_correo?.ToList() ?? new List<string>());
                if (datos.as_correo == null || datos.as_correo.Length == 0) { as_correo.Add(""); }

                List<string> as_celular = new List<string>(datos.as_celular?.ToList() ?? new List<string>());
                if (datos.as_celular == null || datos.as_celular.Length == 0) { as_celular.Add(""); }


                var parameters = new DynamicParameters();
                parameters.Add("as_cod_usuario", datos.as_cod_usuario, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_intper", datos.an_cod_intper, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("as_ap_paterno", datos.as_ap_paterno, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_ap_materno", datos.as_ap_materno, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_nombres", datos.as_nombres, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_num_documento", datos.as_num_documento, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_tipantec", datos.an_cod_tipantec, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_subtipantec", datos.an_cod_subtipantec, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_origantec", datos.an_cod_origantec, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("as_nombre_doc", datos.as_nombre_doc, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("an_n_vd_id", datos.an_n_vd_id, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);


                parameters.Add("as_correo", as_correo.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_celular",as_celular.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);


                parameters.Add("an_orden", an_orden.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);

                parameters.Add("an_principal", an_principal.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_origen", as_origen.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_expediente", as_expediente.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_instancia", as_instancia.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_sancion", as_sancion.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);


                parameters.Add("ad_fecha", ad_fecha.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);

                parameters.Add("as_estado", as_estado.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_observaciones", as_observaciones.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);


                parameters.Add("ad_fec_apertura", ad_fec_apertura.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);

                parameters.Add("as_encargado", as_encargado.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_resolucion", as_resolucion.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_distrito_origen", an_cod_distrito_origen.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_intexp", an_cod_intexp.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("an_num_secuen", an_num_secuen.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_resinf", an_cod_resinf.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_estado_expe", an_cod_estado_expe.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_estado_reso", an_cod_estado_reso.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_encarg", an_cod_encarg.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);

                parameters.Add("as_num_expedi", as_num_expedi.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_num_anios", as_num_anios.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_des_tipexp", as_des_tipexp.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_des_motivo", as_des_motivo.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_detalle_motivo", as_detalle_motivo.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);

                parameters.Add("as_expe_rel", as_expe_rel.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_tipo_rel", as_tipo_rel.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_fla_tipo", as_fla_tipo.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);

                parameters.Add("as_id_sesion", datos.as_id_sesion, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_direccion_ip", datos.as_direccion_ip, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_mac_address", datos.as_mac_address, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_computer_name", datos.as_computer_name, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_sistema_operativo", datos.as_sistema_operativo, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_accion", datos.as_accion, System.Data.DbType.String, System.Data.ParameterDirection.Input);

                parameters.Add("rn_codigo", System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);
                parameters.Add("rs_valor", System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);
                parameters.Add("rn_cod_antec", System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);
                parameters.Add("rd_fec_vigencia", System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);                
                parameters.Add("rs_nro_registro", System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);
                parameters.Add("rs_anio", System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);
                parameters.Add("rs_ruta_ftp", System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);
                parameters.Add("rs_fec_creacion", System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);


                rpta = con_pg.QuerySingleOrDefault<RptVerificacionBE>(
                   "db_anc.antecedentes.sp_antec_master_mant",
                   parameters,
                   commandType: CommandType.StoredProcedure
               );

                con_pg.Close();

            }
            catch (Exception er)
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "Surgio un error inesperado";
                rpta.codigo = -1;
                rpta.valor = "Error: " + er.Message;
                con_pg.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }

            return rpta;
        }

        public async Task<RptPGBE> RegistrarAcuseAntecedentes(AcuseCorreoBE datos)
        {
            RptPGBE rpta = new RptPGBE();
            try
            {
                con_pg = await connection_pg();


                List<string> regitro_antec = new List<string>(datos.regitro_antec?.ToList() ?? new List<string>());
                List<string> ruta_archivo = new List<string>(datos.ruta_archivo?.ToList() ?? new List<string>());

                List<int> cod_antec = new List<int>(datos.cod_antec ?? new int[0]);


                var parameters = new DynamicParameters();
                parameters.Add("an_cod_notifantec", datos.cod_notif_antec, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("as_correo_destino", datos.correo_destino, System.Data.DbType.String, System.Data.ParameterDirection.Input);

                parameters.Add("as_correo_coo", datos.correo_coo, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("asa_ruta_archivo", ruta_archivo.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);

                parameters.Add("as_usu_envio", datos.usu_envio, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_estado_envio", datos.estado_envio, System.Data.DbType.String, System.Data.ParameterDirection.Input);


                parameters.Add("as_nombre_receptor", datos.nombres, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_registro_td", datos.registro_td, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("asa_registro_antec", regitro_antec.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_intmp", datos.cod_intmp, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("ana_cod_antec", cod_antec.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);


                parameters.Add("an_distri_act", datos.cod_distrito, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("as_accion", datos.accion, System.Data.DbType.String, System.Data.ParameterDirection.Input);



                parameters.Add("rn_codigo", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);
                parameters.Add("rs_valor", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);


                rpta = con_pg.QuerySingleOrDefault<RptPGBE>("db_anc.antecedentes.sp_antec_notificacion_mant", parameters, commandType: CommandType.StoredProcedure);



                con_pg.Close();

            }
            catch (Exception er)
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "Surgio un error inesperado";

                con_pg.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }


            return rpta;
        }



        public async Task<List<RptListaResponsableBE>> ListaResponsableAntecedentes()
        {
            List<RptListaResponsableBE> rpta = new List<RptListaResponsableBE>();
            try
            {
                con_pg = await connection_pg();


                string query = "SELECT * FROM antecedentes.f_antec_responsable_list()";
                 rpta = (await con_pg.QueryAsync<RptListaResponsableBE>(query)).ToList();
              



                con_pg.Close();

            }
            catch (Exception er)
            {

                con_pg.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }


            return rpta;
        }

        public async Task<List<RptConsutaAntecedenteBE>> ConsultaAntecedentes(ConsultaAntecedenteBE datos)
        {
            List<RptConsutaAntecedenteBE> rpta = new List<RptConsutaAntecedenteBE>();
            try
            {
                con_pg = await connection_pg();

                var parameters = new
                {
                    an_cod_intper = datos.cod_intper,
                    as_num_documento = datos.num_documento,
                    as_nro_registro = datos.nro_registro,
                    as_anio = datos.anio,
                    an_tipo_consulta = datos.tipo_consulta,
                    an_cod_tipantec = datos.cod_tipantec,
                    an_cod_subtipantec = datos.cod_subtipantec,
                    an_cod_origantec = datos.cod_origantec,
                    an_fla_vigencia = datos.fla_vigencia
                };
                
                string query = "SELECT * FROM antecedentes.f_consulta_tramite_list(@an_cod_intper,@as_num_documento,@as_nro_registro,@as_anio,@an_tipo_consulta,@an_cod_tipantec,@an_cod_subtipantec,@an_cod_origantec,@an_fla_vigencia)";
                
                rpta = (await con_pg.QueryAsync<RptConsutaAntecedenteBE>(query, parameters)).ToList();

                con_pg.Close();

            }
            catch (Exception er)
            {

                con_pg.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }


            return rpta;
        }


        public async Task<RptPGBE> MantenimientoRegistroAntecedentePG(MantRegistroPGBE datos)
        {
            RptPGBE rpta = new RptPGBE();
            try
            {
                con_pg = await connection_pg();


                var parameters = new DynamicParameters();
                parameters.Add("an_cod_intmp", datos.cod_intmp, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_intmp_rpta", datos.cod_intmp_rpta, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_intper", datos.cod_intper, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                parameters.Add("as_nro_documento", datos.nro_documento, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_nro_registro", datos.nro_registro, System.Data.DbType.String, System.Data.ParameterDirection.Input);

                parameters.Add("as_anio", datos.anio, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_nombre_oficio", datos.nombre_oficio, System.Data.DbType.String, System.Data.ParameterDirection.Input);

                parameters.Add("an_cod_antec", datos.cod_antec.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_tipantec", datos.cod_tipantec.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_correo", datos.correo.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_celular", datos.celular.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);
                parameters.Add("as_casilla", datos.casilla.ToArray(), System.Data.DbType.Object, System.Data.ParameterDirection.Input);


                parameters.Add("an_cod_origantec", datos.cod_origantec, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("as_cod_usuario", datos.cod_usuario, System.Data.DbType.String, System.Data.ParameterDirection.Input);            
                parameters.Add("as_accion", datos.accion, System.Data.DbType.String, System.Data.ParameterDirection.Input);



                parameters.Add("rn_codigo", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);
                parameters.Add("rs_valor", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);


                rpta = con_pg.QuerySingleOrDefault<RptPGBE>("db_anc.antecedentes.sp_antec_registro_td_mant", parameters, commandType: CommandType.StoredProcedure);

                con_pg.Close();

            }
            catch (Exception er)
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "Surgio un error inesperado";

                con_pg.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }


            return rpta;
        }

        public async Task<RptVerificacionBE> VerificarTDAntecedente(string num_documento,int cod_tipo_repo)
        {
            RptVerificacionBE rpta = new RptVerificacionBE();
            try
            {
                con_pg = await connection_pg();

                var parameters = new
                {
                   as_num_documento = num_documento,
                   an_cod_tipantec = cod_tipo_repo
                };

                string query = "SELECT * FROM antecedentes.f_registro_td_verificar(@as_num_documento,@an_cod_tipantec)";

                rpta = (await con_pg.QuerySingleOrDefaultAsync<RptVerificacionBE>(query, parameters));

                con_pg.Close();

            }
            catch (Exception er)
            {
                rpta.rn_codigo=-1;
                rpta.rs_valor = "Surgio un error inesperado";
                con_pg.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }


            return rpta;
        }
        public async Task<RptPGBE> MantMovimiento(AuditoriaAntecedentesBE datos)
        {
            RptPGBE rpta = new RptPGBE();
            try
            {
                con_pg = await connection_pg();


                var parameters = new DynamicParameters();
                parameters.Add("as_cod_usuario", datos.cod_usuario, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("an_cod_antec", datos.cod_antec, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);

                parameters.Add("an_cod_tipmovantec", datos.cod_tipmovantec, System.Data.DbType.Int32, System.Data.ParameterDirection.Input);
                parameters.Add("as_des_movantec", datos.des_mov_antec, System.Data.DbType.String, System.Data.ParameterDirection.Input);

                parameters.Add("as_id_sesion", datos.id_sesion, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_direccion_ip", datos.direccion_ip, System.Data.DbType.String, System.Data.ParameterDirection.Input);


                parameters.Add("as_mac_address", datos.mac_address, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_computer_name", datos.computer_name, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_sistema_operativo", datos.sistema_operativo, System.Data.DbType.String, System.Data.ParameterDirection.Input);
     

                parameters.Add("rn_codigo", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);
                parameters.Add("rs_valor", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);


                rpta = con_pg.QuerySingleOrDefault<RptPGBE>("db_anc.antecedentes.sp_antec_master_movimiento_mant", parameters, commandType: CommandType.StoredProcedure);


                con_pg.Close();

            }
            catch (Exception er)
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "Surgio un error inesperado";

                con_pg.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }


            return rpta;
        }
        public async Task<RptPGBE> VerificarIngreso(VerificaIngresoBE datos)
        {
            RptPGBE rpta = new RptPGBE();
            try
            {
                con_pg = await connection_pg();


                var parameters = new DynamicParameters();
                parameters.Add("as_nro_documento", datos.nro_documento, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                parameters.Add("as_direccion_ip", datos.as_direccion_ip, System.Data.DbType.String, System.Data.ParameterDirection.Input);
     

                string query = "SELECT * FROM antecedentes.f_ingreso_cade_verificar(@as_nro_documento,@as_direccion_ip)";

                rpta = (await con_pg.QuerySingleOrDefaultAsync<RptPGBE>(query, parameters));



                con_pg.Close();

            }
            catch (Exception er)
            {
                rpta.rn_codigo = -1;
                rpta.rs_valor = "Surgio un error inesperado";

                con_pg.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }


            return rpta;
        }

        public async Task<List<ListAnioBE>> ListarAnio()
        {
            List<ListAnioBE> rpta = new List<ListAnioBE>();
            try
            {
                con_pg = await connection_pg();


                string query = "SELECT * FROM anc.f_anio_list()";
                rpta = (await con_pg.QueryAsync<ListAnioBE>(query)).ToList();




                con_pg.Close();

            }
            catch (Exception er)
            {

                con_pg.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }


            return rpta;
        }

      
    }
}
