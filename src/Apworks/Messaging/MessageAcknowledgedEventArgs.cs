namespace Apworks.Messaging
{
    public sealed class MessageAcknowledgedEventArgs : MessageProcessedEventArgs
    {
        public MessageAcknowledgedEventArgs(object message)
            : this(message, false)
        { }

        public MessageAcknowledgedEventArgs(object message, bool autoAck) : base(message)
        {
            this.AutoAck = autoAck;
        }

        public bool AutoAck { get; }
    }
}
