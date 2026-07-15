using System;
namespace SHSM_ClientApp.SHSMDataModel
{
    public class ServerInitialETLSDataModel
    {
        public String MACX25519PublicKeyB64 { get; set; }

        public String EncryptionX25519PublicKeyB64 { get; set; }

        public String KEMPublicKeyB64 { get; set; }

        public Boolean IsKEMOrX25519 { get; set; }
    }
}
