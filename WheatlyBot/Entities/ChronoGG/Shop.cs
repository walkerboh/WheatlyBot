using System.Collections.Generic;
using System.Linq;

namespace WheatlyBot.Entities.ChronoGG
{
    public class Shop
    {
        private readonly IEnumerable<ShopItem> _shopItems;

        public Shop(IEnumerable<ShopItem> items)
        {
            _shopItems = items;
        }

        public Shop CurrentItems
        {
            get
            {
                return new Shop(_shopItems.Where(si => !si.SoldOut).ToList());
            }
        }

        public Shop PastItems
        {
            get
            {
                return new Shop(_shopItems.Where(si => si.SoldOut).ToList());
            }
        }

        public IEnumerable<string> GetItemList()
        {
            var index = 1;

            return _shopItems.Select(item => $"{index++}: {item.Name} - {item.Price:n0} Coins ({item.Claimed:0.00%} claimed)").ToList();
        }

        public ShopItem GetShopItemByDisplayIndex(int index)
        {
            return _shopItems.ElementAtOrDefault(index - 1);
        }
    }
}
