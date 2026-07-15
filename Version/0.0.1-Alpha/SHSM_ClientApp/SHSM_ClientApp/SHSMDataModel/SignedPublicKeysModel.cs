using System;
namespace SHSM_ClientApp.SHSMDataModel
{
    public class SignedPublicKeysModel
    {
        public String[] SubSignedPublicKeysB64 { get; set; }

        public Boolean[] IsKEMorSealedBox { get; set; }

        public String[] AlgorithmTypes { get; set; }
    }
}
