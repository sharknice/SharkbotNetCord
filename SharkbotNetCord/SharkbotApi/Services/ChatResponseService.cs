using NetCord.Gateway;
using Newtonsoft.Json;
using SharkbotNetCord.Configuration.Models;
using SharkbotNetCord.SharkbotApi.Models;

namespace SharkbotNetCord.SharkbotApi.Services
{
    public class ChatResponseService
    {
        HttpClient client;
        ApiUtilityService utilityService;
        BotConfiguration configuration;

        public ChatResponseService(HttpClient httpClient, ApiUtilityService apiUtilityService, BotConfiguration botConfiguration)
        {
            client = httpClient;
            utilityService = apiUtilityService;
            configuration = botConfiguration;
        }

        public async Task<ChatResponse> GetChatResponseAsync(Message e)
        {
            var chat = utilityService.GetChat(e);
            var conversationName = utilityService.GetConversationName(e);
            var chatRequest = new ChatRequest { chat = chat, type = configuration.ChatType, conversationName = conversationName, requestTime = DateTime.Now, exclusiveTypes = configuration.ExclusiveTypes, requiredPropertyMatches = configuration.RequiredProperyMatches };

            var httpContent = utilityService.GetHttpContent(chatRequest);
            var response = await client.PutAsync(configuration.ApiUrl + "/api/chat", httpContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var chatResponse = JsonConvert.DeserializeObject<ChatResponse>(jsonResponse);

            return chatResponse;
        }
    }
}
