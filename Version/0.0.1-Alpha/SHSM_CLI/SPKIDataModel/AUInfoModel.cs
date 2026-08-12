using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_CLI.SPKIDataModel
{
    public class AUInfoModel
    {
        public String User_ID { get; set; }

        public String Public_Contact { get; set; }

        public String Private_Contact { get; set; }

        public String Sign_PK { get; set; }

        public String Auth_PK { get; set; }

        public String OOB_PK { get; set; }

        public String DSA_Type { get; set; }

        //Better for datetime parsing as different OS and device
        //along with different regions have different date time format
        public int ValidDate_Day { get; set; }

        public int ValidDate_Month { get; set; }

        public int ValidDate_Year { get; set; }
    }
}
