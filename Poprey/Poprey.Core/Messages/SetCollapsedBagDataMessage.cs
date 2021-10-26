using MvvmCross.Plugin.Messenger;

namespace Poprey.Core.Messages
{
    public class SetCollapsedBagDataMessage : MvxMessage
    {
        public bool? IsBagVisible { get; set; }

        public double? BagSum { get; set; }

        public double? AddToSum { get; set; }

        public double? SubstractFromSum { get; set; }

        public string TempServiceName { get; set; }

        public SetCollapsedBagDataMessage(object sender) : base(sender)
        {
        }
    }
}