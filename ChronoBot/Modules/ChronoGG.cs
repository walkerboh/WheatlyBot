using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace ChronoBot.Modules
{
    [Group]
    public class ChronoGG : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        public async Task Echo([Remainder] string echo)
        {
            await ReplyAsync(echo);
        }
    }
}
