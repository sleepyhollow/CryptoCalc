using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private double _ethereum;

        public MainWindow()
        {
            InitializeComponent();
            TotalUsd.Content = "$";
            EthereumBox.Text = UserPrefs.Default.CryptoAmount.ToString(CultureInfo.CurrentCulture);

            _timer.Tick += Calculate;
            _timer.Interval = new TimeSpan(0,0,2);
            _timer.Start();
        }

        private void Calculate(object sender, EventArgs e)
        {
            var price = GetPrices("ETH");

            if (TotalUsd != null)
            {
                TotalUsd.Content = "$" + _ethereum * double.Parse(price);
            }

            CurrentPrice.Content = "ETH price: " + price;

            UserPrefs.Default.CryptoAmount = _ethereum;
            UserPrefs.Default.Save();
        }

        private void EtheriumBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EthereumBox.Text != "")
                _ethereum = double.Parse(EthereumBox.Text);
        }

        private static string GetPrices(string cryptoName)
        {
            var url = new WebClient().DownloadString("https://api.coinbase.com/v2/prices/" + cryptoName + "-USD/sell");
            var x = JsonConvert.DeserializeObject<PriceData>(url);
            return x.data.amount;
        }

        public class PriceData
        {
            public Data data { get; set; }
            public Warning[] warnings { get; set; }
        }

        public class Data
        {
            public string _base { get; set; }
            public string currency { get; set; }
            public string amount { get; set; }
        }

        public class Warning
        {
            public string id { get; set; }
            public string message { get; set; }
            public string url { get; set; }
        }

        private void Min_Button(object sender, MouseButtonEventArgs e)
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void Exit_Button(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Window_Move(object sender, MouseButtonEventArgs e)
        {
            if (Application.Current.MainWindow != null)
                DragMove();
        }
    }
}