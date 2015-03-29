using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WifiCatcherDesktop.Arduino;
using WifiCatcherDesktop.Wifi;

namespace WifiCatcherDesktop
{
    /// <summary>
    /// Interaction logic for ScanWindow.xaml
    /// </summary>
    public partial class ScanWindow : Window
    {
        private static string arduinoPortName = "COM4";

        private ArduinoController _controller;

        private Base _wifiBase;
        private Scanner _wifiScanner;

        public ScanWindow()
        {
            InitializeComponent();

            _wifiBase = new Base();
            ConnectArduino();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ConnectArduino()
        {
            if (_controller != null)
                _controller.Dispose();

            _controller = new ArduinoController(arduinoPortName);
            _controller.Connect();

            Log("Connected to Arduino");

            _wifiScanner = new Scanner(_controller, _wifiBase);
            _wifiScanner.ScanningStarted += ScanningStarted;
            _wifiScanner.ScanningStopped += ScanningStopped;
            _wifiScanner.ScanningMakeAngle += ScanningMakeAngle;
        }

        private void ScanningStarted()
        {
            Dispatcher.Invoke(() =>
            {
                Log("Scanning started");
            });
        }

        private void ScanningStopped()
        {
            Dispatcher.Invoke(() =>
            {
                Log("Scanning stopped");
                Speedmetr.RenderTransform = new RotateTransform(0);
                ShowEntryLines(_wifiBase.Networks);
            });
        }

        private void ScanningMakeAngle(int angle, List<Network> networks)
        {
            Dispatcher.Invoke(() =>
            {
                Log(String.Format("Go on angle : {0}", angle));
                Speedmetr.RenderTransform = new RotateTransform(AngleTransmit(angle));
                ShowEntryLines(networks);
                ShowEntryPoints(networks);
            });
        }

        private void Log(string message)
        {
            LogView.Text = message;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
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

        private void ShowEntryLines(IEnumerable<Network> networks)
        {
            LinesCanvas.Children.Clear();

            foreach (var network in networks)
            {
                foreach (var entry in network.Entries)
                    ShowLine(entry.GetLevel(), network.Ssid);
            }
        }

        private void ShowLine(int level, string name)
        {
            Line line = new Line();
            Thickness thickness = new Thickness(101,-11,362,250);
            line.Margin = thickness;
            line.Visibility = Visibility.Visible;
            line.StrokeThickness = 3;

            line.Stroke = Brushes.Blue;
            
            line.X1 = -LinesCanvas.Width / 2 - 20;
            line.X2 = LinesCanvas.Width / 2 - 20;

            double y = (level)*1.75 + 20;
            line.Y1 = y;
            line.Y2 = y;

            //line.MouseMove += (s, e) => WifiNameBlock.Text = name;
            //line.MouseLeave += (s, e) => WifiNameBlock.Text = "";
            //line.MouseDown += (s, e) => WifiNameBlock.Text = name;

            line.MouseDown += new MouseButtonEventHandler(line_MouseDown);
            line.MouseUp += new MouseButtonEventHandler(line_MouseUp);

            LinesCanvas.Children.Add(line);
        }

        private void ShowEntryPoints(List<Network> networks)
        {
            EntryCircle.Children.Clear();
            //EntryCircle.LayoutTransform = null;
            //EntryCircle.RenderTransform = null;

            foreach (var network in networks)
            {
                foreach (var entry in network.Entries)
                {
                    ShowEntryPoint(entry);
                }

            }
        }

        private void ShowEntryPoint(Entry entry)
        {
            double l = (double) entry.GetBestLevel()/100*(167 - 50) + 50;
            double angle = AngleTransmit2(entry.GetBestAngle());
            double y = l*Math.Cos(angle);
            double x = l*Math.Sin(angle);

            Ellipse ellipse = new Ellipse();
            ellipse.Width = 3;
            ellipse.Height = 3;
            ellipse.Fill = Brushes.Blue;
            Canvas.SetLeft(ellipse, -x + EntryCircle.Width / 2);
            Canvas.SetTop(ellipse, EntryCircle.Height - y);
           // EntryCircle.Children.Add(ellipse);
        }

        private double AngleTransmit(double angle)
        {
            return (double)160*(angle - ArduinoController.LowestServoAngle)/
                   (ArduinoController.HighestServoAngle - ArduinoController.LowestServoAngle + 1) - 80;
        }

        private double AngleTransmit2(double angle)
        {
            return (double)160 * (angle - ArduinoController.LowestServoAngle) /
                   (ArduinoController.HighestServoAngle - ArduinoController.LowestServoAngle + 1) - 80;
        }

        void line_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Change line colour back to normal 
            ((Line)sender).Stroke = Brushes.Black;
        }

        void line_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Change line Colour to something
            ((Line)sender).Stroke = Brushes.Red;
        }
    }
}
