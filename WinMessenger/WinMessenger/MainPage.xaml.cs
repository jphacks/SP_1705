using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace WinMessenger
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SQLiteConnection db;

        public MainPage()
        {
            this.InitializeComponent();

            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Personal.sqlite");
            db = new SQLiteConnection(new SQLitePlatformWinRT(), path);
            db.CreateTable<DB.ThreadItem>();
            list.ItemsSource = db.Table<DB.ThreadItem>();
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void OnCreateThread(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ThreadPage));
        }
    }
}
