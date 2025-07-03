using ANC.AntecedentesWeb.Api_Tokens;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace ANC.AntecedentesWeb.Controllers
{
    public class PoliticasController : Controller
    {
        util _utilSession=new util();
        public IActionResult TerminosCondiciones()
        {

            int longitudClaveBytes = 32;

            // Generar la clave aleatoria
            byte[] claveAleatoria = _utilSession.GenerarClaveAleatoria(longitudClaveBytes);

            // Convertir la clave aleatoria a una cadena hexadecimal para su fácil visualización
            string claveAleatoriaHex = BitConverter.ToString(claveAleatoria).Replace("-", "");

            //Session.RemoveAll();
            HttpContext.Session.Remove("ssgenerico");

            HttpContext.Session.SetString("ssgenerico", claveAleatoriaHex);
            return View();
        }

       
    }
}
