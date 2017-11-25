using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Discord.Commands;

namespace ChronoBot.Common
{
    public class ChronoCommandAttribute : CommandAttribute
    {
        public ChronoCommandAttribute([CallerMemberName] string memberName = "") : base(memberName)
        {
        }
    }
}
