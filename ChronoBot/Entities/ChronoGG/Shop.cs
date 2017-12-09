using System.Collections.Generic;
using System.Linq;

namespace ChronoBot.Entities.ChronoGG
{
    public class Shop
    {
        private IEnumerable<ShopItem> shopItems;

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
                list.Add(string.Format("{0}: {1} - {2:n0} Coins ({3:0.00%} claimed)", index++, item.Name, item.Price, item.Claimed));
            }

            return list;
        }

        public ShopItem GetShopItemByDisplayIndex(int index)
        {
            return shopItems.ElementAt(index - 1);
        }
    }
}
