using SHSM_ClientApp.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_ClientApp.APIMethodHelper
{
    public static class GetChallengeForSHSMRegisteredUserHelper
    {
        public static Byte[] GetChallenge(String User_ID)
        {
            String ChallengeB64String = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(APIIPAddressHelper.IPAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync("ChallengeRequestor?User_ID="+User_ID);
                response.Wait();
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var Result = readTask.Result;

                    ChallengeB64String = Result.Substring(1, Result.Length - 2); ;
                }
            }
            if (ChallengeB64String.CompareTo("") != 0) 
            {
                try
                {
                    Byte[] ChallengeBytes = Convert.FromBase64String(ChallengeB64String);
                    return ChallengeBytes;
                }
                catch 
                {
                    return new Byte[] { };
                }
            }
            else 
            {
                return new Byte[] { };
            }
        }
    }
}
