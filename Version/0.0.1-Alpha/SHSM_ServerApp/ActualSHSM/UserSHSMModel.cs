using ASodium;
using BCASodium;
using SHSM_ServerApp.RSACredentials;
using SHSM_ServerApp.SHSMDataModel;

namespace SHSM_ServerApp.ActualSHSM
{
    public class UserSHSMModel
    {
        public RegisteredUserModel RegisteredUser { get; set; }

        public IntPtr SecretKeyForEncryption { get; set; }

        public IntPtr SecretKeyForMAC { get; set; }

        public KeyPair ED25519DigitalSignatureKP { get; set; }

        public IntPtr ED255119SecretKey { get; set; }

        public ED448RevampedKeyPair ED448DigitalSignatureRKP { get; set; }
        //Unless specifically work on some parts from bouncycastle
        //else can't prevent it from ended up in swap partitions..

        public Byte[] ED448SecretKey { get; set; }

        public RSACredentialsModel RSAKeyParts { get; set; }

        public IntPtr X25519EncryptionSecretKey { get; set; }

        public IntPtr EncryptionKEMKPSecretKey { get; set; }

        public DateTime ValidDateTime { get; set; }

        public ServerSHSMModel TempServerSHSM { get; set; }
    }
}
