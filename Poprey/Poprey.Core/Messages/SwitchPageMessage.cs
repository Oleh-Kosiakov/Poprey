using System;
using MvvmCross.Plugin.Messenger;

namespace Poprey.Core.Messages
{
    public class SwitchPageMessage : MvxMessage
    {
        public Type SwitchToType { get; set; }

        public SwitchPageMessage(object sender, Type switchToType) : base(sender)
        {
            SwitchToType = switchToType;
        }
    }
}
