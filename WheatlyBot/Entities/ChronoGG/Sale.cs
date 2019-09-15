using System;
using Discord;
using Newtonsoft.Json;

namespace WheatlyBot.Entities.ChronoGG
{
    public class Sale : ChronoGgItem
    {
        protected bool Equals(Sale other)
        {
            return StartDate.Equals(other.StartDate) && EndDate.Equals(other.EndDate);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Sale) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StartDate.GetHashCode() * 397) ^ EndDate.GetHashCode();
            }
        }

        public string Name { get; set; }
        public string Url { get; set; }

        [JsonProperty("unique_url")]
        public string UniqueUrl { get; set; }

        [JsonProperty("steam_url")]
        public string SteamUrl { get; set; }

        [JsonProperty("og_image")]
        public string OgImage { get; set; }
        public string[] Platforms { get; set; }

        [JsonProperty("promo_image")]
        public string PromoImage { get; set; }

        [JsonProperty("normal_price")]
        public decimal NormalPrice { get; set; }
        public string Discount { get; set; }

        [JsonProperty("sale_price")]
        public decimal SalePrice { get; set; }

        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }
        public Item[] Items { get; set; }

        [JsonProperty("end_date")]
        public DateTime EndDate { get; set; }
        public string Currency { get; set; }

        public class Item
        {
            public string Type { get; set; }
            public string Id { get; set; }
        }

        public Embed ToEmbed()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle($"{Name}")
                .WithUrl(UniqueUrl)
                .WithImageUrl(PromoImage)
                .AddField("Sale Price", SalePrice, true)
                .AddField("Original Price", NormalPrice, true)
                .AddField("Discount", Discount, true)
                .AddField("Platforms", CleanPlatforms(Platforms), true)
                .WithFooter($"Sale ends {EndDate.ToUniversalTime():M/d/yy H:mm K}");

            return embed.Build();
        }
    }
}
