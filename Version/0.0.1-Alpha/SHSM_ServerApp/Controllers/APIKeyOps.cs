using ASodium;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SHSM_ServerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIKeyOps : ControllerBase
    {
        //Import API Keys via sealedbox
        //create a sample template to mimic sample post/get..

        //
        public async void SampleGetTemplate() 
        {
            using var client = new HttpClient();

            //Assume the parametervalue as client_id and client_secret from oauth
            //but at the end.. other similar value acting like API keys can be dealt this way..
            String[] ParameterValue = new String[] { "8110847a36776028a60f1b9d72f8c22a5e1c2690c7ddbdd0bc75cff3a4aec5d8", "1a7d784d0dda0edeef444ad32e47a6ce14108e8d2db23ad788d1c0897cff495e" };
            String QueryString = "Param1=" + ParameterValue[0] + "&Param2=" + ParameterValue[1];

            // Base URL + query parameters
            string baseUrl = "https://api.example.com/resource";
            string url = $"{baseUrl}?{QueryString}";

            String[] RequestHeaderValues = new String[] { "fa909a13a4dad78506d5fe0a82d293cc69b2c434bce9e0f547edfd0fc46308a3", "e39cd4ad14b716e0c3b1f0ff2fdf0bbc035299740d559c4e4c46b40a3f60cfea" };

            // Add headers
            client.DefaultRequestHeaders.Add("X-Api-Key", $"{RequestHeaderValues[0]}");
            client.DefaultRequestHeaders.Add("X-Custom-Header", $"{RequestHeaderValues[1]}");

            // Send GET request
            HttpResponseMessage response = await client.GetAsync(url);

            // Read response or can return value if wanted..
            string content = await response.Content.ReadAsStringAsync();

            //API key protection..
            //on one hand it's software based side-channel attacks
            //on the other hand, on the client side, the API keys' values must not be exposed as plaintext
            //====Speaking from the first problem or issue====
            //In C#, only the last copy of String can be cleared..
            //Most cases, this is better than most intermediary languages as there's a way to clear the string
            //even if it's the last copy.
            //Go, Java, Python, NodeJS/TypeScript .. lacks either GCHandle or IntPtr or both.
            //====Speaking from the second problem or issue====
            //Pre-encrypt the API keys using sealedbox from ETLS or secret keys that were imported
            //The encrypted API keys will then be decrypted from SHSM server side..
            //The server side then will help to make http calls on behalf of the developer/client..
            SodiumSecureMemory.SecureClearString(ParameterValue[0]);
            SodiumSecureMemory.SecureClearString(ParameterValue[1]);
            SodiumSecureMemory.SecureClearString(RequestHeaderValues[0]);
            SodiumSecureMemory.SecureClearString(RequestHeaderValues[1]);

            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine(content);

            //There're no exact templates on how an exact httpget or httppost or other equivalent templates
            //will look like as it's highly customizable and no fixed standards.
        }

        //In the bracket, kindly refer to either "ArweaveRSAOps" or "PublicKeyCryptography" or "Registration" or "SecretKeyCryptography"
        //'s HTTPPOST on how a proper post should look like on a controller level..
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
    }
}
