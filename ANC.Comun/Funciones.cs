using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ANC.Comun
{
    public class Funciones
    {
        public string GenerarCapcha(int longitud)
        {
            string caracteres = "1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < longitud--)
            {
                res.Append(caracteres[rnd.Next(caracteres.Length)]);
            }
            return res.ToString();
        }
        public string RemoveAccentsWithRegEx(string inputString)
        {
            Regex replace_a_Accents = new Regex("[á|à|ä|â]", RegexOptions.Compiled);
            Regex replace_e_Accents = new Regex("[é|è|ë|ê]", RegexOptions.Compiled);
            Regex replace_i_Accents = new Regex("[í|ì|ï|î]", RegexOptions.Compiled);
            Regex replace_o_Accents = new Regex("[ó|ò|ö|ô]", RegexOptions.Compiled);
            Regex replace_u_Accents = new Regex("[ú|ù|ü|û]", RegexOptions.Compiled);
            inputString = replace_a_Accents.Replace(inputString, "a");
            inputString = replace_e_Accents.Replace(inputString, "e");
            inputString = replace_i_Accents.Replace(inputString, "i");
            inputString = replace_o_Accents.Replace(inputString, "o");
            inputString = replace_u_Accents.Replace(inputString, "u");
            return inputString;
        }
        
        public string cifradoAES(string data, string clave)
        {
            string correoCifrado = string.Empty;
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(clave);
                    aes.IV = new byte[16]; // inicialización del vector de inicialización
                    byte[] correoBytes = Encoding.UTF8.GetBytes(data);
                    // Cifrar los datos utilizando AES
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(correoBytes, 0, correoBytes.Length);
                            cs.FlushFinalBlock();
                        }
                        byte[] cifradoBytes = ms.ToArray();
                        correoCifrado = Convert.ToBase64String(cifradoBytes);
                        correoCifrado = HttpUtility.UrlEncode(correoCifrado);
                    }
                }
            }
            catch (Exception e)
            {
                return "ERROR";
            }
            


            return correoCifrado;
        }

        public bool ValidarLongitudesIguales(params object[][] arrays)
        {
            // Verificar si todos los arrays son nulos o están vacíos
            if (arrays.All(a => a == null || ((Array)a).Length == 0))
            {
                return true;
            }

            // Verificar si algún array es nulo o vacío mientras otros tienen datos
            if (arrays.Any(a => a == null || ((Array)a).Length == 0) && arrays.Any(a => a != null && ((Array)a).Length > 0))
            {
                return false;
            }

            // Verificar si todos los arrays tienen la misma longitud
            int length = -1;
            foreach (var array in arrays)
            {
                if (array == null)
                {
                    continue;
                }

                int currentLength = ((Array)array).Length;
                if (length == -1)
                {
                    length = currentLength;
                }
                else if (currentLength != length)
                {
                    return false;
                }
            }

            // Si se cumplen todas las condiciones anteriores, las longitudes son iguales
            return true;
        }
        public bool ValidarNullOEmptyArray(params object[] arrays)
        {
            // Verificar si no hay arrays o si algún array es nulo
            if (arrays == null || arrays.Length == 0 || arrays.Any(a => a == null))
            {
                return false;
            }

            // Verificar si algún elemento en los arrays es una cadena vacía
            foreach (var array in arrays)
            {
                // Si el elemento del array no es una cadena, no lo podemos validar, así que lo ignoramos
                if (array is string[] stringArray)
                {
                    if (stringArray.Any(s => string.IsNullOrEmpty(s)))
                    {
                        return false;
                    }
                }
                else
                {
                    // Si el elemento del array no es una cadena, no podemos validar, así que lo ignoramos
                    if (array is Array otherArray)
                    {
                        foreach (var item in otherArray)
                        {
                            if (item == null)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
