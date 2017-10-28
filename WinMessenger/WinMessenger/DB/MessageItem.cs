using DMessenger;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinMessenger.DB
{
    public class MessageItem
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();
        public MessagePriority Priority { get; set; }
        public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
        public string Value { get; set; }
    }
}
