using NetCord.Gateway;
using Newtonsoft.Json;
using SharkbotNetCord.Configuration.Models;
using SharkbotNetCord.SharkbotApi.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace SharkbotNetCord.SharkbotApi.Services
{
    public class ApiUtilityService
    {
        long launchTime;
        BotConfiguration configuration;

        public ApiUtilityService(BotConfiguration botConfiguration)
        {
            configuration = botConfiguration;
            launchTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public StringContent GetHttpContent(dynamic request)
        {
            var jsonString = JsonConvert.SerializeObject(request);
            return new StringContent(jsonString, Encoding.UTF8, "application/json");
        }

        public string GetConversationName(Message message)
        {
            if (message.Channel == null)
            {
                return configuration.ChatType + "-discord-" + message.ChannelId + "-" + launchTime;
            }
            return configuration.ChatType + "-discord-" + message.Channel.Id + "-" + launchTime;
        }

        public Chat GetChat(Message discordMessage)
        {
            var message = discordMessage.Content;
            foreach (var mention in discordMessage.MentionedUsers)
            {
                message = message.Replace(mention.ToString(), "@" + mention.Username);
            }
            return new Chat { botName = configuration.BotName, message = message, user = discordMessage.Author.Username, time = DateTimeOffset.Now.ToUnixTimeMilliseconds() };
        }

        public string formatResponse(Message e, string chat)
        {
            var response = chat.Trim();

            if (response.StartsWith("/me "))
            {
                response = $"*{response}*";
            }

            var regex = new Regex("@(?<name>[^\\s]+)");
            var results = regex.Matches(response)
                .Cast<Match>()
                .Select(m => m.Groups["name"].Value)
                .ToArray();

            if (e.Channel != null)
            {
                foreach (var userName in results)
                {
                    if (e.Author.Username == userName)
                    {
                        var mention = $"<@{e.Author.Id}>";
                        response = response.Replace("@" + userName, mention);
                    }
                }
            }

            return response;
        }

        public int getTypeTime(string message)
        {
            return message.Length * 80;
        }
    }
}
