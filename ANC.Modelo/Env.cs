using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANC.Modelo
{
    public class Env
    {
        //Integrado
        public static string _connectionApiEndPoint;
        public static string _connectionToken;

        public static string _user_auth;
        public static string _password_auth;
        public static string _api_url_auth;
        public static string _url_reniec;
        public static void SetConnectionStrings(string ConnectionApiEndPint,string ConnectionToken, string user_auth, string password_auth, string api_url_auth, string url_reniec)
        {
            _connectionApiEndPoint = ConnectionApiEndPint;
            _connectionToken = ConnectionToken;
            _user_auth = user_auth;
            _password_auth = password_auth;
            _api_url_auth = api_url_auth;
            _url_reniec = url_reniec;
        }

    }
}
