using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WheatlyBot.Entities.ChronoGG;
using WheatlyBot.Services;
using Discord.WebSocket;
using NLog;

namespace WheatlyBot.Modules.ChronoGG
{
    public class ChronoGGService
    {
        public Sale Sale { get; private set; }

        public Shop Shop { get; private set; }

        public ConcurrentDictionary<ulong, bool> AutoSaleChannels { get; } = new ConcurrentDictionary<ulong, bool>();

        private DiscordSocketClient client;

        private LocalStorage localStorage;

        private ChronoGGAPI chronoGGAPI;

        private ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly int[] apiDelay = new int[] { 0, 5, 30, 60, 180, 300 };

        private const string DATA_FILE_NAME = "ChronoGGSaleChannels";

        public ChronoGGService(DiscordSocketClient client, LocalStorage localStorage, ChronoGGAPI chronoGGAPI)
        {
            this.client = client;
            this.localStorage = localStorage;
            this.chronoGGAPI = chronoGGAPI;

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
            Task.Run(UpdateShop);
        }

        private async Task GetSale()
        {
            if (IsOldSale(Sale)) Sale = null;

            int delayIndex = 0;
            Sale newSale = null;

            newSale = await chronoGGAPI.GetCurrentSaleAsync();
            
            while (newSale is null || IsOldSale(newSale))
            {
                logger.Warn($"New sale not retrieved. (Old sale found: ${IsOldSale(newSale)}");
                
                await Task.Delay(apiDelay[delayIndex] * 1000);

                newSale = await chronoGGAPI.GetCurrentSaleAsync();

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

            logger.Info($"Next sale in {diff.Hours} hours {diff.Minutes} minutes. ({diff.TotalSeconds} total seconds)");

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => Task.Delay(Sale.EndDate.ToUniversalTime() - DateTime.UtcNow).ContinueWith((_) => { RunSaleNotifcation(); })).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            logger.Info("Sale retrieved and notification scheduled.");
        }

        private async Task RunSaleNotifcation()
        {
            logger.Info("Running sale notification");

            await GetSale();

            foreach (ulong channelId in AutoSaleChannels.Keys.ToList())
            {
                if (!(client.GetChannel(channelId) is ISocketMessageChannel channel))
                    AutoSaleChannels.Remove(channelId, out bool _);
                else
                    await channel.SendMessageAsync(string.Empty, false, Sale.ToEmbed());
            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => Task.Delay(Sale.EndDate.ToUniversalTime() - DateTime.UtcNow).ContinueWith((_) => { RunSaleNotifcation(); })).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            logger.Info("Sale retrieved and next notification scheduled.");

            await WriteChannelIds();
        }

        private async Task UpdateShop()
        {
            int delayIndex = 0;
            Shop newShop = null;

            while (newShop is null)
            {
                await Task.Delay(apiDelay[delayIndex] * 1000);

                newShop = await chronoGGAPI.GetShopAsync();

                delayIndex = Math.Min(delayIndex + 1, apiDelay.Length);
            }

            Shop = newShop;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => Task.Delay(new TimeSpan(6, 0, 0)).ContinueWith((_) => UpdateShop())).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public async Task WriteChannelIds()
        {
            List<ulong> channelIds = AutoSaleChannels.Keys.ToList();
            await localStorage.WriteData(channelIds, DATA_FILE_NAME);
        }
    }
}
