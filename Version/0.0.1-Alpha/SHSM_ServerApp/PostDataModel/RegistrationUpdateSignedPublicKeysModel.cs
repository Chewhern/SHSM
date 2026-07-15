using SHSM_ServerApp.SHSMDataModel;

namespace SHSM_ServerApp.PostDataModel
{
    public class RegistrationUpdateSignedPublicKeysModel
    {
        public String User_ID { get; set; }

        public SignedPublicKeysModel DataUpdateModel { get; set; }

        public String SignedChallengeB64 { get; set; }
    }
}
