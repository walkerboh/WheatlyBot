using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ChronoBot.Common.Setup;
using ChronoBot.Entities;
using ChronoBot.Extensions;
using ChronoBot.Modules.ChronoGG;
using ChronoBot.Services;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NLog;

namespace ChronoBot
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        private Logger logger;

        public async Task MainAsync()
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
            chronoGGService.StartService();

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
                .BuildServiceProvider();

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task MessageReceived(SocketMessage messageParam)
        {
            SocketUserMessage message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!message.HasCharPrefix('!', ref argPos)) return;

            SocketCommandContext context = new SocketCommandContext(client, message);

            IResult result = await commands.ExecuteAsync(context, argPos, services);
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
