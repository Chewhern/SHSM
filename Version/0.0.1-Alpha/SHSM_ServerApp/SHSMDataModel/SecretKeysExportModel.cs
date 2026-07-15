namespace SHSM_ServerApp.SHSMDataModel
{
    public class SecretKeysExportModel
    {
        public String EncryptedEncryptionSecretKey { get; set; }

        public String EncryptedMACSecretKey { get; set; }

        public String StatusString { get; set; }
    }
}
