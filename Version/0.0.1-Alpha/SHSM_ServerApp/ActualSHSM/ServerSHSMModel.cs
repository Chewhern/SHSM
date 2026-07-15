using ASodium;

namespace SHSM_ServerApp.ActualSHSM
{
    public class ServerSHSMModel
    {
        public KeyPair MACX25519KeyPair { get; set; }

        public KeyPair EncryptionX25519KeyPair { get; set; }

        public KeyPair MyKEMKeyPair { get; set; }
    }
}
