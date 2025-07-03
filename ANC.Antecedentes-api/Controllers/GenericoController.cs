using Microsoft.AspNetCore.Mvc;
using ANC.Entidades;
using ANC.LogicaNegocio;
using System.Net;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using ANC.WebApi.Services;



namespace ANC.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericoController : ControllerBase
    {
        GenericoLN comuln = new GenericoLN();

        private readonly IConfiguration _configuration;

        public GenericoController(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        private bool IsTokenValid()
        {
            string headerToken = Request.Headers["Authorization"];
            string configToken = _configuration.GetValue<string>("Token:key");

            return headerToken == configToken;
        }

        [Route("ConsultarReniecDNI")]
        [HttpGet]
        public async Task<ReniecBE> ConsultarReniecDNI(string dni)
        {
            string HeaderToken = Request.Headers.Where(x => x.Key == "Authorization").FirstOrDefault().Value;
            string ConfigToken = _configuration.GetValue<string>("Token:key");

            if (HeaderToken != ConfigToken)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return null;
            }
            return null;
        }


   

        [Route("GetCredencialesFTP")]
        [HttpGet]
        public async Task<CredencialesFTPBE> GetCredencialesFTP(int coddistri)
        {
            string HeaderToken = Request.Headers.Where(x => x.Key == "Authorization").FirstOrDefault().Value;
            string ConfigToken = _configuration.GetValue<string>("Token:key");

            if (HeaderToken == ConfigToken)
            {
                return await comuln.GetCredencialesFTP(coddistri);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return null;
            }
        }



        #region APIS ANTECEDENTES WEB implementado por Johan Perez
        [Route("ObtenerTipoDoc")]
        [HttpGet]
        public async Task<IEnumerable<PetTipoDocBE>> ObtenerTipoDoc()
        {
            if (!IsTokenValid())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return null;
            }

            return await comuln.ObtenerTipoDoc();

        }
     

        [Route("ObtenerDepartamento")]
        [HttpGet]
        public async Task<IEnumerable<DepartamentoBE>> ObtenerDepartamento()
        {
            if (!IsTokenValid())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return null;
            }
            return await comuln.ObtenerDepartamento();
        }
        [Route("ObtenerProvincia")]
        [HttpGet]
        public async Task<IEnumerable<ProvinciaBE>> ObtenerProvincia(string codDepa)
        {
            return await comuln.ObtenerProvincia(codDepa);
        }

        [Route("ObtenerDistrito")]
        [HttpGet]
        public async Task<IEnumerable<DistritoBE>> ObtenerDistrito(string codDepa, string codProv)
        {
            if (!IsTokenValid())
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return null;
            }
            return await comuln.ObtenerDistrito(codDepa, codProv);
        }


        #endregion
    }
}
