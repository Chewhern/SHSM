using System;
namespace SHSM_ClientApp.PostDataModel
{
    public class PublicKeyCryptographyDecryptDataModel
    {
        public String User_ID { get; set; }

        public String SignedChallengeB64 { get; set; }

        public String EncryptedDataB64 { get; set; }

        public Boolean IsSealedBoxOrKEM { get; set; }

        public Boolean IsXSalsa20Poly1305OrXChaCha20Poly1305 { get; set; }

        public String KEMEncryptionPKB64 { get; set; }
    }
}
