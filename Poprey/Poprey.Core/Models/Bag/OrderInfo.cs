
using Poprey.Core.Models.Instagram.Tariffs;

namespace Poprey.Core.Models.Bag
{
    public class OrderInfo
    {
        public Order Order { get; set; }
        public bool IsInBagAlternativeMode { get; set; }
        public bool IsInBagCompactMode { get; set; }
        public bool AdditionalOptionsHidden { get; set; }
        public int DiscountPercent { get; set; }
        public string ServicePseudonym { get; set; }
        public string NormalModeBagItemLabelText { get; set; }
        public string AlternativeModeBagItemLabelText { get; set; }
        public ExtraItem OfferedImpressionsExtraItem { get; set; }
    }
}