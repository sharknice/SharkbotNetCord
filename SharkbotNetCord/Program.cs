using Microsoft.Extensions.Hosting;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using Microsoft.Extensions.DependencyInjection;
using SharkbotNetCord.Configuration.Models;
using SharkbotNetCord.Ollama.Services;
using SharkbotNetCord.SharkbotApi.Services;
using SharkbotNetCord.ImageGeneration;

Console.WriteLine("starting SharkbotNetCord");

var builder = Host.CreateApplicationBuilder(args);

var httpClient = new HttpClient();
httpClient.Timeout = TimeSpan.FromSeconds(600);

builder.Services.AddDiscordGateway(options =>
{
    options.Intents = GatewayIntents.GuildMessages
                  | GatewayIntents.DirectMessages
                  | GatewayIntents.MessageContent
                  | GatewayIntents.DirectMessageReactions
                  | GatewayIntents.GuildMessageReactions
                  | GatewayIntents.Guilds;
}).
AddGatewayEventHandlers(typeof(Program).Assembly).
AddSingleton(httpClient).
AddSingleton<BotConfiguration>().
AddSingleton<ApiUtilityService>().
AddSingleton<ChatUpdateService>().
AddSingleton<ChatResponseService>().
AddSingleton<DirectedReplyService>().
AddSingleton<ImageResponseUtility>().
AddSingleton<ImageGenerationService>().
AddSingleton<GenerateImageResponseService>().
AddSingleton<OllamaResponseService>();


var host = builder.Build().UseGatewayEventHandlers();

await host.RunAsync();