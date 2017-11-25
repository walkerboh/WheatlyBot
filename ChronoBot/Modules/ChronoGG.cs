using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChronoBot.Common;
using ChronoBot.Entities.ChronoGG;
using Discord;
using Discord.Commands;

namespace ChronoBot.Modules
{
    [Group]
    public class ChronoGG : ModuleBase<SocketCommandContext>
    {
        [ChronoCommand]
        public async Task Hello()
        {
            await ReplyAsync($"Hello {Context.Message.Author.Username}!");
        }

        [ChronoCommand]
        public async Task Sale()
        {
            Sale sale = await new Services.ChronoGGAPI().GetCurrentSaleAsync();
            await ReplyAsync(string.Empty, false, sale.ToEmbed());
        }
    }
}
