using System;
namespace SHSM_ClientApp.SHSMDataModel
{
    public class SupportedAlgorithmsModel
    {
        public Boolean AbleToSupportSecureAES { get; set; }

        public String[] ListOfAESAlgorithms { get; set; }

        public int[] ListOfAESAlgorithmsIndices { get; set; }

        public String[] ListOfAEADAlgorithms { get; set; }

        public int[] ListOfAEADAlgorithmsIndices { get; set; }

        public String[] StreamCipherAlgorithms { get; set; }

        public int[] StreamCipherAlgorithmsIndices { get; set; }

        public String[] MACAlgorithms { get; set; }

        public int[] MACAlgorithmsIndices { get; set; }

        public String[] DigitalSignatureAlgorithms { get; set; }

        public int[] DigitalSignatureAlgorithmsIndices { get; set; }
        
        public String[] KeyExchangeAlgorithms { get; set; }

        public int[] KeyExchangeAlgorithmsIndices { get; set; }

        public String KEMAlgorithm { get; set; }
    }
}
