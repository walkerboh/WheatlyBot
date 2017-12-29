using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Newtonsoft.Json;

namespace WheatlyBot.Entities.ChronoGG
{
    public class ShopItem : ChronoGGItem
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
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"{Name}")
                .WithUrl(@"https://www.chrono.gg/shop")
                .AddField("Description", $"{Description}")
                .AddInlineField("Cost", $"{Price:n0} Coins")
                .AddInlineField("Claimed", string.Format("{0:0.00%}", Claimed))
                .AddInlineField("Platforms", CleanPlatforms(Platforms));

            return embed.Build();
        }
    }
}
