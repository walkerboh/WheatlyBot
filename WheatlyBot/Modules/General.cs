using Discord.Commands;
using System.Threading.Tasks;
using WheatlyBot.Common;

namespace WheatlyBot.Modules
{
    [Group]
    public class General : ModuleBase<SocketCommandContext>
    {
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