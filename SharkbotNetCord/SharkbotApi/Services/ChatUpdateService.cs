using NetCord.Gateway;
using Newtonsoft.Json;
using SharkbotNetCord.Configuration.Models;
using SharkbotNetCord.SharkbotApi.Models;

namespace SharkbotNetCord.SharkbotApi.Services
{
    public class ChatUpdateService
    {
        HttpClient client;
        ApiUtilityService utilityService;
        BotConfiguration configuration;

        public ChatUpdateService(HttpClient httpClient, ApiUtilityService apiUtilityService, BotConfiguration botConfiguration)
        {
            client = httpClient;
            utilityService = apiUtilityService;
            configuration = botConfiguration;
        }

        public async Task<bool> UpdateChatAsync(Message e)
        {
            var chat = utilityService.GetChat(e);
            var conversationName = utilityService.GetConversationName(e);
            var chatRequest = new ChatRequest { chat = chat, type = configuration.ChatType, conversationName = conversationName };

            var httpContent = utilityService.GetHttpContent(chatRequest);
            var response = await client.PutAsync(configuration.ApiUrl + "/api/chatupdate", httpContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var success = JsonConvert.DeserializeObject<bool>(jsonResponse);

            return success;
        }
    }
}
