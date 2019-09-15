using System;
using Discord;
using Newtonsoft.Json;

namespace WheatlyBot.Entities.ChronoGG
{
    public class ShopItem : ChronoGgItem
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        public bool Active { get; set; }
        public float Claimed { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public string Hash { get; set; }
        public string Name { get; set; }
        public string[] Platforms { get; set; }
        public int Price { get; set; }

        [JsonProperty("sold_out")]
        public bool SoldOut { get; set; }
        public string Status { get; set; }
        public string Url { get; set; }

        public Embed ToEmbed()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle($"{Name}")
                .WithUrl(@"https://www.chrono.gg/shop")
                .AddField("Description", $"{Description}")
                .AddField("Cost", $"{Price:n0} Coins", true)
                .AddField("Claimed", $"{Claimed:0.00%}", true)
                .AddField("Platforms", CleanPlatforms(Platforms), true);

            return embed.Build();
        }
    }
}
