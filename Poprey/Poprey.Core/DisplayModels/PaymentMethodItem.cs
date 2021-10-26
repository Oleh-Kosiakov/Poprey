using Poprey.Core.Models;

namespace Poprey.Core.DisplayModels
{
    public class PaymentMethodItem
    {
        public string Name { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}