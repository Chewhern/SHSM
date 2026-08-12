using SHSM_CLI.DirectoryHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_CLI.APIMethodHelper
{
    public static class SecretKeyCryptoDecryptHelper
    {
        public static String SecretKeyCryptoDecrypt(String JSONString)
        {
            String OutputString = "";
            String API_IPAddress = File.ReadAllText(StandardizedDirectoriesFunction.ServerRootFolder + "IP.txt");
            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(JSONString, Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(API_IPAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.PostAsync("SecretKeyCryptography/Decrypt", content);
                response.Wait();
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var Result = readTask.Result;

                    OutputString = Result.Substring(1, Result.Length - 2);
                }
            }
            return OutputString;
        }
    }
}
