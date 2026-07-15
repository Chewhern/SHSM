using System;
namespace SHSM_ClientApp.PostDataModel
{
    public class SecretKeyCryptographyImportKeysDataModel
    {
        public String User_ID { get; set; }

        public String SignedChallengeB64 { get; set; }

        public String EncryptedMACSecretKeyB64 { get; set; }

        public String EncryptedEncryptionSecretKeyB64 { get; set; }

        public Boolean IsKEMOrSealedBox { get; set; }

        public Boolean IsXSalsa20Poly1305OrXChaCha20Poly1305 { get; set; }
    }
}
