using ANC.Comun.Log4net;
using ANC.Entidades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ANC.Modelo
{
    public class AntecedentesModel
    {
        public static string _configURL = Env._connectionApiEndPoint;
        JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        public async Task<string> GetTokenAsync( )
        {

            using (var client = new HttpClient())
            {
                
                client.DefaultRequestHeaders.Add("Authorization", Env._connectionToken);
                string path = _configURL + @"Antecedentes/Autenticacion";
                
                try
                {
                    HttpResponseMessage response =  client.GetAsync(path).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var tokenResponse = await response.Content.ReadAsStringAsync();
                        JWTBE token = JsonSerializer.Deserialize<JWTBE>(await response.Content.ReadAsStringAsync(), options);
                        // Devuelves el token obtenido
                        return token._token;
                    }
                    else { throw new Exception("Error al obtener el token de autenticación"); }

                   
                }
                catch (Exception er)
                {
                    LogAntecedentes.Logger().Error("Error: " + er.Message);
                    throw;
                }
              
                
            }
         
        }

        public  async Task<List<ExpedienteSancionesBE>> AntecedentesRecord(string codigo_persona,string tipo_antecedentes, int Sub_tipo_antecedente)
        {//usando
            HttpClient client = new HttpClient();
            List<ExpedienteSancionesBE> lista = new List<ExpedienteSancionesBE>();

            var token = await GetTokenAsync(); // Obtener el token de autenticación

            client.DefaultRequestHeaders.Add("Authorization", token);
            string path = "";
            if (tipo_antecedentes == "T_RS")
            {
                 path = _configURL + @"Antecedentes/AntecedentesSanciones?codigo_persona=" + codigo_persona + "&sub_tipo_antecedente=" + Sub_tipo_antecedente;

            }
            else if(tipo_antecedentes == "T_RE")
            {
                path = _configURL + @"Antecedentes/AntecedentesExpedientes?codigo_persona=" + codigo_persona + "&sub_tipo_antecedente=" + Sub_tipo_antecedente;
            }
            

            try
            {
                HttpResponseMessage response = client.GetAsync(path).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    lista = JsonSerializer.Deserialize<List<ExpedienteSancionesBE>>(content, options);

                }

                return lista;
            }
            catch (Exception er)
            {
                LogAntecedentes.Logger().Error("Error: " + er.Message);
                throw;
            }

        }

        public async Task<RptVerificacionBE> LoginVericar(AntecedentesBE parametroBE)
        {//usando
            RptVerificacionBE respuesta = new RptVerificacionBE();

            HttpClient client = new HttpClient();
            var token = await GetTokenAsync(); // Obtener el token de autenticación

            client.DefaultRequestHeaders.Add("Authorization", token);

            string path = _configURL + @"Antecedentes/LoginVericar";
            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync(path, parametroBE).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    respuesta = JsonSerializer.Deserialize<RptVerificacionBE>(content, options);
                }
                else
                {
                    respuesta.codigo = -1;
                }
              
                return respuesta;
            }
            catch (Exception er)
            {
                LogAntecedentes.Logger().Error("Error: " + er.Message);
                throw;
            }
        }
        public async Task<RptVerificacionBE> AuditoriaAntecedentes(AuditoriaAntecedentesBE parametroBE)
        {//usando
            RptVerificacionBE respuesta = new RptVerificacionBE();

            HttpClient client = new HttpClient();
            var token = await GetTokenAsync(); // Obtener el token de autenticación

            client.DefaultRequestHeaders.Add("Authorization", token);

            string path = _configURL + @"Antecedentes/AuditoriaAntecedentes";
            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync(path, parametroBE).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    respuesta = JsonSerializer.Deserialize<RptVerificacionBE>(content, options);
                }
                else
                {
                    respuesta.codigo = -1;
                    respuesta.valor = "No se optuvo respuesta de Api";
                }

                return respuesta;
            }
            catch (Exception er)
            {
                LogAntecedentes.Logger().Error("Error: " + er.Message);
                throw;
            }
        }
        public async Task<RptPGBE> AuditoriaMantMovimiento(AuditoriaAntecedentesBE parametroBE)
        {//usando
            RptPGBE respuesta = new RptPGBE();
            HttpClient client = new HttpClient();
            var token = await GetTokenAsync(); // Obtener el token de autenticación

            client.DefaultRequestHeaders.Add("Authorization", token);

            string path = _configURL + @"Antecedentes/MantMovimiento";
            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync(path, parametroBE).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    respuesta = JsonSerializer.Deserialize<RptPGBE>(content, options);
                }
                else
                {
                    respuesta.rn_codigo = -1;
                    respuesta.rs_valor = "No se optuvo respuesta de Api";
                }

                return respuesta;
            }
            catch (Exception er)
            {
                LogAntecedentes.Logger().Error("Error: " + er.Message);
                throw;
            }
        }

        public async Task<RptVerificarDocBE> verificarCodigo(string cod_usuario)
        {//usando
            HttpClient client = new HttpClient();
            RptVerificarDocBE rpt = new RptVerificarDocBE();

            var token = await GetTokenAsync(); // Obtener el token de autenticación

            client.DefaultRequestHeaders.Add("Authorization", token);
            string path = _configURL + @"Antecedentes/VericarDoc?cod_usuario=" + cod_usuario;

            try
            {
                HttpResponseMessage response = client.GetAsync(path).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    rpt = JsonSerializer.Deserialize<RptVerificarDocBE>(content, options);

                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // La respuesta indica que el token no es válido
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new UnauthorizedAccessException(errorMessage); // Lanzar una excepción con el mensaje de error
                }
                else
                {
                    // Otra respuesta de error diferente a 401
                    throw new HttpRequestException($"Error en la solicitud: {response.StatusCode}");
                }


            }
            catch (Exception er)
            {
                LogAntecedentes.Logger().Error("Error: " + er.Message);
                throw;
            }
            return rpt;
        }
        public async Task<RptVerificacionBE> VericarAntecedentes(VerificaAntecedentesBE parametroBE)
        {//usando
            RptVerificacionBE respuesta = new RptVerificacionBE();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", Env._connectionToken);

            string path = _configURL + @"Antecedentes/VericarAntecedentes";
            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync(path, parametroBE).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    respuesta = JsonSerializer.Deserialize<RptVerificacionBE>(content, options);
                }

                return respuesta;
            }
            catch (Exception er)
            {
                LogAntecedentes.Logger().Error("Error: " + er.Message);
                throw;
            }
        }
        public async Task<RptBE> EnviarConfirmacionAcuse(WSEmailBE datos)
        {
            RptBE respuesta = new RptBE();
            HttpClient client = new HttpClient();
           
            client.DefaultRequestHeaders.Add("Authorization", Env._connectionToken);

            string path = _configURL + @"Antecedentes/EnviarConfirmacionAcuse";

            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync(path, datos).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    respuesta = JsonSerializer.Deserialize<RptBE>(content, options);
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
        public async Task<RptPGBE> RegistrarAcuseReciboAntecedentes(AcuseCorreoBE datos)
        {
            RptPGBE respuesta = new RptPGBE();
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", Env._connectionToken);
            string path = _configURL + @"Antecedentes/RegistrarAcuseReciboAntecedentes";

            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync(path, datos).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    respuesta = JsonSerializer.Deserialize<RptPGBE>(content, options);
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

        public async Task<RespuestaArchivoBE> GenerarCertificadoPDF(SesionAntecedentesBE datos)
        {
            RespuestaArchivoBE respuesta = new RespuestaArchivoBE();
            HttpClient client = new HttpClient();

            var token = await GetTokenAsync(); 

            client.DefaultRequestHeaders.Add("Authorization", token);
            string path = _configURL + @"Antecedentes/GenerarCertificadoPDF";

            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync(path, datos).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    respuesta = JsonSerializer.Deserialize<RespuestaArchivoBE>(content, options);
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
        public async Task<RptPGBE> EnviarAcuseAntecedente(AcuseCorreoBE datos)
        {
            RptPGBE respuesta = new RptPGBE();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", Env._connectionToken);

            string path = _configURL + @"Antecedentes/EnviarAcuseAntecedente";

            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync(path, datos).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    respuesta = JsonSerializer.Deserialize<RptPGBE>(content, options);
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

        public async Task<List<RptConsutaAntecedenteBE>> ConsultaAntecedentes(ConsultaAntecedenteBE datos)
        {
            List<RptConsutaAntecedenteBE> respuesta = new List<RptConsutaAntecedenteBE>();

            HttpClient client = new HttpClient();

            var token = await GetTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", token);
            string path = _configURL + @"Antecedentes/ConsultaAntecedentes";

            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync(path, datos).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    respuesta = JsonSerializer.Deserialize<List<RptConsutaAntecedenteBE>> (content, options);
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

        public async Task<RptTramiteDocumentarioBE> RegistroTramiteDocumentario(TramiteDocumentarioBE datos)
        {
            RptTramiteDocumentarioBE respuesta = new RptTramiteDocumentarioBE();

            HttpClient client = new HttpClient();

            var token = await GetTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", token);
            string path = _configURL + @"Antecedentes/RegistroTramiteDocumentario";

            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync(path, datos).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    respuesta = JsonSerializer.Deserialize<RptTramiteDocumentarioBE>(content, options);
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

        public async Task<RptVerificacionBE> VerificarTDAntecedente(string num_documento,int cod_tipo_repo)
        {//usando
            HttpClient client = new HttpClient();
            RptVerificacionBE rpt = new RptVerificacionBE();

            var token = await GetTokenAsync(); // Obtener el token de autenticación

            client.DefaultRequestHeaders.Add("Authorization", token);
            string path = _configURL + @"Antecedentes/VerificarTDAntecedente?num_documento=" + num_documento+ "&cod_tipo_repo=" + cod_tipo_repo;

            try
            {
                HttpResponseMessage response = client.GetAsync(path).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    rpt = JsonSerializer.Deserialize<RptVerificacionBE>(content, options);

                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // La respuesta indica que el token no es válido
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new UnauthorizedAccessException(errorMessage); // Lanzar una excepción con el mensaje de error
                }
                else
                {
                    // Otra respuesta de error diferente a 401
                    throw new HttpRequestException($"Error en la solicitud: {response.StatusCode}");
                }


            }
            catch (Exception er)
            {
                LogAntecedentes.Logger().Error("Error: " + er.Message);
                throw;
            }
            return rpt;
        }
        public async Task<RptPGBE> VerificarIngreso(VerificaIngresoBE datos)
        {
            RptPGBE respuesta = new RptPGBE();

            HttpClient client = new HttpClient();

            var token = await GetTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", token);
            string path = _configURL + @"Antecedentes/VerificarIngreso";

            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync(path, datos).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    respuesta = JsonSerializer.Deserialize<RptPGBE>(content, options);
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
        public async Task<List<ListAnioBE>> ListarAnio()
        {

            using (var client = new HttpClient())
            {

                var token = await GetTokenAsync();

                client.DefaultRequestHeaders.Add("Authorization", token);
                string path = _configURL + @"Antecedentes/ListarAnio";

                try
                {
                    HttpResponseMessage response = client.GetAsync(path).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var tokenResponse = await response.Content.ReadAsStringAsync();
                        List<ListAnioBE> listAnio = JsonSerializer.Deserialize<List<ListAnioBE>>(await response.Content.ReadAsStringAsync(), options);
                        // Devuelves el token obtenido
                        return listAnio;
                    }
                    else { throw new Exception("Error al obtener el token de autenticación"); }


                }
                catch (Exception er)
                {
                    LogAntecedentes.Logger().Error("Error: " + er.Message);
                    throw;
                }


            }

        }
    }
}
