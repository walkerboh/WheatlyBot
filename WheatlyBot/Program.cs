using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using WheatlyBot.Common.Setup;
using WheatlyBot.Entities;
using WheatlyBot.Extensions;
using WheatlyBot.Modules.ChronoGG;
using WheatlyBot.Services;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NLog;

namespace WheatlyBot
{
    internal class Program
    {
        private static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        private Logger logger;

        private async Task MainAsync()
        {
            LoggerSetup.SetupLog();

            client = new DiscordSocketClient();
            commands = new CommandService();

            await InstallCommandsAsync();

            logger = LogManager.GetCurrentClassLogger();

            client.Log += Log;

            string token = JsonConvert.DeserializeObject<Credentials>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Credentials.json"))).Token;
            await client.LoginAsync(Discord.TokenType.Bot, token);
            await client.StartAsync();

            var chronoGGService = services.GetService<ChronoGGService>();
            await chronoGGService.StartService();

            logger.Debug("Bot running at {0}", DateTime.Now);

            await Task.Delay(-1);
        }

        private async Task InstallCommandsAsync()
        {
            client.MessageReceived += MessageReceived;

            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .AddSingleton<ChronoGGService>()
                .AddTransient<LocalStorage>()
                .AddTransient<ChronoGGAPI>()
                .BuildServiceProvider();

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        private async Task MessageReceived(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage message)) return;

            int argPos = 0;

            if (!message.HasCharPrefix('$', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos) || message.Author.IsBot) return;

            var context = new SocketCommandContext(client, message);

            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                Console.WriteLine(result.ErrorReason);
        }

        private Task Log(Discord.LogMessage logMessage)
        {
            logger.Log(logMessage.Severity.ToNLogLevel(), logMessage.Exception, logMessage.Message);
            return Task.CompletedTask;
        }
    }
}
