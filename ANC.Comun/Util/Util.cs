using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANC.Comun.Util
{
    public abstract class Util
    {
        public static void ValidarCarpeta(string carpeta)
        {
            if (!System.IO.Directory.Exists(carpeta))
            {
                System.IO.Directory.CreateDirectory(carpeta);
            }
        }
        public static byte[] ConvertBase64ToBytes(string base64String)
        {
            try
            {
                return Convert.FromBase64String(base64String);
            }
            catch (FormatException ex)
            {
                throw new Exception("Error al convertir la cadena Base64 a datos binarios: " + ex.Message);
            }
        }
    }
}
