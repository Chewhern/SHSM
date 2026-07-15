using System;
namespace SHSM_ClientApp.SHSMDataModel
{
    public class SecretKeysExportModel
    {
        public String EncryptedEncryptionSecretKey { get; set; }

        public String EncryptedMACSecretKey { get; set; }

        public String StatusString { get; set; }
    }
}
