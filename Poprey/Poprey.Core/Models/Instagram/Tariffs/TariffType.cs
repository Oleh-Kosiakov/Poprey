using System.Collections.Generic;

namespace Poprey.Core.Models.Instagram.Tariffs
{
    public class TariffType
    {
        public string Name { get; set; }

        public List<TariffItem> TariffItems { get; set; }
     }
}