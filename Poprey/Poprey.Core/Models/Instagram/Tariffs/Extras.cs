using System.Collections.Generic;

namespace Poprey.Core.Models.Instagram.Tariffs
{
    public class Extras
    {
        public List<ExtraItem> Impressions { get; set; }

        public List<ExtraItem> Reaches { get; set; }

        public List<ExtraItem> Saves { get; set; }

        public int ImpressionsDisabled { get; set; }

        public int ReachesDisabled { get; set; }

        public int SavesDisabled { get; set; }
    }
}