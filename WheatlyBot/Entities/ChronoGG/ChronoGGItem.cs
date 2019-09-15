using System.Collections.Generic;

namespace WheatlyBot.Entities.ChronoGG
{
    public abstract class ChronoGgItem
    {
        protected string CleanPlatforms(string[] platforms)
        { 
            var cleanedStrings = new List<string>();

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
                    default:
                        break;
                }
            }

            return string.Join(", ", cleanedStrings);
        }
    }
}
