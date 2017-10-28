using Microsoft.Toolkit.Uwp.Notifications;
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
using Windows.UI.Notifications;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace WinMessenger
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            list.ItemsSource = DB.LocalDB.db.Table<DB.ThreadItem>();

            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
            {
                new AdaptiveText()
                {
                    Text = "スレッド名"
                },
                new AdaptiveText()
                {
                    Text = "メッセージの本文"
                }
            }
                    }
                }
            };
        }
        
        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private async void OnCreateThread(object sender, RoutedEventArgs e)
        {
            var dialog = new CreateThreadDialog();//ダイアログからデータを取得
            if (await dialog.ShowAsync() != ContentDialogResult.Primary)
                return;

            var item = new DB.ThreadItem(dialog.ThreadTitle);//コンストラクタ呼び出し
            DB.LocalDB.db.Insert(item);//DBにインサート

            Frame.Navigate(typeof(ThreadPage), item);
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem is DB.ThreadItem thread)
            {
                Frame.Navigate(typeof(ThreadPage), thread);
            }
        }
    }
}
