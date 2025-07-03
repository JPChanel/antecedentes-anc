using System.Net.NetworkInformation;

namespace ANC.AntecedentesWeb.UtilesPC
{
    public class Util
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public  Util(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public static string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }

       
        public  string GetIPAddress()
        {
            var context = _httpContextAccessor.HttpContext;
            string ipAddress = context.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
            {
                // Intenta obtener la dirección IP del encabezado X-Forwarded-For si está presente
                ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                // Si la dirección IP es nula o está en formato de loopback, usa la dirección remota de la conexión
                if (string.IsNullOrEmpty(ipAddress) || ipAddress.Contains("::1"))
                {
                    ipAddress = context.Connection.RemoteIpAddress?.ToString();
                }
            }

            return ipAddress;
        }

        public static string GetOperatingSystem(string userAgent)
        {

            // Por ejemplo, una forma simple de obtener el sistema operativo podría ser buscar ciertos patrones en el User-Agent:
            if (userAgent.Contains("Windows"))
            {
                return "Windows";
            }
            else if (userAgent.Contains("Macintosh"))
            {
                return "Mac OS";
            }
            else if (userAgent.Contains("Linux"))
            {
                return "Linux";
            }
            else
            {
                return "Desconocido";
            }
        }
    }
}
