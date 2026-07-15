using SHSM_ClientApp.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SHSM_ClientApp.APIMethodHelper
{
    public static class SecretKeyCryptoExtendDurationHelper
    {
        public static string SecretKeyCryptoExtendDuration(string User_ID, string SignedChallengeB64) 
        {
            string StatusString = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(APIIPAddressHelper.IPAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync($"SecretKeyCryptography/ExtendDuration?User_ID={User_ID}&SignedChallengeB64={HttpUtility.UrlEncode(SignedChallengeB64)}");
                response.Wait();
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var Result = readTask.Result;

                    StatusString = Result.Substring(1, Result.Length - 2);
                }
            }
            return StatusString;
        }
    }
}
