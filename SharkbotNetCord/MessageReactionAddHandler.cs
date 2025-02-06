using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace SharkbotNetCord
{
    [GatewayEvent(nameof(GatewayClient.MessageReactionAdd))]
    public class MessageReactionAddHandler(RestClient client, ILogger<MessageCreateHandler> logger) : IGatewayEventHandler<MessageReactionAddEventArgs>
    {
        public async ValueTask HandleAsync(MessageReactionAddEventArgs args)
        {
            logger.LogInformation("{}", $"<@{args.UserId}> reacted with {args.Emoji.Name}!");
        }
    }
}
