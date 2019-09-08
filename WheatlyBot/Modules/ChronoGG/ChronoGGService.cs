using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
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

        private readonly int[] apiDelay = { 0, 5, 30, 60, 180, 300 };

        private const string DATA_FILE_NAME = "ChronoGGSaleChannels";

        private Timer _saleTimer;
        private Timer _shopTimer;

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

        public async Task StartService()
        {
            await GetSale();
            await UpdateShop();

            //Task.Run(SetupSale);
            _saleTimer = new Timer
            {
                AutoReset = true,
                Interval = 1000 * 60 * 5, // 5 minutes in ms
            };

            _saleTimer.Elapsed += async (source, args) => await GetSale();
            _saleTimer.Enabled = true;

            //Task.Run(UpdateShop);
            _shopTimer = new Timer
            {
                AutoReset = true,
                Interval = 1000 * 60 * 30, // 30 minutes in ms
            };

            _shopTimer.Elapsed += async (source, args) => await UpdateShop();
            _shopTimer.Enabled = true;
        }

        private async Task GetSale()
        {
            if (IsOldSale(Sale)) Sale = null;

            Sale newSale = await chronoGGAPI.GetCurrentSaleAsync();

            if (newSale is null)
            {
                logger.Warn("No sale retrieved.");
            }
            else if (IsOldSale(newSale))
            {
                logger.Warn("Ended sale retrieved.");
            }
            else
            {
                Sale = newSale;
                logger.Debug("Updated sale.");
            }

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
            Task.Run(async () =>
            {
                await Task.Delay(Sale.EndDate.ToUniversalTime() - DateTime.UtcNow);
                RunSaleNotification();
            }).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            logger.Info("Sale retrieved and notification scheduled.");
        }

        private async Task RunSaleNotification()
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

            await WriteChannelIds();
        }

        private async Task UpdateShop()
        {
            var newShop = await chronoGGAPI.GetShopAsync();

            if (newShop != null)
            {
                logger.Debug("Updated shop.");
                Shop = newShop;
            }
        }

        public async Task WriteChannelIds()
        {
            List<ulong> channelIds = AutoSaleChannels.Keys.ToList();
            await localStorage.WriteData(channelIds, DATA_FILE_NAME);
        }
    }
}
