namespace Apworks.Messaging
{
    public sealed class MessageAcknowledgedEventArgs : MessageProcessedEventArgs
    {
        public MessageAcknowledgedEventArgs(object message, IMessageSerializer messageSerializer)
            : this(message, messageSerializer, false)
        { }

        public MessageAcknowledgedEventArgs(object message, IMessageSerializer messageSerializer, bool autoAck) : base(message, messageSerializer)
        {
            this.AutoAck = autoAck;
        }

        public bool AutoAck { get; }
    }
}
