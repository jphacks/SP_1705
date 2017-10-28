using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WinMessenger
{
    static class MessageAPI
    {
        public static RSACryptoServiceProvider tempAccount;
        private static ConcurrentDictionary<DB.MessageItem, byte[]> queue = new ConcurrentDictionary<DB.MessageItem, byte[]>();

        static MessageAPI()
        {
            tempAccount = new RSACryptoServiceProvider(new CspParameters()
            {
                KeyContainerName = "MessageAccount05"
            })
            {
                PersistKeyInCsp = true
            };
        }

        public static async Task SendMessageAsync(DB.MessageItem item)
        {
            item.Status = DB.MessageStatus.Sending;
            await Task.Run(() =>
            {
                queue[item] = item.Binary;
                DB.LocalDB.db.Insert(item);
            });
        }
    }
}
