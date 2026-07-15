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
    public static class GetNodeInformationHelper
    {
        public static String GetNodeInfo()
        {
            String NodeInformation = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(APIIPAddressHelper.IPAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync("ServerInformation");
                response.Wait();
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var Result = readTask.Result;

                    //NodeInformation = Result.Substring(1, Result.Length - 2);
                    NodeInformation = Result;
                }
            }
            return NodeInformation;
        }
    }
}
