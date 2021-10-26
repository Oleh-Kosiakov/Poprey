using System.Collections.Generic;

namespace Poprey.Core.Models.Instagram.Tariffs
{
    public class TariffService
    {
        public string Name { get; set; }

        public List<TariffType> TariffTypes { get; set; }

        public  Extras Extras { get; set; }
    }
}