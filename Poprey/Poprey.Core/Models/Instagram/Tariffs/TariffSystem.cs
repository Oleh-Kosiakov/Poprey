using System.Collections.Generic;

namespace Poprey.Core.Models.Instagram.Tariffs
{
    public class TariffSystem
    {
        public string Name { get; set; }

        public List<TariffService> TariffServices { get; set; }
    }
}