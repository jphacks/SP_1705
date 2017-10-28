using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;

namespace WinMessenger
{
    static class MessageAPI
    {
        public static RSACryptoServiceProvider tempAccount;
        private static ConcurrentDictionary<DB.MessageItem, byte[]> queue = new ConcurrentDictionary<DB.MessageItem, byte[]>();
        private static BluetoothLEAdvertisementPublisher publisher = new BluetoothLEAdvertisementPublisher();

        static MessageAPI()
        {
            tempAccount = new RSACryptoServiceProvider(new CspParameters()
            {
                KeyContainerName = "MessageAccount05"
            })
            {
                PersistKeyInCsp = true
            };

            //var manufacturerData = new BluetoothLEManufacturerData();
            //manufacturerData.CompanyId = 0xFFFE;

            //var writer = new DataWriter();
            //using (var ms = new MemoryStream())
            //{
            //    ms.WriteByte(0);
            //    var pkey = tempAccount.ExportParameters(false).Modulus;
            //    ms.Write(pkey, 0, pkey.Length);
            //}
            //manufacturerData.Data = writer.DetachBuffer();
            //publisher.Advertisement.ManufacturerData.Add(manufacturerData);

            //publisher.Start();
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
