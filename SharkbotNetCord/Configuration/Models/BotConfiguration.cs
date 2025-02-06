using Microsoft.Extensions.Configuration;

namespace SharkbotNetCord.Configuration.Models
{
    public class BotConfiguration
    {
        public BotConfiguration(IConfiguration configuration) 
        {
            Token = configuration.GetSection("Token").Value;
            ApiUrl = configuration.GetSection("ApiUrl").Value;
            ImageApiUrl = configuration.GetSection("ImageApiUrl").Value;
            ImageApiDirectory = configuration.GetSection("ImageApiDirectory").Value;
            MusicApiUrl = configuration.GetSection("MusicApiUrl").Value;
            BotName = configuration.GetSection("BotName").Value;
            ChatType = configuration.GetSection("ChatType").Value;

            IgnoredChannels = configuration.GetSection("IgnoredChannels").Get<List<string>>();
            if (IgnoredChannels == null)
            {
                IgnoredChannels = new List<string>();
            }

            ExclusiveTypes = configuration.GetSection("ExclusiveTypes").Get<List<string>>();
            if (ExclusiveTypes == null)
            {
                ExclusiveTypes = new List<string>();
            }

            RequiredProperyMatches = configuration.GetSection("RequiredProperyMatches").Get<List<string>>();
            if (RequiredProperyMatches == null)
            {
                RequiredProperyMatches = new List<string>();
            }

            NickNames = configuration.GetSection("NickNames").Get<List<string>>();
            if (NickNames == null)
            {
                NickNames = new List<string>();
            }

            var targetedResponseConfidenceThreshold = configuration.GetSection("TargetedResponseConfidenceThreshold");
            TargetedResponseConfidenceThreshold = 0;
            if (targetedResponseConfidenceThreshold.Value != null)
            {
                TargetedResponseConfidenceThreshold = double.Parse(targetedResponseConfidenceThreshold.Value);
            }

            var reactionConfidenceThreshold = configuration.GetSection("ReactionConfidenceThreshold");
            ReactionConfidenceThreshold = 0;
            if (reactionConfidenceThreshold.Value != null)
            {
                ReactionConfidenceThreshold = double.Parse(reactionConfidenceThreshold.Value);
            }

            var maximumReactionsPerMessage = configuration.GetSection("MaximumReactionsPerMessage");
            MaximumReactionsPerMessage = 1;
            if (maximumReactionsPerMessage.Value != null)
            {
                MaximumReactionsPerMessage = int.Parse(maximumReactionsPerMessage.Value);
            }

            DefaultResponse = configuration.GetSection("DefaultResponse").Value;
            var defaultResponse = configuration.GetSection("DefaultResponse");
            DefaultResponse = string.Empty;
            if (defaultResponse.Value != null)
            {
                DefaultResponse = defaultResponse.Value;
            }

            MongoDbConnectionString = null;
            var connectionStringSection = configuration.GetSection("MongoDbConnectionString");
            if (connectionStringSection.Value != null)
            {
                MongoDbConnectionString = connectionStringSection.Value;
            }

            var ollamaApiUrl = configuration.GetSection("OllamaApiUrl");
            DefaultResponse = string.Empty;
            if (ollamaApiUrl.Value != null)
            {
                OllamaApiUrl = ollamaApiUrl.Value;
            }

            var ollamaModel = configuration.GetSection("OllamaModel");
            OllamaModel = string.Empty;
            if (ollamaModel.Value != null)
            {
                OllamaModel = ollamaModel.Value;
            }

            var ollamaSystemPrompt = configuration.GetSection("OllamaSystemPrompt");
            OllamaSystemPrompt = string.Empty;
            if (ollamaSystemPrompt.Value != null)
            {
                OllamaSystemPrompt = ollamaSystemPrompt.Value;
            }

            var ollamaConfidence = configuration.GetSection("OllamaConfidence");
            OllamaConfidence = 0;
            if (ollamaConfidence.Value != null)
            {
                OllamaConfidence = double.Parse(ollamaConfidence.Value);
            }

            var ollamaChance = configuration.GetSection("OllamaChance");
            OllamaChance = 0;
            if (ollamaChance.Value != null)
            {
                OllamaChance = double.Parse(ollamaChance.Value);
            }

            var ollamaReplyChance = configuration.GetSection("OllamaReplyChance");
            OllamaReplyChance = 0;
            if (ollamaReplyChance.Value != null)
            {
                OllamaReplyChance = double.Parse(ollamaReplyChance.Value);
            }
        }

        public string Token { get; set; }
        public string ApiUrl { get; set; }
        public string OllamaApiUrl { get; set; }
        public string ImageApiUrl { get; set; }
        public string MusicApiUrl { get; set; }

        public string OllamaModel { get; set; }
        public string OllamaSystemPrompt { get; set; }
        public double OllamaConfidence { get; set; }
        public double OllamaChance { get; set; }
        public double OllamaReplyChance { get; set; }
        public string ImageApiDirectory { get; set; }
        public string BotName { get; set; }
        public List<string> IgnoredChannels { get; set; }
        public string ChatType { get; set; }
        public List<string> ExclusiveTypes { get; set; }
        public List<string> RequiredProperyMatches { get; set; }
        public List<string> NickNames { get; set; }
        public double TargetedResponseConfidenceThreshold { get; set; }
        public double ReactionConfidenceThreshold { get; set; }
        public int MaximumReactionsPerMessage { get; set; }
        public string DefaultResponse { get; set; }
        public string MongoDbConnectionString { get; set; }

        public List<ChannelConfiguration> Channels { get; set; }
    }
}
