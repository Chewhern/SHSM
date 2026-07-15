using System;
namespace SHSM_ClientApp.PostDataModel
{
    public class SecretKeyCryptographyAEADDataModel
    {
        public String User_ID { get; set; }

        public String SignedChallengeB64 { get; set; }

        public String Base64DataOrCipherText { get; set; }

        public String AdditionalDataB64 { get; set; }

        public int AEADAlgorithmIndex { get; set; }
    }
}
