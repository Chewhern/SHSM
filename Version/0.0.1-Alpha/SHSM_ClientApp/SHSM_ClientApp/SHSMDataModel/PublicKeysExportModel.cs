using System;
using SHSM_ClientApp.PostDataModel;

namespace SHSM_ClientApp.SHSMDataModel
{
    public class PublicKeysExportModel
    {
        public String EncryptedPrivateKeyB64 { get; set; }

        public EncryptedRSACredentialsModel EncryptedRSAKey { get; set; }

        public String StatusString { get; set; }
    }
}
