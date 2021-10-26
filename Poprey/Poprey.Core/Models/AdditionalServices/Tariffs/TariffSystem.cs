using System.Collections.Generic;

namespace Poprey.Core.Models.AdditionalServices.Tariffs
{
    public class TariffSystem
    {
        public string Name { get; set; }

        public List<TariffService> TariffServices { get; set; }
    }
}