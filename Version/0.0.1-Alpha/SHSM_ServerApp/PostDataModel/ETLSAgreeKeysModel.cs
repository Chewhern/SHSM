namespace SHSM_ServerApp.PostDataModel
{
    public class ETLSAgreeKeysModel
    {
        public String User_ID { get; set; }

        public String SignedMACX25519PKB64 { get; set; }

        public String SignedEncryptionX25519PKB64 { get; set; }

        public String EncryptedMACSecretKeyB64 { get; set; }

        public String EncryptedEncryptionSecretKeyB64 { get; set; }

        public Boolean IsKEMOrX25519 { get; set; }

        public String SignedChallengeB64 { get; set; }
    }
}
