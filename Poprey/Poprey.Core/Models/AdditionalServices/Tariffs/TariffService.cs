using System.Collections.Generic;

namespace Poprey.Core.Models.AdditionalServices.Tariffs
{
    public class TariffService
    {
        public string Name { get; set; }

        public List<TariffItem> TariffItems { get; set; }
    }
}