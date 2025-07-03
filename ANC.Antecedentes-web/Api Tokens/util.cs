using System.Security.Cryptography;

namespace ANC.AntecedentesWeb.Api_Tokens
{
    public class util
    {
        public  byte[] GenerarClaveAleatoria(int longitud)
        {
            // Crear un generador de números aleatorios criptográficamente seguro
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] claveAleatoria = new byte[longitud];
                rng.GetBytes(claveAleatoria);
                return claveAleatoria;
            }
        }
    }
}
