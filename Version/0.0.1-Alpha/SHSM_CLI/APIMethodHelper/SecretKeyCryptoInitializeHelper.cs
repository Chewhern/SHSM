using SHSM_CLI.DirectoryHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SHSM_CLI.APIMethodHelper
{
    public static class SecretKeyCryptoInitializeHelper
    {
        public static String SecretKeyCryptoInitialize(String User_ID, String SignedChallengeB64)
        {
            String StatusString = "";
            String API_IPAddress = File.ReadAllText(StandardizedDirectoriesFunction.ServerRootFolder + "IP.txt");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_IPAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync($"SecretKeyCryptography?User_ID={User_ID}&SignedChallengeB64={HttpUtility.UrlEncode(SignedChallengeB64)}");
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
