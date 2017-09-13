using Apworks.Messaging;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace Apworks.Serialization.Json
{
    public sealed class MessageJsonSerializer : IMessageSerializer
    {
        private readonly IObjectSerializer internalSerializer;

        public MessageJsonSerializer()
            : this(null, null)
        { }

        public MessageJsonSerializer(JsonSerializerSettings settings, Encoding encoding)
        {
            this.internalSerializer = new ObjectJsonSerializer(settings, encoding);
        }

        public dynamic Deserialize(byte[] value)
            => this.internalSerializer.Deserialize(value);

        public async Task<dynamic> DeserializeAsync(byte[] value, CancellationToken cancellationToken = default(CancellationToken))
            => await this.internalSerializer.DeserializeAsync(value, cancellationToken);

        public byte[] Serialize<TMessage>(TMessage message) 
            where TMessage : IMessage
        {
            return this.internalSerializer.Serialize(message);
        }

        public async Task<byte[]> SerializeAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : IMessage
        {
            return await this.internalSerializer.SerializeAsync(message, cancellationToken);
        }

        private static string GetMessageClrTypeName<TMessage>(byte[] value)
            where TMessage : IMessage
        {
            var json = Encoding.UTF8.GetString(value);
            var jobj = JObject.Parse(json);
            var messageClrType = (jobj?.Property("Metadata")?.Value as JObject)
                ?.Property(Message.MessageClrTypeMetadataKey)
                ?.Value
                ?.ToString();

            if (string.IsNullOrEmpty(messageClrType))
            {
                throw new MessageSerializationException($"Cannot deserialize the message of type {typeof(TMessage)} as the type of the message cannot be inferred from the message metadata.");
            }

            return messageClrType;
        }
    }
}
