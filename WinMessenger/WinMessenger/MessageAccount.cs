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
        public IEnumerable<DB.ThreadItem> Threads => from item in db.Table<DB.ThreadItem>() select item.SetDB(this);

        public static MessageAccount Get(Guid id)
        {
            return accounts.GetOrAdd(id, i => new MessageAccount(i));
        }

        public void AddThread(DB.ThreadItem item)
        {
            db.Insert(item);
            item.SetDB(this);
        }
    }
}
