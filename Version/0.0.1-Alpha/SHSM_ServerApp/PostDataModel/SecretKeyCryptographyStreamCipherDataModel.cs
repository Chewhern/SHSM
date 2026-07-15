namespace SHSM_ServerApp.PostDataModel
{
    public class SecretKeyCryptographyStreamCipherDataModel
    {
        public String User_ID { get; set; }

        public String SignedChallengeB64 { get; set; }

        public String Base64DataOrCipherText { get; set; }

        public int StreamCipherAlgorithmIndex { get; set; }

        public int MACAlgorithmIndex { get; set; }
    }
}
