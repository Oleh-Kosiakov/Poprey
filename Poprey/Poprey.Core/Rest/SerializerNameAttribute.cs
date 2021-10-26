using System;

namespace Poprey.Core.Rest
{
    public class SerializerNameAttribute : Attribute
    {
        public string SerializerName { get; private set; }

        public SerializerNameAttribute(string serializerName)
        {
            SerializerName = serializerName;
        }
    }
}