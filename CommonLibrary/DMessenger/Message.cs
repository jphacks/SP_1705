using System;
using System.Xml.Linq;

namespace DMessenger
{
    public sealed class Message
    {
        public Message()
        {
            MessageId = Guid.NewGuid();
        }
        public Message(XElement xe)
        {
            foreach (var item in xe.Attributes())
            {
                switch (item.Name.ToString())
                {
                    case "deleted":
                        IsDeleted = true;
                        break;
                    case "id":
                        MessageId = Guid.Parse(item.Value);
                        break;
                    case "priority":
                        Priority = (MessagePriority)Enum.Parse(typeof(MessagePriority), item.Value);
                        break;
                    case "update":
                        UpdateTime = DateTime.Parse(item.Value);
                        break;
                }
            }

            if (!IsDeleted)
                Value = new XElement(xe.Element("value"));
        }

        public bool IsDeleted { get; private set; }
        public MessageThread Thread { get; internal set; }
        public Guid MessageId { get; }
        public MessagePriority Priority { get; set; }
        public DateTime UpdateTime { get; set; }
        public XElement Value { get; set; }

        public void Delete()
        {
            IsDeleted = true;
            Value = null;
            UpdateTime = DateTime.Now;
        }

        public XElement ToXml()
        {
            var xe = new XElement("message");
            xe.SetAttributeValue("id", MessageId.ToString("N")); // メッセージID
            xe.SetAttributeValue("update", UpdateTime); // 更新日時
            if (Thread != null)
                xe.SetAttributeValue("thread", Thread.ThreadId);

            if (IsDeleted)
            {
                xe.SetAttributeValue("deleted", "true");
            }
            else
            {
                xe.Add(new XElement(Value)); // 本文
                if (Priority != MessagePriority.Normal)
                    xe.SetAttributeValue("priority", Priority.ToString()); // 緊急度
            }

            return xe;
        }
    }

    public enum MessagePriority
    {
        Talk = -1,
        Normal = 0,
        Emergency = 1
    }
}
