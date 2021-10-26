using System;

namespace Poprey.Core.Rest.Models
{
    public class NotConnectedException : Exception
    {
        public NotConnectedException(string message) : base(message)
        {
        }
    }
}