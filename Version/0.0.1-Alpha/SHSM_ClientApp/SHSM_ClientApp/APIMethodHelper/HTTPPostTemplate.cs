using ASodium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_ClientApp.APIMethodHelper
{
    internal class HTTPPostTemplate
    {
        /*
        public async void SamplePostTemplate()
        {
            using var client = new HttpClient();

            string url = "https://api.example.com/resource";

            String[] RequestHeaderValues = new String[] { "fa909a13a4dad78506d5fe0a82d293cc69b2c434bce9e0f547edfd0fc46308a3", "e39cd4ad14b716e0c3b1f0ff2fdf0bbc035299740d559c4e4c46b40a3f60cfea" };

            // Add headers
            client.DefaultRequestHeaders.Add("X-Api-Key", $"{RequestHeaderValues[0]}");
            client.DefaultRequestHeaders.Add("X-Custom-Header", $"{RequestHeaderValues[1]}");

            // Body intentionally omitted (empty content)
            using var content = new StringContent(string.Empty);

            // Send POST request
            HttpResponseMessage response = await client.PostAsync(url, content);

            // Read response
            string responseBody = await response.Content.ReadAsStringAsync();

            SodiumSecureMemory.SecureClearString(RequestHeaderValues[0]);
            SodiumSecureMemory.SecureClearString(RequestHeaderValues[1]);

            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine(responseBody);
        }
        */
    }
}
