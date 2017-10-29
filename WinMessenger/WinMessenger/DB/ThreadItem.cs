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
        public ThreadItem() { Id = Guid.NewGuid(); }
        public ThreadItem(string title)
        {
            Id = Guid.NewGuid();
            Title = title;
        }

        [PrimaryKey]
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
