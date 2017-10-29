using SQLite.Net;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinMessenger.DB
{
    public class ThreadItem
    {
        internal MessageAccount account;

        public ThreadItem() { }
        public ThreadItem(string title)
        {
            Id = Guid.NewGuid();
            Title = title;
        }

        [PrimaryKey]
        public Guid Id { get; set; }
        public string Title { get; set; }

        [Ignore]
        public IEnumerable<MessageItem> Messages => from item in account.db.Table<MessageItem>() where item.ThreadId == Id select item;

        public void AddMessage(MessageItem item)
        {
            account.db.Insert(item);
            account.db.Commit();
        }

        internal ThreadItem SetDB(MessageAccount account)
        {
            this.account = account;
            return this;
        }
    }
}
