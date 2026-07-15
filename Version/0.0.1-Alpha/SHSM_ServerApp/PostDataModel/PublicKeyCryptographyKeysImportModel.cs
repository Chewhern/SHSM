namespace SHSM_ServerApp.PostDataModel
{
    public class PublicKeyCryptographyKeysImportModel
    {
        public String User_ID { get; set; }

        public String SignedChallengeB64 { get; set; }

        public int IsDSOrSealedBoxOrKEMKeyType { get; set; }

        public EncryptedRSACredentialsModel MyRSAKey { get; set; }

        public String EncryptedPrivateKeyB64 { get; set; }

        public Boolean IsKEMOrSealedBox { get; set; }

        public int IsED25519OrED448OrRSA { get; set; }

        public Boolean IsXSalsa20Poly1305OrXChaCha20Poly1305 { get; set; }
    }
}
