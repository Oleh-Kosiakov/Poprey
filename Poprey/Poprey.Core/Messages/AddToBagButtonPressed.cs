using MvvmCross.Plugin.Messenger;

namespace Poprey.Core.Messages
{
    public class AddToBagButtonPressedMessage : MvxMessage
    {
        public AddToBagButtonPressedMessage(object sender) : base(sender)
        {
        }
    }
}