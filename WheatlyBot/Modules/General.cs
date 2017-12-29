using Discord.Commands;
using NLog;
using System.Threading.Tasks;
using WheatlyBot.Common;

namespace WheatlyBot.Modules
{
    [Group]
    public class General : ModuleBase<SocketCommandContext>
    {
        private Logger Logger = LogManager.GetCurrentClassLogger();

        [ChronoCommand]
        public async Task Hello()
        {
            await ReplyAsync($"Hello {Context.Message.Author.Username}!");
        }
    }
}
