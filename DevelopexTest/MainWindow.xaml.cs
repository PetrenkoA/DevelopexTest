using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevelopexTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApplicationData AppData;
        Thread BaseThread;
        static object locker = new object();

        public class ApplicationData
        {
            public ObservableCollection<Page> Queue;
            public int actualPosition;
            public int scannedPages;

            public Thread[] Threads;
            public ManualResetEvent stopEvent;
            public ManualResetEvent pauseEvent;

            public int urlCount;
            public string wordToFind;
            public int TotalWordCount;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeData()
        {
            AppData = new ApplicationData {
                Queue = new ObservableCollection<Page>(),
                actualPosition = 0,
                TotalWordCount = 0,
                scannedPages = 0,
                wordToFind = tbx_seatchText.Text,
                urlCount = Int32.Parse(tbx_urlNum.Text),
                Threads = new Thread[Int32.Parse(tbx_threadCount.Text)],
                stopEvent = new ManualResetEvent(false),
                pauseEvent = new ManualResetEvent(true)
            };
            AppData.Queue = new ObservableCollection<Page>();
            AppData.Queue.Add(new Page(0, tbx_initURL.Text));
            lbox_console.ItemsSource = AppData.Queue;
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            if(!WebAccessHelper.CheckConnection())
            {
                MessageBox.Show("Internet connection is not available!", "Application stopped working", MessageBoxButton.OK);
                return;
            }
            if (AppData == null || AppData.stopEvent.WaitOne(0))
            {
                InitializeData();
                AppData.stopEvent.Reset();
                BaseThread = new Thread(() => BaseThreadMethod(AppData));
                BaseThread.Start();
            }
            else AppData.pauseEvent.Set();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            AppData.stopEvent.Set();
        }

        private void btn_pause_Click(object sender, RoutedEventArgs e)
        {
            AppData.pauseEvent.Reset();
        }

        public void BaseThreadMethod(ApplicationData AppData)
        {
            do
            {
                int stopPosition = AppData.Queue.Count;
                int cnt = 0;
                int elemPerThread;
                if (AppData.Threads.Length > (stopPosition - AppData.actualPosition)) elemPerThread = 1;
                else elemPerThread = (int)Math.Round((double)(stopPosition - AppData.actualPosition) / AppData.Threads.Length);

                for (int i = AppData.actualPosition; i < stopPosition; i += elemPerThread)
                {
                    int begPosition = i;
                    int endPosition;
                    if (AppData.Threads.Length > (stopPosition - AppData.actualPosition)) endPosition = begPosition + 1;
                    else if (i + elemPerThread > stopPosition) endPosition = stopPosition;
                    else endPosition = i + elemPerThread;
                    AppData.Threads[cnt] = new Thread(() => ScanningThreadMethod(AppData, begPosition, endPosition));
                    AppData.Threads[cnt++].Start();
                }

                for (int i = 0; i < cnt; i++) AppData.Threads[i].Join();
                AppData.actualPosition = stopPosition;
                // Stop and pause
                if (AppData.stopEvent.WaitOne(0))
                {
                    tbx_initURL.Dispatcher.Invoke(new Action(() =>
                    {
                        lbox_console.ItemsSource = null;
                        lbox_console.Items.Refresh();
                        lock (locker)
                        {
                            AppData.Queue.Clear();
                            AppData.Queue.Add(new Page(0, tbx_initURL.Text));
                        }
                    }));
                    MessageBox.Show("The application was terminated successfuly!", "Application stopped working", MessageBoxButton.OK);
                    return;
                }
                AppData.pauseEvent.WaitOne(Timeout.Infinite);
            } while (AppData.actualPosition < AppData.Queue.Count - 1);
        }

        public void ScanningThreadMethod(ApplicationData AppData, int begPosition, int endPosition)
        {
            for(int i = begPosition; i < endPosition; i++)
            {
                if (AppData.stopEvent.WaitOne(0)) return;
                AppData.pauseEvent.WaitOne(Timeout.Infinite);

                AppData.Queue[i].Status = "Being scanned";
                try
                { AppData.Queue[i].innerHTML = WebAccessHelper.MakeGetRequest(AppData.Queue[i].Url); }
                catch(Exception e)
                {
                    AppData.Queue[i].Status = string.Format("Error: {0}", e.Message);
                    return;
                }
                AppData.TotalWordCount += AppData.Queue[i].getWordCount(AppData.wordToFind);
                List<string> urls = AppData.Queue[i].getPageURL(AppData.urlCount);
                tbx_initURL.Dispatcher.Invoke(new Action(() =>
                {
                    lock (locker)
                    {
                        foreach (string url in urls)
                            if (!AppData.Queue.Any(x => x.Url == url)) AppData.Queue.Add(new Page(AppData.Queue.Count, url));
                        AppData.scannedPages++;
                        double val = ((double)AppData.scannedPages / AppData.Queue.Count) * 100;
                        pbr_progress.Value = ((double)AppData.scannedPages / AppData.Queue.Count) * 100;
                        lbox_console.Items.Refresh();
                    }
                }));
            }
        }

    }
}
