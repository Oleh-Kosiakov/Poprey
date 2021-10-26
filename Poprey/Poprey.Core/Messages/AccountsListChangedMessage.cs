using MvvmCross.Plugin.Messenger;

namespace Poprey.Core.Messages
{
    public class AccountsListChangedMessage : MvxMessage
    {
        public AccountsListChangedMessage(object sender) : base(sender)
        {
        }
    }
}