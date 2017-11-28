using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChronoBot.Entities.ChronoGG;
using ChronoBot.Services;
using Discord.WebSocket;
using NLog;

namespace ChronoBot.Modules.ChronoGG
{
    public class ChronoGGService
    {
        public Sale Sale { get; set; }

        private DiscordSocketClient client;

        private LocalStorage localStorage;

        public ConcurrentDictionary<ulong, bool> AutoSaleChannels { get; } = new ConcurrentDictionary<ulong, bool>();

        private ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly int[] apiDelay = new int[] { 0, 60, 180, 300 };

        private const string DATA_FILE_NAME = "ChronoGGSaleChannels";

        public ChronoGGService(DiscordSocketClient client, LocalStorage localStorage)
        {
            this.client = client;
            this.localStorage = localStorage;

            List<ulong> channelIds = localStorage.ReadData<List<ulong>>(DATA_FILE_NAME).Result;

            if (channelIds != null && channelIds.Any())
            {
                foreach (ulong channelId in channelIds)
                {
                    AutoSaleChannels.TryAdd(channelId, true);
                }
            }

            logger.Info("{0} channel ids loaded for ChronoGGService", channelIds?.Count ?? 0);
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

            logger.Info("Sale retrieved and notification scheduled.");
        }

        private async Task RunSaleNotifcation()
        {
            await GetSale();

            foreach (ulong channelId in AutoSaleChannels.Keys.ToList())
            {
                var channel = client.GetChannel(channelId) as ISocketMessageChannel;

                if (channel is null)
                    AutoSaleChannels.Remove(channelId, out bool _);

                await channel.SendMessageAsync(string.Empty, false, Sale.ToEmbed());
            }

            Task.Delay(Sale.EndDate.ToUniversalTime() - DateTime.UtcNow).ContinueWith((_) => { RunSaleNotifcation(); }).ConfigureAwait(false);

            logger.Info("Sale retrieved and next notification scheduled.");

            await WriteChannelIds();
        }

        public async Task WriteChannelIds()
        {
            List<ulong> channelIds = AutoSaleChannels.Keys.ToList();
            await localStorage.WriteData(channelIds, DATA_FILE_NAME);
        }
    }
}
