namespace SHSM_ServerApp.RSACredentials
{
    public class RSACredentialsModel
    {
        public Byte[] Exponent { get; set; }

        public Byte[] Modulus { get; set; }

        public Byte[] D { get; set; }

        public Byte[] P { get; set; }

        public Byte[] Q { get; set; }

        public Byte[] DP { get; set; }

        public Byte[] DQ { get; set; }

        public Byte[] InverseQ { get; set; }
    }
}
