using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChronoBot.Common;
using ChronoBot.Entities.ChronoGG;

namespace ChronoBot.Services
{
    public class ChronoGGAPI
    {
        private readonly string SaleURI = @"https://api.chrono.gg/sale";

        public async Task<Sale> GetCurrentSaleAsync()
        {
            Sale sale = null;

            try
            {
                sale = await REST.Get<Sale>(@"https://api.chrono.gg/sale");
            }
            catch
            {
                // TODO
            }

            return sale;
        }
    }
}
