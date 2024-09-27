using Newtonsoft.Json;
using Resource_Monitor___PC_Client.Properties;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Resource_Monitor___PC_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly ResourceRepository resourceRepository;
        private CancellationTokenSource cts;
        public MainWindow()
        {
            InitializeComponent();

            resourceRepository = new ResourceRepository();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //Get local IP
            lblAddress.Content = GetLocalIP();

            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;

            cts = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    resourceRepository.Poll();
                    var data = resourceRepository.GetDashboardJson();
                    DashboardModel? dashboardModel = JsonConvert.DeserializeObject<DashboardModel>(data);
                    if (dashboardModel == null) continue;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        progressCpu.Value = (int)dashboardModel.CpuUsage;
                        progressGpu.Value = (int)dashboardModel.GpuUsage;
                    }));
                    Thread.Sleep(1000);
                }
            }, TaskCreationOptions.LongRunning);
        }

        static string? GetLocalIP()
        {
            try
            {
                string? localIP;
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint?.Address.ToString();
                }
                return localIP;
            }
            catch (Exception)
            {
                return "127.0.0.1";
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if(this.WindowState == WindowState.Minimized)
                this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HttpServer httpServer = new HttpServer(resourceRepository);
            httpServer.Start();

            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            var ico = Properties.Resources.laptop;
            using (MemoryStream ms = new MemoryStream(ico))
            {
                ni.Icon = new Icon(ms);
            }
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    Show();
                    WindowState = WindowState.Normal;
                };
        }
    }
}