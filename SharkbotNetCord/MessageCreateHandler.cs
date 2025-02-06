using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using Newtonsoft.Json;
using SharkbotNetCord.Configuration.Models;
using SharkbotNetCord.ImageGeneration;
using SharkbotNetCord.Ollama.Services;
using SharkbotNetCord.SharkbotApi.Models;
using SharkbotNetCord.SharkbotApi.Services;
using System.Text.RegularExpressions;
using NetCord.Rest;
using NetCord;

namespace SharkbotNetCord
{
    [GatewayEvent(nameof(GatewayClient.MessageCreate))]
    public class MessageCreateHandler(ILogger<MessageCreateHandler> logger, HttpClient client, RestClient restClient, BotConfiguration botConfiguration, OllamaResponseService ollamaResponseService, ApiUtilityService apiUtilityService, ChatUpdateService chatUpdateService, ChatResponseService chatResponseService, DirectedReplyService directedReplyService, ImageResponseUtility imageResponseUtility, ImageGenerationService imageGenerationService, GenerateImageResponseService generateImageResponseService) : IGatewayEventHandler<Message>
    {
        Random random = new Random();

        public async ValueTask HandleAsync(Message message)
        {
            logger.LogInformation("{}", message.Content);

            var imageGenerationText = imageResponseUtility.AskingForImageResponse(message);
            var result = await chatResponseService.GetChatResponseAsync(message);
            if (!message.Author.IsBot)
            {
                var replied = false;
                var typeTime = 0;
                foreach (var chat in result.response)
                {
                    var emojiRegex = new Regex(@"^(\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff])+$", RegexOptions.Compiled);
                    if (emojiRegex.IsMatch(chat.Trim()))
                    {
                        if (result.confidence > botConfiguration.ReactionConfidenceThreshold)
                        {
                            await message.AddReactionAsync(new ReactionEmojiProperties(chat.Trim()));
                        }
                    }
                    else if (imageGenerationText == null)
                    {
                        if (result.confidence > botConfiguration.TargetedResponseConfidenceThreshold)
                        {
                            if (message.Channel != null)
                            {
                                await Task.Delay(typeTime).ContinueWith((task) => { message.Channel.TriggerTypingStateAsync(); });
                            }
                            else
                            {
                                var channel = await restClient.GetChannelAsync(message.ChannelId) as TextChannel;
                                if (channel != null)
                                {
                                    await Task.Delay(typeTime).ContinueWith((task) => { channel.TriggerTypingStateAsync(); });
                                }
                            }
                            var formattedChat = apiUtilityService.formatResponse(message, chat);
                            typeTime += apiUtilityService.getTypeTime(formattedChat);
                            await Task.Delay(typeTime).ContinueWith((task) => { message.ReplyAsync(formattedChat); });
                            replied = true;
                        }
                    }
                }
                if (replied)
                {
                    return;
                }
            }

            var conversationName = apiUtilityService.GetConversationName(message);
            var response = await client.GetAsync(botConfiguration.ApiUrl + "/api/conversation/discord/" + conversationName);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var conversation = JsonConvert.DeserializeObject<ConversationData>(jsonResponse);

            if (message.Author.IsBot)
            {
                return;
            }

            if (imageGenerationText != null)
            {
                await message.AddReactionAsync(new ReactionEmojiProperties("👌"));

                var imagePath = await imageGenerationService.GenerateImageResponseAsync(message, imageGenerationText.Text, imageGenerationText.UserName);
                generateImageResponseService.GenerateImageResponse(message, imagePath);
                return;
            }

            var randomChance = random.NextDouble();
            if (botConfiguration.OllamaChance > randomChance)
            {
                await ollamaResponseService.HandleAsync(message, conversation);
            }
            else
            {
                var directedReplyConfidence = directedReplyService.DirectedReply(conversation, message);
                if (botConfiguration.OllamaReplyChance * directedReplyConfidence > randomChance)
                {
                    await ollamaResponseService.HandleAsync(message, conversation);
                }
            }
        }
    }
}