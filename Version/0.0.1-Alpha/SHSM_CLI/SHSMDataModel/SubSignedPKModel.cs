using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_CLI.SHSMDataModel
{
    public class SubSignedPKModel
    {
        public String SignedDigitalSignaturePublicKeyB64 { get; set; }

        public String DigitalSignatureAlgorithm { get; set; }

        public int ValidDate_Day { get; set; }

        public int ValidDate_Month { get; set; }

        public int ValidDate_Year { get; set; }
    }
}
