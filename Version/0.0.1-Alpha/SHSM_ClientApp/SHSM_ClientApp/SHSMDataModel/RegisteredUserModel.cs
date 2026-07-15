using System;
namespace SHSM_ClientApp.SHSMDataModel
{
    public class RegisteredUserModel
    {
        public String User_ID { get; set; }

        public String AUInfoModelArweaveID { get; set; }

        public String SubSignedDSPKArweaveID { get; set; }

        public SignedPublicKeysModel UserSignedPublicKeys { get; set; }
    }
}
