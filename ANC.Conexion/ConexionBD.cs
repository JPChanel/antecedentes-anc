using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANC.Conexion
{
    public class ConexionBD
    {
        private static string _ConnectionSybaseUsis;
        private static string _ConnectionSybaseZav;
        private static string _ConnectionPG;
        public static void SetConnectionStrings(string ConnectionSybaseUsis, string ConnectionSybaseZav,string ConnectionPG)
        {
            _ConnectionSybaseUsis = ConnectionSybaseUsis;
            _ConnectionSybaseZav = ConnectionSybaseZav;
            _ConnectionPG = ConnectionPG;
        }

        public static  string ConnectionString
        {
            get
            {
                return _ConnectionSybaseUsis;
               
            }
        }

        public static string ConnectionStringZavala
        {
            get
            {

                return _ConnectionSybaseZav;
             

            }
        }
        public static string ConnectionStringPostgres
        {
            get
            {
                return _ConnectionPG;

              
            }
        }

    }
}
