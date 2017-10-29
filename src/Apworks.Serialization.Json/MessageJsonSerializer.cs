using Apworks.Messaging;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace Apworks.Serialization.Json
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Apworks.Messaging.IMessageSerializer" />
    public sealed class MessageJsonSerializer : IMessageSerializer
    {
        private readonly Encoding encoding;
        private readonly IObjectSerializer objectSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageJsonSerializer"/> class.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        public MessageJsonSerializer(Encoding encoding = null)
        {
            this.encoding = encoding ?? Encoding.UTF8;
            this.objectSerializer = new ObjectJsonSerializer(encoding);
        }

        public IMessage Deserialize(byte[] value)
        {
            var jsonString = this.encoding.GetString(value);
            return (IMessage)this.objectSerializer.Deserialize(value, GetMessageClrTypeNameFromMessageJson(jsonString));
        }

        public async Task<IMessage> DeserializeAsync(byte[] value, CancellationToken cancellationToken = default(CancellationToken))
        {
            var jsonString = this.encoding.GetString(value);
            return (IMessage)await this.objectSerializer.DeserializeAsync(value, GetMessageClrTypeNameFromMessageJson(jsonString));
        }

        public byte[] Serialize<TMessage>(TMessage message) where TMessage : IMessage
        {
            return this.objectSerializer.Serialize(message);
        }

        public async Task<byte[]> SerializeAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : IMessage
        {
            return await this.objectSerializer.SerializeAsync(message, cancellationToken);
        }

        private static Type GetMessageClrTypeNameFromMessageJson(string json)
        {
            var jobj = JObject.Parse(json);
            var messageClrTypeName = (jobj?.Property("Metadata")?.Value as JObject)
                ?.Property(Message.MessageClrTypeNameMetadataKey)
                ?.Value
                ?.ToString();

            if (string.IsNullOrEmpty(messageClrTypeName))
            {
                throw new MessageSerializationException($"Cannot deserialize the message. The type of the message cannot be inferred from the message metadata.");
            }

            var messageClrType = Type.GetType(messageClrTypeName);
            if (messageClrType == null)
            {
                throw new MessageSerializationException($"The message cannot be deserialized. The specified type {messageClrTypeName} cannot be found.");
            }

            return messageClrType;
        }
    }
}
