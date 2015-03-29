using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WifiCatcherDesktop.Arduino;
using WifiCatcherDesktop.Wifi;

namespace WifiCatcherDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> _portNames;
        private ArduinoController _controller;

        private static int AngleStep = 10;

        private Base _wifiBase;
        private Scanner _wifiScanner;

        public MainWindow()
        {
            InitializeComponent();
            LoadPortsList();
            MakeDisconnectedViews();
            UpdateAngleViews();

            AngleSlider.Minimum = ArduinoController.LowestServoAngle;
            AngleSlider.Maximum = ArduinoController.HighestServoAngle;

            _wifiBase = new Base();
            _wifiBase.NetworkUpdated += network => Dispatcher.Invoke(() => LogNetwork(network));
        }

        private void Log(string message)
        {
            LogView.Text += message + '\n';
            LogView.ScrollToEnd();
        }

        private void LogNetwork(Network network)
        {
            Log(String.Format("{0}", network.Ssid));
        }

        private void LoadPortsList()
        {
            _portNames = SerialPort.GetPortNames().ToList();
            PortView.ItemsSource = _portNames;
            if (_portNames.Count > 0)
                PortView.SelectedIndex = 0;
        }

        private void PortsUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPortsList();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            int portIndex = PortView.SelectedIndex;
            if (portIndex != -1)
            {
                ConnectArduino(_portNames[portIndex]);
                MakeConnectedViews();
                UpdateAngleViews();
            }
        }

        private void ConnectArduino(string portName)
        {
            if (_controller != null)
                _controller.Dispose();

            _controller = new ArduinoController(portName);
            _controller.Connect();

            _wifiScanner = new Scanner(_controller, _wifiBase);
            _wifiScanner.ScanningStarted += ScanningStarted;
            _wifiScanner.ScanningStopped += ScanningStopped;
            _wifiScanner.ScanningMakeAngle += ScanningMakeAngle;
        }

        private void MakeConnectedViews()
        {
            AnglePanel.IsEnabled = true;
        }

        private void MakeDisconnectedViews()
        {
            AnglePanel.IsEnabled = false;
        }

        private void MinusAngleButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.MakeAngle(_controller.Angle - AngleStep);
            UpdateAngleViews();
        }

        private void PlusAngleButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.MakeAngle(_controller.Angle + AngleStep);
            UpdateAngleViews();
        }

        private void AngleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_controller != null)
            {
                _controller.MakeAngle((int)Math.Round(AngleSlider.Value));
                UpdateAngleViews();
            }
        }

        private void SliderChangeValue(double value)
        {
            if (_controller != null)
            {
                _controller.MakeAngle((int)Math.Round(value));
                UpdateAngleViews();
            }
        }

        private void UpdateAngleViews()
        {
            if (_controller != null)
            {
                AngleSlider.Value = _controller.Angle;
                AngleBlock.Text = _controller.Angle.ToString();
            }
            else
            {
                AngleBlock.Text = "";
            }
        }

        private bool dragStarted = false;

        private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            SliderChangeValue(((Slider)sender).Value);
            this.dragStarted = false;
        }

        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.dragStarted = true;
        }

        private void Slider_ValueChanged(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
                SliderChangeValue(e.NewValue);

        }

        private void ScanSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (_wifiScanner.IsScanning)
            {
                _wifiScanner.StopScanning();
            }
            else
            {
                _wifiScanner.StartScanning();
            }
        }

        private void ScanningStarted()
        {
            Dispatcher.Invoke(() =>
            {
                Log("Scanning...");
                ScanSwitchButton.Content = "Stop scan";
            });
        }

        private void ScanningStopped()
        {
            Dispatcher.Invoke(() =>
            {
                Log("Stop scanning");
                ScanSwitchButton.Content = "Scan";
                LogWifi(_wifiBase.Networks);
            });
        }

        private void LogWifi(List<Network> networks)
        {
            foreach (var network in networks)
            {
                Log(String.Format("{0} | {1}", network.Ssid, network.IsFree ? "Open" : "Pass"));
                foreach (var pair in network.Values)
                {
                    Log(String.Format("{0:3} : {1}", pair.Key, pair.Value));
                }
            }
        }


        private void ScanningMakeAngle(int angle)
        {
            Dispatcher.Invoke(() =>
            {
                Log(String.Format("--- Make angle = {0} ---", angle));
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogView.Clear();
        }
    }
}
