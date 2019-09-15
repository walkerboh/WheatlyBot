using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using WheatlyBot.Common.Setup;
using WheatlyBot.Extensions;
using WheatlyBot.Modules.ChronoGG;
using WheatlyBot.Services;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using WheatlyBot.Modules.Meh;
using WheatlyBot.Settings;

namespace WheatlyBot
{
    internal class Program
    {
        private static void Main() => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        private Logger _logger;

        private async Task MainAsync()
        {
            LoggerSetup.SetupLog();

            _client = new DiscordSocketClient();
            _commands = new CommandService();

            var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);
            var configuration = configBuilder.Build();

            await InstallCommandsAsync(configuration);

            _logger = LogManager.GetCurrentClassLogger();

            _client.Log += Log;

            DiscordSettings discordSettings = new DiscordSettings();
            configuration.Bind("Discord", discordSettings);

            await _client.LoginAsync(Discord.TokenType.Bot, discordSettings.Token);
            await _client.StartAsync();

            var chronoGgService = _services.GetService<ChronoGgService>();
            await chronoGgService.StartService();

            var mehService = _services.GetRequiredService<MehService>();
            await mehService.StartService();

            _logger.Debug("Bot running at {0}", DateTime.Now);

            await Task.Delay(-1);
        }

        private async Task InstallCommandsAsync(IConfigurationRoot configuration)
        {
            _client.MessageReceived += MessageReceived;

            _services = new ServiceCollection()
                .Configure<ChronoGgSettings>(configuration.GetSection("ChronoGg"))
                .Configure<MehSettings>(configuration.GetSection("Meh"))
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton<ChronoGgService>()
                .AddTransient<LocalStorage>()
                .AddTransient<ChronoGGAPI>()
                .AddTransient<MehApi>()
                .AddSingleton<MehService>()
                .BuildServiceProvider();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task MessageReceived(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage message)) return;

            int argPos = 0;

            if (!message.HasCharPrefix('$', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.Author.IsBot) return;

            var context = new SocketCommandContext(_client, message);

            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
                Console.WriteLine(result.ErrorReason);
        }

        private Task Log(Discord.LogMessage logMessage)
        {
            _logger.Log(logMessage.Severity.ToNLogLevel(), logMessage.Exception, logMessage.Message);
            return Task.CompletedTask;
        }
    }
}
