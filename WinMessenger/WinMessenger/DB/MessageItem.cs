using DMessenger;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace WinMessenger.DB
{
    public class MessageItem
    {
        private Lazy<Message> message;

        public MessageItem()
        {
            message = new Lazy<Message>(() => new Message(MessageEncoder.Decode(Binary)));
        }
        public MessageItem(Guid msgid, Guid thid, byte[] bin)
        {
            Id = msgid;
            ThreadId = thid;
            Binary = bin;
        }
        public MessageItem(Message value, RSA key)
        {
            SetValue(value, key);
        }

        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ThreadId { get; set; }
        public byte[] Binary { get; set; }
        public MessageStatus Status { get; set; }
        public Brush BackColor => Value.Priority == MessagePriority.Emergency ? new SolidColorBrush(Colors.Orange) : new SolidColorBrush(Colors.Transparent);

        [Ignore]
        public Message Value => message.Value;

        public void SetValue(Message value, RSA key)
        {
            Binary = MessageEncoder.Encode(value.ToXml(), key);
            ThreadId = value.Thread.ThreadId;
            message = new Lazy<Message>(value);
        }

        public override string ToString() => Value.Value.Value;
    }

    public enum MessageStatus
    {
        Sending,
        Sended,
        Error
    }
}
