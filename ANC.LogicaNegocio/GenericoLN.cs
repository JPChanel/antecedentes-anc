using ANC.AccesoDatos;
using ANC.Entidades;

namespace ANC.LogicaNegocio
{
    public class GenericoLN
    {
        GenericoAD comunAD=new GenericoAD();
        public async Task<CredencialesFTPBE> GetCredencialesFTP(int coddistri)
        {
            return await comunAD.GetCredencialesFTP(coddistri);
        }
 
        #region APIS AntecedentesWEB --- Johan Perez
        public async Task<IEnumerable<PetTipoDocBE>> ObtenerTipoDoc()
        {
            return await comunAD.ObtenerTipoDoc();
        }

        public async Task<IEnumerable<DepartamentoBE>> ObtenerDepartamento()
        {
            return await comunAD.ObtenerDepartamento();
        }
        public async Task<IEnumerable<DistritoBE>> ObtenerDistrito(string codDepa, string codProv)
        {
            return await comunAD.ObtenerDistrito(codDepa, codProv);
        }
        public async Task<IEnumerable<ProvinciaBE>> ObtenerProvincia(string codDepa)
        {
            return await comunAD.ObtenerProvincia(codDepa);
        }

        #endregion
    }
}
