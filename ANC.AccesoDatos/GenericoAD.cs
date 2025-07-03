using ANC.Comun.Log4net;
using ANC.Conexion;
using ANC.Entidades;
using Dapper;
using System.Data.Odbc;

namespace ANC.AccesoDatos
{
    public class GenericoAD
    {
        public OdbcConnection con;
        private void connection()
        {
            string constr = ConexionBD.ConnectionString;
            con = new OdbcConnection(constr);
        }
        public async Task<CredencialesFTPBE> GetCredencialesFTP(int coddistri)
        {
            CredencialesFTPBE lista = new CredencialesFTPBE();
            try
            {
                connection();
                con.Open();

                var parameters = new
                {
                    coddistri = coddistri
                };

                lista = await con.QueryFirstOrDefaultAsync<CredencialesFTPBE>("OCMAERP.SP_DDJJ_CREDENCIALES_FTP :coddistri", parameters, commandType: System.Data.CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception er)
            {
                con.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);
            }

            return lista;
        }
        #region APIS AntecedentesWEB --- Johan Perez
        public async Task<IEnumerable<PetTipoDocBE>> ObtenerTipoDoc()
        {
            IEnumerable<PetTipoDocBE> lista = new List<PetTipoDocBE>();

            try
            {
                connection();
                con.Open();

                lista = await con.QueryAsync<PetTipoDocBE>("OCMAERP.SP_SelectTipoDoc", new { }, commandType: System.Data.CommandType.StoredProcedure);
                con.Close();

            }
            catch (Exception er)
            {
                con.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);

            }

            return lista;
        }

        public async Task<IEnumerable<DepartamentoBE>> ObtenerDepartamento()
        {

            IEnumerable<DepartamentoBE> lista = new List<DepartamentoBE>();

            try
            {
                connection();
                con.Open();

                IList<DepartamentoBE> rpta = SqlMapper.Query<DepartamentoBE>(
                    con, "OCMAERP.SP_SelectDepartamento", "", commandType: System.Data.CommandType.StoredProcedure).ToList();
                con.Close();

                lista = rpta.ToList();


                lista = await con.QueryAsync<DepartamentoBE>("OCMAERP.SP_SelectDepartamento", new { }, commandType: System.Data.CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception er)
            {
                con.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);

            }
            return lista;
        }
        public async Task<IEnumerable<ProvinciaBE>> ObtenerProvincia(string codDepa)
        {

            IEnumerable<ProvinciaBE> lista = new List<ProvinciaBE>();
            try
            {
                connection();
                con.Open();


                var parameters = new
                {
                    codDepartamento = codDepa,
                };

                lista = await con.QueryAsync<ProvinciaBE>("OCMAERP.SP_SelectProvincia :codDepartamentoa", parameters, commandType: System.Data.CommandType.StoredProcedure);
                con.Close();

            }
            catch (Exception er)
            {
                con.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);

            }
            return lista;
        }
        public async Task<IEnumerable<DistritoBE>> ObtenerDistrito(string codDepa, string codProv)
        {

            IEnumerable<DistritoBE> lista = new List<DistritoBE>();

            try
            {
                connection();
                con.Open();

                var parameters = new
                {
                    codDepartamento = codDepa,
                    codProvincia = codProv,
                };

                lista = await con.QueryAsync<DistritoBE>("OCMAERP.SP_SelectDistrito :codDepartamento, :codProvincia", parameters, commandType: System.Data.CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception er)
            {
                con.Close();
                LogAntecedentes.Logger().Error("Error: " + er.Message);

            }
            return lista;
        }


        #endregion
    }
}
