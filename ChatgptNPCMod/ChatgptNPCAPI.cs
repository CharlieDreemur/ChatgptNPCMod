using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChatgptNPCMod
{
    public class ChatgptNPCAPI
    {
        private static readonly HttpClient httpClient = new HttpClient();
        //private static readonly string apiKey = "sk - ldDPvT9nRiBlGRmsdL67T3BlbkFJpLDmBiyRBZx7rZizqcRI";

        public static async Task<string> SendChatgptMessage(string message)
        {
            string apiUrl = "http://localhost:3000/conversation";

            var requestData = new
            {
                message
            };

            var jsonContent = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody.Trim();
        }
        public static async Task<string> GetParseMessage(string message)
        {
            string result = "";
            try
            {
                string apiResponse = await SendChatgptMessage(message);
                //ChatgptNPC.MyLogger.LogInfo($"Chatgpt API response: {apiResponse}");
                JObject jsonObj = JObject.Parse(apiResponse);
                result = jsonObj["response"].ToString();
                ChatgptNPC.MyLogger.LogInfo("Parse\n" + result);
            }
            catch (Exception ex)
            {
                ChatgptNPC.MyLogger.LogError(ex.ToString());
            }
            return result;
        }
    }
}
