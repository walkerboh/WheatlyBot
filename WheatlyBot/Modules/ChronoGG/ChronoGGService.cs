using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using WheatlyBot.Entities.ChronoGG;
using WheatlyBot.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using NLog;
using WheatlyBot.Modules.Interfaces;
using WheatlyBot.Settings;

namespace WheatlyBot.Modules.ChronoGG
{
    public class ChronoGgService : IDisposable, INotificationService
    {
        public Sale Sale { get; private set; }

        public Shop Shop { get; private set; }

        public ConcurrentDictionary<ulong, bool> AutoSaleChannels { get; } = new ConcurrentDictionary<ulong, bool>();

        private readonly DiscordSocketClient _client;

        private readonly LocalStorage _localStorage;

        private readonly ChronoGgApi _chronoGgApi;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private const string DataFileName = "ChronoGGSaleChannels";

        private readonly ChronoGgSettings _settings;

        private Timer _saleTimer;
        private Timer _shopTimer;

        public ChronoGgService(DiscordSocketClient client, LocalStorage localStorage, ChronoGgApi chronoGgApi, IOptions<ChronoGgSettings> settings)
        {
            _client = client;
            _localStorage = localStorage;
            _chronoGgApi = chronoGgApi;
            _settings = settings.Value;

            var channelIds = localStorage.ReadData<List<ulong>>(DataFileName).Result;

            if (channelIds != null && channelIds.Any())
            {
                foreach (ulong channelId in channelIds)
                {
                    AutoSaleChannels.TryAdd(channelId, true);
                }
            }

            _logger.Info("{0} channel ids loaded for ChronoGGService", channelIds?.Count ?? 0);
        }

        public async Task StartService()
        {
            await GetSale();
            await UpdateShop();

            _saleTimer = new Timer
            {
                AutoReset = true,
                Interval = _settings.SaleDelay
            };

            _saleTimer.Elapsed += async (source, args) => await GetSale();
            _saleTimer.Enabled = true;

            _shopTimer = new Timer
            {
                AutoReset = true,
                Interval = _settings.ShopDelay
            };

            _shopTimer.Elapsed += async (source, args) => await UpdateShop();
            _shopTimer.Enabled = true;
        }

        private async Task GetSale()
        {
            var newSale = await _chronoGgApi.GetCurrentSaleAsync();

            if (newSale is null)
            {
                _logger.Warn("No sale retrieved.");
            }
            else if (IsOldSale(newSale))
            {
                _logger.Warn("Ended sale retrieved.");
            }
            else if(!newSale.Equals(Sale))
            {
                var oldSale = Sale;
                Sale = newSale;
                _logger.Debug("Updated sale.");

                if (!(oldSale is null))
                {
                    await RunSaleNotification();
                }
            }

            bool IsOldSale(Sale sale)
            {
                return sale == null || sale.EndDate < DateTime.Now;
            }
        }

        private async Task RunSaleNotification()
        {
            _logger.Info("Running sale notification");

            foreach (ulong channelId in AutoSaleChannels.Keys.ToList())
            {
                if (!(_client.GetChannel(channelId) is ISocketMessageChannel channel))
                    AutoSaleChannels.Remove(channelId, out bool _);
                else
                    await channel.SendMessageAsync(string.Empty, false, Sale.ToEmbed());
            }

            await WriteChannelIds();
        }

        private async Task UpdateShop()
        {
            var newShop = await _chronoGgApi.GetShopAsync();

            if (newShop != null)
            {
                _logger.Debug("Updated shop.");
                Shop = newShop;
            }
        }

        public async Task WriteChannelIds()
        {
            var channelIds = AutoSaleChannels.Keys.ToList();
            await _localStorage.WriteData(channelIds, DataFileName);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _saleTimer?.Dispose();
            _shopTimer?.Dispose();
        }
    }
}
