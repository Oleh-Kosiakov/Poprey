using MvvmCross.Plugin.Messenger;

namespace Poprey.Core.Messages
{
    public class InstagramShowCustomErrorMessage : MvxMessage
    {
        public string ErrorMessage { get; set; }

        public InstagramShowCustomErrorMessage(object sender, string errorMessage) : base(sender)
        {
            ErrorMessage = errorMessage;
        }
    }
}