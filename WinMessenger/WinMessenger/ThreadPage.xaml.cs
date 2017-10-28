using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace WinMessenger
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class ThreadPage : Page
    {
        private DB.ThreadItem thread;
        private IEnumerable<DMessenger.Message> dbsource;

        public ThreadPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            thread = (DB.ThreadItem)e.Parameter;
            var thid = thread.Id;

            dbsource = DB.LocalDB.db.Table<DB.MessageItem>().Where(item => item.ThreadId == thid).Select(item => item.Value);
            toke.ItemsSource = dbsource;

            var nav = SystemNavigationManager.GetForCurrentView();
            nav.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            nav.BackRequested += Nav_BackRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            var nav = SystemNavigationManager.GetForCurrentView();
            nav.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            nav.BackRequested -= Nav_BackRequested;
        }

        private void Nav_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var message = textBox.Text;
            var msg = new DMessenger.Message()
            {
                Value = new XElement("value", new XText(message)),
                UpdateTime = DateTime.UtcNow
            };
            DMessenger.MessageThread.Get(thread.Id).AddOrUpdate(msg);
            await MessageAPI.SendMessageAsync(new DB.MessageItem(msg, MessageAPI.tempAccount));
            toke.ItemsSource = null;
            toke.ItemsSource = dbsource;
            textBox.Text = "";
        }
    }
}
