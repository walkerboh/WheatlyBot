using System;
using System.Collections.Generic;
using System.Text;

namespace WheatlyBot.Entities.ChronoGG
{
    public abstract class ChronoGGItem
    {
        protected string CleanPlatforms(string[] platforms)
        {
            List<string> cleanedStrings = new List<string>();

            foreach (string platform in platforms)
            {
                switch (platform)
                {
                    case "windows":
                        cleanedStrings.Add("Windows");
                        break;
                    case "macos":
                        cleanedStrings.Add("MacOS");
                        break;
                    case "linux":
                        cleanedStrings.Add("Linux");
                        break;
                }
            }

            return String.Join(", ", cleanedStrings);
        }
    }
}
