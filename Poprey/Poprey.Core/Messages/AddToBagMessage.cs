using MvvmCross.Plugin.Messenger;
using Poprey.Core.DisplayModels;
using Poprey.Core.Models.Bag;

namespace Poprey.Core.Messages
{
    public class AddToBagMessage : MvxMessage
    {
        public OrderInfo OrderInfo { get; set; }

        public AddToBagMessage(object sender) : base(sender)
        {
        }
    }
}