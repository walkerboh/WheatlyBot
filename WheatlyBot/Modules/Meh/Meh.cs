using System.Threading.Tasks;
using Discord.Commands;
using WheatlyBot.Common;

namespace WheatlyBot.Modules.Meh
{
    [Group("meh")]
    public class Meh : ModuleBase<SocketCommandContext>
    {
        private readonly MehService _mehService;

        public Meh(MehService service)
        {
            _mehService = service;
        }

        [WheatlyCommand]
        [Alias("d")]
        public async Task Deal()
        {
            await ReplyAsync(
                _mehService.Deal is null
                    ? "The current deal has not yet been retrieved. Please try again later."
                    : string.Empty, false, _mehService.Deal.ToEmbed());
        }

        [WheatlyCommand]
        [Alias("ad", "auto")]
        [RequireUserPermission(Discord.GuildPermission.ManageGuild | Discord.GuildPermission.ManageChannels)]
        public async Task AutoDeal()
        {
            var channelId = Context.Channel.Id;

            if(_mehService.AutoDealChannels.ContainsKey(channelId))
            {
                _mehService.AutoDealChannels.TryRemove(channelId, out bool _);
                await ReplyAsync("Channel removed from automatic deal notifications.");
            }
            else
            {
                _mehService.AutoDealChannels.TryAdd(channelId, true);
                await ReplyAsync("Channel added to automatic deal notifications.");
            }

            await _mehService.WriteChannelIds();
        }

        [WheatlyCommand]
        public async Task AutoStatus()
        {
            var channelId = Context.Channel.Id;

            if(_mehService.AutoDealChannels.ContainsKey(channelId))
            {
                await ReplyAsync("Channel is receiving Meh deal notifications.");
            }
            else
            {
                await ReplyAsync("Channel is not receiving Meh deal notifications.");
            }
        }
    }
}