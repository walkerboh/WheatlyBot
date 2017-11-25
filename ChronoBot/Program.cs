using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace ChronoBot
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private CommandService commands;

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();

            await InstallCommandsAsync();

            string token = JsonConvert.DeserializeObject<Credentials>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Credentials.json"))).Token;
            await client.LoginAsync(Discord.TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task InstallCommandsAsync()
        {
            client.MessageReceived += MessageReceived;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task MessageReceived(SocketMessage messageParam)
        {
            SocketUserMessage message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!message.HasCharPrefix('!', ref argPos)) return;

            SocketCommandContext context = new SocketCommandContext(client, message);

            IResult result = await commands.ExecuteAsync(context, argPos);
            if (!result.IsSuccess)
                Console.WriteLine(result.ErrorReason);
        }
    }
}
