using Apworks.Messaging;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Apworks.Serialization.Json
{
    public sealed class MessageJsonSerializer : IMessageSerializer
    {
        private readonly IObjectSerializer internalSerializer = new ObjectJsonSerializer();

        public TMessage Deserialize<TMessage>(byte[] value) 
            where TMessage : IMessage
        {
            var messageClrType = GetMessageClrTypeName<TMessage>(value);
            var messageType = Type.GetType(messageClrType);
            return (TMessage)this.internalSerializer.Deserialize(messageType, value);
        }

        public async Task<TMessage> DeserializeAsync<TMessage>(byte[] value, CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : IMessage
        {
            var messageClrType = GetMessageClrTypeName<TMessage>(value);
            var messageType = Type.GetType(messageClrType);
            return (TMessage)(await this.internalSerializer.DeserializeAsync(messageType, value, cancellationToken));
        }

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
