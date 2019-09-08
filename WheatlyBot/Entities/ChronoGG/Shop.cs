using System.Collections.Generic;
using System.Linq;

namespace WheatlyBot.Entities.ChronoGG
{
    public class Shop
    {
        private readonly IEnumerable<ShopItem> shopItems;

        public Shop(IEnumerable<ShopItem> items)
        {
            shopItems = items;
        }

        public Shop CurrentItems
        {
            get
            {
                return new Shop(shopItems.Where(si => !si.SoldOut).ToList());
            }
        }

        public Shop PastItems
        {
            get
            {
                return new Shop(shopItems.Where(si => si.SoldOut).ToList());
            }
        }

        public IEnumerable<string> GetItemList()
        {
            List<string> list = new List<string>();

            int index = 1;

            foreach (ShopItem item in shopItems)
            {
                list.Add($"{index++}: {item.Name} - {item.Price:n0} Coins ({item.Claimed:0.00%} claimed)");
            }

            return list;
        }

        public ShopItem GetShopItemByDisplayIndex(int index)
        {
            return shopItems.ElementAtOrDefault(index - 1);
        }
    }
}
