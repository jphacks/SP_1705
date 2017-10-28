using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WinMessenger.DB
{
    static class LocalDB
    {
        public static SQLiteConnection db;

        static LocalDB()
        {
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Personal.sqlite");
            db = new SQLiteConnection(new SQLitePlatformWinRT(), path);
            db.CreateTable<ThreadItem>();
            db.CreateTable<MessageItem>();
        }
    }
}
