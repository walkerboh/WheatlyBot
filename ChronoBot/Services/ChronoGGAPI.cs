using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChronoBot.Common;
using ChronoBot.Entities.ChronoGG;
using Microsoft.Extensions.Logging;

namespace ChronoBot.Services
{
    public class ChronoGGAPI
    {
        public ILogger<ChronoGGAPI> Logger { get; set; }

        private readonly string SaleURI = @"https://api.chrono.gg/sale";

        public async Task<Sale> GetCurrentSaleAsync()
        {
            Sale sale = null;

            try
            {
                sale = await REST.Get<Sale>(SaleURI);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in ChronoGGAPI.GetCurrentSaleAsync");
            }

            return sale;
        }
    }
}
