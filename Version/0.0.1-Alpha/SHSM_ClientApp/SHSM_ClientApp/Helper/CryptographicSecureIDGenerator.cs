using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using ASodium;

namespace SHSM_ClientApp.Helper
{
    public static class CryptographicSecureIDGenerator
    {
        public static String GenerateUniqueString()
        {
            Byte[] Random_Data = SodiumRNG.GetRandomBytes(128);
            int Loop = 0;
            StringBuilder stringBuilder = new StringBuilder();
            while (Loop < Random_Data.Length)
            {
                if (Random_Data[Loop] >= 48 && Random_Data[Loop] <= 57)
                {
                    stringBuilder.Append((char)Random_Data[Loop]);
                }
                else if (Random_Data[Loop] >= 65 && Random_Data[Loop] <= 90)
                {
                    stringBuilder.Append((char)Random_Data[Loop]);
                }
                else if (Random_Data[Loop] >= 97 && Random_Data[Loop] <= 122)
                {
                    stringBuilder.Append((char)Random_Data[Loop]);
                }
                Loop += 1;
            }
            if (stringBuilder.ToString().CompareTo("") != 0)
            {
                return stringBuilder.ToString();
            }
            else
            {
                return "";
            }
        }

        public static String GenerateMinimumAmountOfUniqueString(int Amount)
        {
            String TestString = GenerateUniqueString();
            while (TestString.Length < Amount)
            {
                TestString += GenerateUniqueString();
            }
            return TestString;
        }
    }
}
