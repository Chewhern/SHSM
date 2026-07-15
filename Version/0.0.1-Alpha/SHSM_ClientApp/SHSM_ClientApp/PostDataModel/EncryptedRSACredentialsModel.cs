using System;
namespace SHSM_ClientApp.PostDataModel
{
    public class EncryptedRSACredentialsModel
    {
        public String EncryptedExponentB64 { get; set; }

        public String EncryptedModulusB64 { get; set; }

        public String EncryptedDB64 { get; set; }

        public String EncryptedPB64 { get; set; }

        public String EncryptedQB64 { get; set; }

        public String EncryptedDPB64 { get; set; }

        public String EncryptedDQB64 { get; set; }

        public String EncryptedInverseQB64 { get; set; }
    }
}
