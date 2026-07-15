using System;
using SHSM_ClientApp.SHSMDataModel;

namespace SHSM_ClientApp.PostDataModel
{
    public class RegistrationUpdateSignedPublicKeysModel
    {
        public String User_ID { get; set; }

        public SignedPublicKeysModel DataUpdateModel { get; set; }

        public String SignedChallengeB64 { get; set; }
    }
}
