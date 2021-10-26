using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Poprey.Core.Rest
{
    public class HttpRequestMessageBuilder : HttpRequestMessage
    {
        public HttpRequestMessageBuilder(Uri url, HttpMethod method)
        {
            RequestUri = url;
            Method = method;
        }

        public HttpRequestMessageBuilder WithContent(HttpContent value)
        {
            Content = value;
            return this;
        }

        public HttpRequestMessageBuilder WithObjectContent(object value, string csrfToken)
        {
            if (value == null)
                value = new object();

            Content = new StringContent(AmpersandSerializeObject(value, csrfToken), Encoding.UTF8, "application/x-www-form-urlencoded");
            return this;
        }

        private static string AmpersandSerializeObject(object data, string csrfToken)
        {
            var builder = new StringBuilder();

            builder.Append($"csrf={csrfToken}&");

            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(data))
            {
                if (propertyDescriptor.PropertyType.GetInterfaces().Any(t => t.IsGenericType
                                                                             && t.GetGenericTypeDefinition() == typeof(IList<>)))
                {
                    SerializeList(builder, propertyDescriptor, data);
                }
                else
                {
                    var value = propertyDescriptor.GetValue(data);

                    if (value != null)
                    {
                        builder.Append($"{propertyDescriptor.Name.ToLowerInvariant()}={propertyDescriptor.GetValue(data)}&");
                    }
                }
            }

            var builtString = builder.ToString();

            if (builtString.Length != 0)
            {
                return builtString.TrimEnd('&');
            }

            return string.Empty;
        }

        private static void SerializeList(StringBuilder builder, PropertyDescriptor propertyDescriptor, object data)
        {
            var listTType = propertyDescriptor.PropertyType.GetGenericArguments()[0];
            var list = propertyDescriptor.GetValue(data) as IList;

            foreach (var propertyInfo in listTType.GetProperties())
            {
                string key;

                var serializerNameAttribute = propertyInfo.GetCustomAttributes(true)
                                            .FirstOrDefault(ca => ca.GetType() == typeof(SerializerNameAttribute)) as SerializerNameAttribute;

                if (serializerNameAttribute == null)
                {
                    key = $"{propertyInfo.Name}[]";
                }
                else
                {
                    key = $"{serializerNameAttribute.SerializerName}[]";
                }

                foreach (var obj in list)
                {
                    builder.Append($"{key}={propertyInfo.GetValue(obj)}&");
                }
            }
        }
    }
}