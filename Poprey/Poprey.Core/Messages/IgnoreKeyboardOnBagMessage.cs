using MvvmCross.Plugin.Messenger;

namespace Poprey.Core.Messages
{
    public class IgnoreKeyboardOnBagMessage : MvxMessage
    {
        public bool Ignore { get; }

        public IgnoreKeyboardOnBagMessage(object sender, bool ignore) : base(sender)
        {
            Ignore = ignore;
        }
    }
}