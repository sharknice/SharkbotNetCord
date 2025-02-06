using SharkbotNetCord.SharkbotApi.Models;

namespace SharkbotNetCord.SharkbotApi.Services
{
    public class DirectedReplyService
    {
        public double DirectedReply(ConversationData conversationData, NetCord.Gateway.Message message)
        {
            var lastMessage = conversationData.responses.LastOrDefault();
            if (message.MentionedUsers.Any(m => m.Username == lastMessage.chat.botName))
            {
                return 2;
            }

            if (conversationData.responses.Count() > 1 && lastMessage != null && lastMessage.chat.user != lastMessage.chat.botName)
            {
                var secondToLastMessage = conversationData.responses[^2];
                if (secondToLastMessage != null && secondToLastMessage.botName == secondToLastMessage.chat.user)
                {
                    var confidence = secondToLastMessage.naturalLanguageData.responseConfidence;
                    if (lastMessage.naturalLanguageData.sentences.Any(s => s.SentenceType == SentenceType.Interrogative))
                    {
                        return confidence * 2;
                    }
                    return confidence;
                }
            }

            if (lastMessage != null && !conversationData.groupChat && lastMessage.naturalLanguageData.sentences.Any(s => s.SentenceType == SentenceType.Interrogative))
            {
                return 2;
            }

            if (conversationData.responses.Count() == 1 && lastMessage.chat.user != lastMessage.chat.botName)
            {
                if (conversationData.groupChat)
                {
                    return 2;
                }
                return 1;
            }

            return 0;
        }
    }
}
