using SHSM_ClientApp.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_ClientApp.APIMethodHelper
{
    public static class SignupAsSHSMUserHelper
    {
        public static String SignupAsSHSMUser(String JSONString) 
        {
            String OutputString = "";
            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(JSONString, Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(APIIPAddressHelper.IPAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.PostAsync("Registration",content);
                response.Wait();
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var Result = readTask.Result;

                    OutputString = Result.Substring(1, Result.Length - 2);
                }
                else 
                {
                    Console.WriteLine(result.StatusCode.ToString());
                }
            }
            return OutputString;
        }
        
    }
}
