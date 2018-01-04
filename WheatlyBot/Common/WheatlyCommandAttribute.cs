using Discord.Commands;
using System.Runtime.CompilerServices;

namespace WheatlyBot.Common
{
    public class WheatlyCommandAttribute : CommandAttribute
    {
        public WheatlyCommandAttribute([CallerMemberName] string memberName = "") : base(memberName)
        {
        }
    }
}
