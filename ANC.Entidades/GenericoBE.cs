namespace ANC.Entidades
{
    public class GenericoBE
    {

    }
    public class PetTipoDocBE
    {
        public int codtipodoc { get; set; }
        public string descripcion { get; set; }
        public int longituddoc { get; set; }
        public int longitudexacta { get; set; }
        public int numericalfanum { get; set; }
    }
    public class DepartamentoBE
    {
        public string cod_departamento { get; set; }
        public string des_nombre { get; set; }
    }
    public class ProvinciaBE
    {
        public string cod_provincia { get; set; }
        public string des_nombre { get; set; }
    }
    public class DistritoBE
    {
        public string cod_distrito { get; set; }
        public string des_nombre { get; set; }
    }


    public class CredencialesFTPBE
    {
        public string user_ftp { get; set; }
        public string user_ftp2 { get; set; }
        public string pass_ftp { get; set; }
        public string pass_ftp2 { get; set; }
        public string fla_acti { get; set; }
        public string server_ftp { get; set; }
        public string server_ftp2 { get; set; }
        public int puerto_ftp { get; set; }
        public string server_sftp { get; set; }
        public string user_sftp { get; set; }
        public string pass_sftp { get; set; }
        public int puerto_sftp { get; set; }
        public double sinoe2_mb { get; set; }
        public string user_ftp3 { get; set; }
        public string pass_ftp3 { get; set; }
        public string server_ftp3 { get; set; }
        public int puerto_ftp3 { get; set; }

        public string key_sftp { get; set; }
        public string ruta_sinoe_sftp { get; set; }


        public string user_ftp4 { get; set; }
        public string pass_ftp4 { get; set; }
        public string server_ftp4 { get; set; }
        public int puerto_ftp4 { get; set; }


    }

    public class RespuestaBE
    {
        public string codigo { get; set; }
        public string valor { get; set; }
        public byte[] documento { get; set; }

    }
}
