using System;
using System.Collections.Generic;
using System.Linq;
using Discord;

namespace WheatlyBot.Entities.Meh
{
    public class Deal
    {
        public string Features { get; set; }
        public string Id { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<string> Photos { get; set; }
        public PurchaseQuantity PurchaseQuantity { get; set; }
        public string Title { get; set; }
        public string Specifications { get; set; }
        public Story Story { get; set; }
        public Theme Theme { get; set; }
        public string Url { get; set; }
        public IEnumerable<Launch> Launches { get; set; }
        public DealTopic Topic { get; set; }

        internal Embed ToEmbed()
        {
            var item = Items.FirstOrDefault();

            var embed = new EmbedBuilder();
            embed.WithTitle(Title).WithUrl(Url).WithImageUrl(item?.Photo).AddField("Price", $"{item?.Price:c}", true)
                .AddField("Condition", item?.Condition, true)
                .AddField("Max Purchase", $"{PurchaseQuantity.MaximumLimit} maximum", true);

            return embed.Build();
        }
    }

    public class PurchaseQuantity
    {
        public int MaximumLimit { get; set; }
        public int MinimumLimit { get; set; }
    }

    public class Story
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class Theme
    {
        public string AccentColor { get; set; }
        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public string Foreground { get; set; }
    }

    public class DealTopic
    {
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Id { get; set; }
        public int ReplyCount { get; set; }
        public string Url { get; set; }
        public int VoteCount { get; set; }
    }

    public class Item
    {
        public IEnumerable<object> Attributes { get; set; }
        public string Condition { get; set; }
        public string Id { get; set; }
        public int Price { get; set; }
        public string Photo { get; set; }
    }

    public class Launch
    {
        public object SoldOutAt { get; set; }
    }
}