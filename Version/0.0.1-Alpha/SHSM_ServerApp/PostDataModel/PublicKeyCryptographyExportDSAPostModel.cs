namespace SHSM_ServerApp.PostDataModel
{
    public class PublicKeyCryptographyExportDSAPostModel
    {
        public String User_ID { get; set; }

        public String SignedChallengeB64 { get; set; }
        
        public int IsED25519OrED448OrRSA { get; set; } 
        
        public Boolean UseXSalsa20Poly1305 { get; set; }
    }
}
