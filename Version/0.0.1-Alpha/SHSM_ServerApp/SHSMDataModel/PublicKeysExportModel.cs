using SHSM_ServerApp.PostDataModel;

namespace SHSM_ServerApp.SHSMDataModel
{
    public class PublicKeysExportModel
    {
        public String EncryptedPrivateKeyB64 { get; set; }

        public EncryptedRSACredentialsModel EncryptedRSAKey { get; set; }

        public String StatusString { get; set; }
    }
}
