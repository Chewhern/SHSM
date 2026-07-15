using System;
namespace SHSM_ClientApp.PostDataModel
{
    public class PublicKeyCryptographySignDataModel
    {
        public String User_ID { get; set; }
        
        public String SignedChallengeB64 { get; set; }

        public String DataB64 { get; set; }

        public int IsED25519OrED448OrRSA { get; set; }
    }
}
