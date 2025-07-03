using ANC.Comun.Log4net;
using ANC.Entidades;
using System.Net.Http.Json;
using System.Text.Json;

namespace ANC.Modelo
{
    public class GenericoModel
    {
        public static string _configURL = Env._connectionApiEndPoint;

        private static string _user_auth = Env._user_auth;
        private static string _password_auth = Env._password_auth;
        private static string _api_url_auth = Env._api_url_auth;
        private static string _api_url_reniec = Env._url_reniec;
        JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        #region FTP
        public async Task<CredencialesFTPBE> GetCredencialesFTP(int coddistri)
        {//usando
            CredencialesFTPBE respuesta = new CredencialesFTPBE();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", Env._connectionToken);

            string path = _configURL + @"Generico/GetCredencialesFTP?coddistri=" + coddistri;

            try
            {
                HttpResponseMessage response = client.GetAsync(path).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    respuesta = JsonSerializer.Deserialize<CredencialesFTPBE>(content, options);
                }
                else
                {
                    throw new Exception("Error al esperar respuesta del webapiCRM");
                }
                return respuesta;
            }
            catch (Exception er)
            {
                LogAntecedentes.Logger().Error("Error: " + er.Message);
                throw;
            }
        }
        #endregion
        #region Modelo AntecedentesWEB --- Johan Perez
        public async Task<string> GetTokenAuth()
        {
            HttpClient client = new HttpClient();
            var request = new
            {
                usuario = _user_auth,
                clave = _password_auth
            };


            var tokenPath = _api_url_auth + @"api/Auth/GenerateAuthToken";
            responseAuth resultAuth = new responseAuth();

            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync(tokenPath, request).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    resultAuth = JsonSerializer.Deserialize<responseAuth>(content, options);


                }
                else
                {
                    throw new Exception("Error al esperar respuesta del Servicio");
                }

            }
            catch (Exception er)
            {

                throw;
            }



            return resultAuth.obj.token;
        }
        public async Task<List<PetTipoDocBE>> ObtenerTipoDoc()
        {
            HttpClient client = new HttpClient();
            List<PetTipoDocBE> lista = new List<PetTipoDocBE>();
            client.DefaultRequestHeaders.Add("Authorization", Env._connectionToken);

            string path = _configURL + @"Generico/ObtenerTipoDoc";

            try
            {
                HttpResponseMessage response = client.GetAsync(path).Result;
                if (response.IsSuccessStatusCode)
                {

                    var content = await response.Content.ReadAsStringAsync();
                    lista = JsonSerializer.Deserialize<List<PetTipoDocBE>>(content, options);
                }

                return lista;
            }
            catch (Exception er)
            {
                LogAntecedentes.Logger().Error("Error: " + er.Message);
                throw;
            }

        }


        public async Task<List<DepartamentoBE>> ObtenerDepartamento()
        {
            HttpClient client = new HttpClient();
            List<DepartamentoBE> lista = new List<DepartamentoBE>();
            client.DefaultRequestHeaders.Add("Authorization", Env._connectionToken);
            string path = _configURL + @"Generico/ObtenerDepartamento";

            try
            {
                HttpResponseMessage response = client.GetAsync(path).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    lista = JsonSerializer.Deserialize<List<DepartamentoBE>>(content, options);
                }

                return lista;
            }
            catch (Exception er)
            {
                LogAntecedentes.Logger().Error("Error: " + er.Message);
                throw;
            }

        }
        public async Task<List<ProvinciaBE>> ObtenerProvincia(string codDepa)
        {
            HttpClient client = new HttpClient();
            List<ProvinciaBE> lista = new List<ProvinciaBE>();
            client.DefaultRequestHeaders.Add("Authorization", Env._connectionToken);
            string path = _configURL + @"Generico/ObtenerProvincia?codDepa=" + codDepa;

            try
            {
                HttpResponseMessage response = client.GetAsync(path).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    lista = JsonSerializer.Deserialize<List<ProvinciaBE>>(content, options);
                }

                return lista;
            }
            catch (Exception er)
            {
                LogAntecedentes.Logger().Error("Error: " + er.Message);
                throw;
            }

        }
        public async Task<List<DistritoBE>> ObtenerDistrito(string codDepa, string codProv)
        {
            HttpClient client = new HttpClient();
            List<DistritoBE> lista = new List<DistritoBE>();
            client.DefaultRequestHeaders.Add("Authorization", Env._connectionToken);
            string path = _configURL + @"Generico/ObtenerDistrito?codDepa=" + codDepa + "&codProv=" + codProv;

            try
            {
                HttpResponseMessage response = client.GetAsync(path).Result;
                if (response.IsSuccessStatusCode)
                {

                    var content = await response.Content.ReadAsStringAsync();
                    lista = JsonSerializer.Deserialize<List<DistritoBE>>(content, options);
                }

                return lista;
            }
            catch (Exception er)
            {
                LogAntecedentes.Logger().Error("Error: " + er.Message);
                throw;
            }

        }
        public async Task<ReniecBE> ConsultarReniecDNI(paramsApiReniec datos)
        {
            HttpClient client = new HttpClient();
            ReniecBE reniecBE = new ReniecBE();


            var token = await GetTokenAuth();
            client.DefaultRequestHeaders.Add("Authorization", token);



            var path = _api_url_reniec + @"Reniec";

            try
            {
                ResponseReniecModelType2 responseData = new ResponseReniecModelType2();


                HttpResponseMessage response = client.PostAsJsonAsync(path, datos).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    responseData = JsonSerializer.Deserialize<ResponseReniecModelType2>(content, options);


                    if (responseData.codigo != "0000")
                    {
                        reniecBE.codigo = "-2";
                        reniecBE.descripcion = responseData.descripcion;
                        return reniecBE;
                    }

                    var data = responseData.data;



                    reniecBE.codigo = "0";
                    reniecBE.numdni = data.nroDocumentoIdentidad;
                    reniecBE.foto = Convert.FromBase64String(data.foto);
                    reniecBE.firma = (string.IsNullOrEmpty(data.firma)) ? null : Convert.FromBase64String(data.firma);
                    reniecBE.apaterno = data.apellidoPaterno;
                    reniecBE.amaterno = data.apellidoMaterno;
                    reniecBE.apcasada = data.apellidoCasado;
                    reniecBE.nombres = data.nombres;
                    reniecBE.sexo = data.sexo;
                    reniecBE.estadocivil_detalle = data.estadoCivil;
                    reniecBE.estatura = data.estatura;
                    reniecBE.departamentoubi_detalle = data.departamentoDomicilio;
                    reniecBE.provinciaubi_detalle = data.provinciaDomicilio;
                    reniecBE.distritoubi_detalle = data.distritoDomicilio;
                    reniecBE.localidadubi_detalle = data.localidadDomicilio;
                    reniecBE.urbaniza = data.urbanizacion;
                    reniecBE.direccion = data.direccion;
                    reniecBE.num_direccion = data.nroDireccion;
                    reniecBE.block = data.blockOChalet;
                    reniecBE.pref_des = data.preBlockOChalet;
                    reniecBE.interior = data.interior;
                    reniecBE.etapa = data.etapa;
                    reniecBE.manzana = data.manzana;
                    reniecBE.lote = data.lote;
                    reniecBE.grado_instruccion = data.gradoInstruccion;
                    reniecBE.tipdoc_sustento = data.documentoSustentatorioTipoDocumento;
                    reniecBE.doc_sustento = data.documentoSustentatorioTipoDocumento;
                    reniecBE.departamentonac_detalle = data.departamentoNacimiento;
                    reniecBE.provincianac_detalle = data.provinciaNacimiento;
                    reniecBE.distritonac_detalle = data.distritoNacimiento;
                    reniecBE.localidadnac_detalle = data.localidadNacimiento;
                    reniecBE.fechanac = DateTime.Parse(data.fechaNacimiento);
                    reniecBE.nombre_padre = data.nombrePadre;
                    reniecBE.tipdoc_padre = data.documentoPadreTipDocumento;
                    reniecBE.nombre_madre = data.nombreMadre;
                    reniecBE.tipdoc_madre = data.documentoMadreTipoDocumento;
                    reniecBE.fecha_inscripcion = (string.IsNullOrEmpty(data.fechaInscripcion)) ? (DateTime?)null : DateTime.Parse(data.fechaInscripcion);
                    reniecBE.fecha_expedicion = (string.IsNullOrEmpty(data.fechaEmision)) ? (DateTime?)null : DateTime.Parse(data.fechaEmision);
                    reniecBE.fechafallecimiento = (string.IsNullOrEmpty(data.fechaFallecimiento)) ? (DateTime?)null : DateTime.Parse(data.fechaFallecimiento);
                    reniecBE.cons_votacion = data.constanciaVotacion;
                    reniecBE.cons_restricciones = data.restricciones;
                    reniecBE.fecha_caducidad = (string.IsNullOrEmpty(data.fechaCaducidad)) ? (DateTime?)null : DateTime.Parse(data.fechaCaducidad);
                    reniecBE.codigo = "0";
                    reniecBE.cod_ubigeonac = data.codigoUbigeoDepartamentoNacimiento + data.codigoUbigeoProvinciaNacimiento + data.codigoUbigeoDistritoNacimiento;
                    reniecBE.digverifica = data.codigoVerificacion;
                }
                else
                {
                    reniecBE.codigo = "-2";
                    reniecBE.descripcion = "¡No hubo respuesta del servicio de RENIEC, vuelva a intentar más tarde o registre los datos manualmente.";

                }


            }
            catch (Exception er)
            {
                reniecBE.codigo = "-2";
                reniecBE.descripcion = "¡Servicio RENIEC no disponible!. Vuelva a internar en unos minutos.";

            }

            return reniecBE;
        }
        #endregion
    }
}
