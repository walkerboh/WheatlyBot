using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ChronoBot.Entities.ChronoGG;
using ChronoBot.Services;
using Discord.WebSocket;
using NLog;

namespace ChronoBot.Modules.ChronoGG
{
    public class ChronoGGService
    {
        private DiscordSocketClient Client { get; set; }

        public ConcurrentDictionary<ulong, bool> AutoSaleChannels { get; } = new ConcurrentDictionary<ulong, bool>();

        public Sale Sale { get; set; }

        public ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly int[] apiDelay = new int[] { 0, 60, 180, 300 };

        public ChronoGGService(DiscordSocketClient client)
        {
            Client = client;
        }

        public void StartService()
        {
            Task.Run(SetupSale);
        }

        private async Task GetSale()
        {
            int delayIndex = 0;
            Sale newSale = null;

            while (newSale is null)
            {
                await Task.Delay(apiDelay[delayIndex] * 1000);

                newSale = await new ChronoGGAPI().GetCurrentSaleAsync();
            }

            Sale = newSale;
        }

        private async Task SetupSale()
        {
            await GetSale();

            var diff = Sale.EndDate.ToUniversalTime() - DateTime.UtcNow;

            Task.Delay(Sale.EndDate.ToUniversalTime() - DateTime.UtcNow).ContinueWith((_) => { RunSaleNotifcation(); }).ConfigureAwait(false);

            Logger.Info("Sale retrieved and notification scheduled.");
        }

        private async Task RunSaleNotifcation()
        {
            await GetSale();

            foreach (ulong channelId in AutoSaleChannels.Keys)
            {
                var channel = Client.GetChannel(channelId) as ISocketMessageChannel;

                if (channel is null) return;

                await channel.SendMessageAsync(string.Empty, false, Sale.ToEmbed());
            }

            Task.Delay(Sale.EndDate.ToUniversalTime() - DateTime.UtcNow).ContinueWith((_) => { RunSaleNotifcation(); }).ConfigureAwait(false);

            Logger.Info("Sale retrieved and next notification scheduled.");
        }
    }
}
