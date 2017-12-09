using System;
using System.Threading.Tasks;
using ChronoBot.Common;
using Discord.Commands;
using NLog;

namespace ChronoBot.Modules.ChronoGG
{
    [Group]
    public class ChronoGG : ModuleBase<SocketCommandContext>
    {
        private ChronoGGService ChronoGGService { get; set; }

        private Logger Logger = LogManager.GetCurrentClassLogger();

        public ChronoGG(ChronoGGService chronoGGService)
        {
            ChronoGGService = chronoGGService;
        }

        [ChronoCommand]
        public async Task Hello()
        {
            await ReplyAsync($"Hello {Context.Message.Author.Username}!");
        }

        [ChronoCommand]
        public async Task Sale()
        {
            await ReplyAsync(ChronoGGService.Sale is null ? "The current sale has not yet been retrieved. Please try again later." : string.Empty, false, ChronoGGService.Sale?.ToEmbed());
        }

        [ChronoCommand]
        public async Task AutoSale()
        {
            ulong channelId = Context.Channel.Id;

            if (ChronoGGService.AutoSaleChannels.ContainsKey(channelId))
            {
                ChronoGGService.AutoSaleChannels.TryRemove(channelId, out bool _);
                await ReplyAsync("Channel removed from automatic sale notifications.");
            }
            else
            {
                ChronoGGService.AutoSaleChannels.TryAdd(channelId, true);
                await ReplyAsync("Channel added to automatic sale notifications.");
            }

            await ChronoGGService.WriteChannelIds();
        }

        [ChronoCommand]
        public async Task AutoStatus()
        {
            ulong channelId = Context.Channel.Id;

            if (ChronoGGService.AutoSaleChannels.ContainsKey(channelId))
            {
                await ReplyAsync("Channel is receiving sale notifications.");
            }
            else
            {
                await ReplyAsync("Channel is not receiving sale notifications.");
            }
        }

        [ChronoCommand]
        public async Task Shop(int index = 0)
        {
            if (ChronoGGService.Shop is null)
                await ReplyAsync("The shop has not been loaded. Please try again later.");
            else if (index == 0)
                await ReplyAsync(string.Join(Environment.NewLine + Environment.NewLine, ChronoGGService.Shop.CurrentItems.GetItemList()));
            else
                await ReplyAsync(string.Empty, false, ChronoGGService.Shop.GetShopItemByDisplayIndex(index).ToEmbed());
        }
    }
}
