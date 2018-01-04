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

        [WheatlyCommand]
        public async Task Hello()
        {
            await ReplyAsync($"Hello {Context.Message.Author.Username}!");
        }

        [WheatlyCommand, Alias("h")]
        public async Task Help()
        {
            await ReplyAsync(@"For help or issues please refer to the GitHub page: https://github.com/walkerboh/WheatlyBot");
        }
    }
}