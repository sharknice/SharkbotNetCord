using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using SharkbotNetCord.Configuration.Models;
using SharkbotNetCord.Ollama.Models;
using SharkbotNetCord.SharkbotApi.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SharkbotNetCord.SharkbotApi.Services;
using NetCord.Rest;
using NetCord;
using System.Collections.Concurrent;

namespace SharkbotNetCord.Ollama.Services
{
    public class OllamaResponseService(ILogger<MessageCreateHandler> logger, HttpClient client, RestClient restClient, BotConfiguration botConfiguration, ApiUtilityService apiUtilityService)
    {
        private readonly ConcurrentDictionary<string, bool> activeConversations = new ConcurrentDictionary<string, bool>();

        public async ValueTask HandleAsync(Message message, ConversationData conversation)
        {
            if (IsProcessing(conversation.name))
            {
                logger.LogInformation($"ollama already responding to {conversation.name}");
                return;
            }
            StartProcessing(conversation.name);

            var userName = message.Author.Username;

            var messages = new List<OllamaMessage>
            {
                new OllamaMessage { Role = "system", Content = botConfiguration.OllamaSystemPrompt },
                new OllamaMessage { Role = "system", Content = $"Your name is {botConfiguration.BotName}. Do not talk about yourself in third person. Do not talk about sharkbot. Do not break down your answer, just give the answer." }
            };

            if (conversation.groupChat)
            {
                messages.Add(new OllamaMessage { Role = "system", Content = $"You are participating in a group chat." });
            }
            else
            {
                messages.Add(new OllamaMessage { Role = "system", Content = $"You are talking with user:{userName}." });
            }

            var userResponse = await client.GetAsync(botConfiguration.ApiUrl + "/api/user/" + userName);
            userResponse.EnsureSuccessStatusCode();
            var userJsonResponse = await userResponse.Content.ReadAsStringAsync();
            var userData = JsonConvert.DeserializeObject<UserData>(userJsonResponse);
            if (userData.derivedProperties != null)
            {
                foreach (var userProperty in userData.derivedProperties.DistinctBy(p => p.name + p.value))
                {
                    messages.Add(new OllamaMessage { Role = "system", Content = $"{userName}'s {userProperty.name} is {userProperty.value}" });
                }
            }

            messages.Add(new OllamaMessage { Role = "system", Content = $"The time is {DateTime.Now.ToString("F")}" });

            foreach (var chat in conversation.responses)
            {
                if (chat.chat.user == chat.botName)
                {
                    messages.Add(new OllamaMessage { Role = "assistant", Content = chat.chat.message });
                }
                else
                {
                    var content = chat.chat.message;
                    if (conversation.groupChat)
                    {
                        content = $"{chat.chat.user}: " + content;
                    }
                    messages.Add(new OllamaMessage { Role = "user", Content = content });
                }
            }

            var chatRequest = new OllamaChatRequest
            {
                Model = botConfiguration.OllamaModel,
                Messages = messages,
                Stream = false
            };

            var httpContent = apiUtilityService.GetHttpContent(chatRequest);
            var ollamaResponse = await client.PostAsync(botConfiguration.OllamaApiUrl + "/api/chat", httpContent);
            ollamaResponse.EnsureSuccessStatusCode();
            var ollamaJsonResponse = await ollamaResponse.Content.ReadAsStringAsync();
            var ollamaChatResponse = JsonConvert.DeserializeObject<OllamaChatResponse>(ollamaJsonResponse);

            var responseMessage = RemoveThinkTags(ollamaChatResponse.Message.Content);

            if (message.Channel != null)
            {
                message.Channel.TriggerTypingStateAsync();
            }
            else
            {
                var channel = await restClient.GetChannelAsync(message.ChannelId) as TextChannel;
                if (channel != null)
                {
                    channel.TriggerTypingStateAsync();
                }
            }
            var typeTime = 0;
            var formattedChat = apiUtilityService.formatResponse(message, responseMessage);
            typeTime += apiUtilityService.getTypeTime(formattedChat);
            await Task.Delay(typeTime).ContinueWith((task) => { message.SendAsync(formattedChat); });

            FinishProcessing(conversation.name);
        }

        string RemoveThinkTags(string input)
        {
            Console.WriteLine(input);
            var message = Regex.Replace(input, @"<think>.*?</think>", string.Empty, RegexOptions.Singleline);
            return message.Trim();
        }

        void StartProcessing(string name)
        {
            activeConversations.TryAdd(name, true);
            logger.LogInformation($"ollama started responding to {name}");
        }

        void FinishProcessing(string name)
        {
            activeConversations.TryRemove(name, out _);
            logger.LogInformation($"ollama finished responding to {name}");
        }

        bool IsProcessing(string name)
        {
            return activeConversations.ContainsKey(name);
        }
    }
}
