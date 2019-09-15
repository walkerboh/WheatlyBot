using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using NLog;
using WheatlyBot.Entities.Meh;
using WheatlyBot.Modules.Interfaces;
using WheatlyBot.Services;
using WheatlyBot.Settings;

namespace WheatlyBot.Modules.Meh
{
    public class MehService : IDisposable, INotificationService
    {
        public Deal Deal { get; private set; }
        public Poll Poll { get; private set; }
        public Video Video { get; private set; }

        public ConcurrentDictionary<ulong, bool> AutoDealChannels { get; } = new ConcurrentDictionary<ulong, bool>();

        private readonly DiscordSocketClient _client;
        private readonly LocalStorage _localStorage;
        private readonly MehApi _mehApi;
        private readonly MehSettings _settings;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private const string DataFileName = "MehDealChannels";

        private Timer _dealTimer;

        public MehService(DiscordSocketClient client, LocalStorage localStorage, MehApi api, IOptions<MehSettings> settings)
        {
            _client = client;
            _localStorage = localStorage;
            _mehApi = api;
            _settings = settings.Value;

            var channelIds = localStorage.ReadData<List<ulong>>(DataFileName).Result;

            if(channelIds != null && channelIds.Any())
            {
                foreach(var channelId in channelIds)
                {
                    AutoDealChannels.TryAdd(channelId, true);
                }
            }

            _logger.Info($"{channelIds?.Count ?? 0} channel ids loaded for MehService");
        }

        public async Task StartService()
        {
            await GetDeal();

            _dealTimer = new Timer
            {
                AutoReset =  true,
                Interval =  _settings.DataDelay
            };

            _dealTimer.Elapsed += async (source, args) => await GetDeal();
            _dealTimer.Enabled = true;
        }

        private async Task GetDeal()
        {
            var apiResponse = await _mehApi.GetMehDataAsync();

            if(apiResponse is null)
            {
                _logger.Warn("No Meh data retrieved.");
                return;
            }

            if(Deal is null)
            {
                Deal = apiResponse.Deal;
                _logger.Debug("Updated Meh deal.");
            }
            else if(!apiResponse.Deal.Id.Equals(Deal.Id))
            {
                Deal = apiResponse.Deal;
                _logger.Debug("Updated Meh deal.");

                await RunDealNotification();
            }
        }

        private async Task RunDealNotification()
        {
            _logger.Info("Running Meh deal notification");

            foreach(var channelId in AutoDealChannels.Keys.ToList())
            {
                if(!(_client.GetChannel(channelId) is ISocketMessageChannel channel))
                {
                    AutoDealChannels.Remove(channelId, out _);
                }
                else
                {
                    await channel.SendMessageAsync(string.Empty, false, Deal.ToEmbed());
                }
            }

            await WriteChannelIds();
        }

        public async Task WriteChannelIds()
        {
            var channelIds = AutoDealChannels.Keys.ToList();
            await _localStorage.WriteData(channelIds, DataFileName);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _dealTimer?.Dispose();
        }
    }
}