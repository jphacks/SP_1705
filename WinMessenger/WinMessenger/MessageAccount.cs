using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WinMessenger
{
    class MessageAccount
    {
        private static readonly ConcurrentDictionary<Guid, MessageAccount> accounts
            = new ConcurrentDictionary<Guid, MessageAccount>();
        private static readonly SQLitePlatformWinRT platform = new SQLitePlatformWinRT();
        private static readonly SQLiteConnection users;

        internal readonly SQLiteConnection db;
        internal readonly RSACryptoServiceProvider rsa;

        static MessageAccount()
        {
            users = new SQLiteConnection(platform,
                Path.Combine(ApplicationData.Current.LocalFolder.Path, "common.sqlite"));
            users.CreateTable<DB.AccountInfo>();
        }

        private MessageAccount(Guid id)
        {
            var csp = new CspParameters()
            {
                KeyContainerName = id.ToString("N")
            };
            rsa = new RSACryptoServiceProvider(csp)
            {
                PersistKeyInCsp = true
            };
            PublicKey = rsa.ExportParameters(false).Modulus;

            db = new SQLiteConnection(platform,
                Path.Combine(ApplicationData.Current.LocalFolder.Path, id.ToString("N") + ".sqlite"));
            db.CreateTable<DB.ThreadItem>();
            db.CreateTable<DB.MessageItem>();
        }

        public static IEnumerable<Guid> AccountIds => from info in users.Table<DB.AccountInfo>() select info.Id;

        public byte[] PublicKey { get; }
        public IEnumerable<DB.ThreadItem> Threads => db.Table<DB.ThreadItem>();
        //{
        //    get
        //    {
        //        var ids = new HashSet<Guid>();
        //        foreach (var thid in (from msg in db.Table<DB.MessageItem>() select msg.ThreadId))
        //        {
        //            if (!ids.Add(thid))
        //                continue;

        //            var thread = (from th in db.Table<DB.ThreadItem>() where th.Id == thid select th).FirstOrDefault();
        //            if (thread is null)
        //            {
        //                thread = new DB.ThreadItem()
        //                {
        //                    Id = thid,
        //                    Title = "<不明なスレッド>"
        //                };
        //                db.Insert(thread);
        //            }
        //            yield return thread;
        //        }
        //    }
        //}

        public static MessageAccount Get(Guid id)
        {
            return accounts.GetOrAdd(id, i => new MessageAccount(i));
        }

        public void AddThread(DB.ThreadItem item)
        {
            db.Insert(item);
            db.Commit();
        }

        public void AddMessage(byte[] bin)
        {
            var xml = DMessenger.MessageEncoder.Decode(bin);
            var msg = new DMessenger.Message(xml);

            AddMessage(msg, bin);
        }
        public void AddMessage(DMessenger.Message msg)
        {
            AddMessage(msg, DMessenger.MessageEncoder.Encode(msg.ToXml(), rsa));
        }
        public void AddMessage(DMessenger.Message msg, byte[] bin)
        {
            var msgid = msg.MessageId;
            if (GetMessages().All(it => it.Id != msgid))
            {
                db.Insert(new DB.MessageItem(msgid, msg.Thread.ThreadId, bin));
                if (!db.Table<DB.ThreadItem>().Any(th => th.Id == msg.Thread.ThreadId))
                    db.Insert(new DB.ThreadItem() { Id = msg.Thread.ThreadId, Title = msg.Thread.Title });
                db.Commit();
            }
        }

        public IEnumerable<DB.MessageItem> GetMessages() => db.Table<DB.MessageItem>();
        public IEnumerable<DB.MessageItem> GetMessages(Guid thid) => db.Table<DB.MessageItem>().Where(th => th.ThreadId == thid);
    }
}
