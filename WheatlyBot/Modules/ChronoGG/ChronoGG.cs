using System;
using System.Threading.Tasks;
using WheatlyBot.Common;
using Discord.Commands;

namespace WheatlyBot.Modules.ChronoGG
{
    [Group("chrono")]
    public class ChronoGg : ModuleBase<SocketCommandContext>
    {
        private readonly ChronoGgService _chronoGgService;

        public ChronoGg(ChronoGgService chronoGgService)
        {
            _chronoGgService = chronoGgService;
        }

        [WheatlyCommand]
        [Alias("s")]
        public async Task Sale()
        {
            await ReplyAsync(_chronoGgService.Sale is null ? "The current sale has not yet been retrieved. Please try again later." : string.Empty, false, _chronoGgService.Sale?.ToEmbed());
        }

        [WheatlyCommand]
        [Alias("as")]
        [RequireUserPermission(Discord.GuildPermission.ManageGuild | Discord.GuildPermission.ManageChannels)]
        public async Task AutoSale()
        {
            var channelId = Context.Channel.Id;

            if (_chronoGgService.AutoSaleChannels.ContainsKey(channelId))
            {
                _chronoGgService.AutoSaleChannels.TryRemove(channelId, out bool _);
                await ReplyAsync("Channel removed from automatic sale notifications.");
            }
            else
            {
                _chronoGgService.AutoSaleChannels.TryAdd(channelId, true);
                await ReplyAsync("Channel added to automatic sale notifications.");
            }

            await _chronoGgService.WriteChannelIds();
        }

        [WheatlyCommand]
        public async Task AutoStatus()
        {
            var channelId = Context.Channel.Id;

            if (_chronoGgService.AutoSaleChannels.ContainsKey(channelId))
            {
                await ReplyAsync("Channel is receiving sale notifications.");
            }
            else
            {
                await ReplyAsync("Channel is not receiving sale notifications.");
            }
        }

        [WheatlyCommand]
        public async Task Shop(int index = 0)
        {
            if (_chronoGgService.Shop is null)
                await ReplyAsync("The shop has not been loaded. Please try again later.");
            else if (index == 0)
                await ReplyAsync(string.Join(Environment.NewLine + Environment.NewLine, _chronoGgService.Shop.CurrentItems.GetItemList()));
            else
            {
                var item = _chronoGgService.Shop.CurrentItems.GetShopItemByDisplayIndex(index);

                if (item != null)
                    await ReplyAsync(string.Empty, false, _chronoGgService.Shop.CurrentItems.GetShopItemByDisplayIndex(index).ToEmbed());
                else
                    await ReplyAsync("Invalid shop item ID.");
            }
        }
    }
}
