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

        private readonly int[] apiDelay = new int[] { 0, 5, 30, 60, 180, 300 };

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
            if (IsOldSale(Sale)) Sale = null;

            int delayIndex = 0;
            Sale newSale = null;

            while (newSale is null || IsOldSale(newSale))
            {
                await Task.Delay(apiDelay[delayIndex] * 1000);

                newSale = await new ChronoGGAPI().GetCurrentSaleAsync();

                delayIndex = Math.Min(delayIndex + 1, apiDelay.Length);
            }

            Sale = newSale;

            bool IsOldSale(Sale sale)
            {
                return sale == null || sale.EndDate < DateTime.Now;
            }
        }

        private async Task SetupSale()
        {
            await GetSale();

            var diff = Sale.EndDate.ToUniversalTime() - DateTime.UtcNow;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => Task.Delay(Sale.EndDate.ToUniversalTime() - DateTime.UtcNow).ContinueWith((_) => { RunSaleNotifcation(); })).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => Task.Delay(Sale.EndDate.ToUniversalTime() - DateTime.UtcNow).ContinueWith((_) => { RunSaleNotifcation(); })).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

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
