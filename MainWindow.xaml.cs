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

namespace WifiCatcherDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> _portNames;
        private Controller _servoController;

        private static int AngleStep = 10;

        public MainWindow()
        {
            InitializeComponent();
            LoadPortsList();
            MakeDisconnectedViews();
            UpdateAngleViews();

            AngleSlider.Minimum = Controller.LowestServoAngle;
            AngleSlider.Maximum = Controller.HighestServoAngle;
        }

        private void Log(string message)
        {
            LogView.Text += message + '\n';
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
            if (_servoController != null)
                _servoController.Dispose();

            _servoController = new Controller(portName);
            _servoController.Connect();
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
            _servoController.MakeAngle(_servoController.Angle - AngleStep);
            UpdateAngleViews();
        }

        private void PlusAngleButton_Click(object sender, RoutedEventArgs e)
        {
            _servoController.MakeAngle(_servoController.Angle + AngleStep);
            UpdateAngleViews();
        }

        private void AngleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_servoController != null)
            {
                _servoController.MakeAngle((int)Math.Round(AngleSlider.Value));
                UpdateAngleViews();
            }
        }

        private void SliderChangeValue(double value)
        {
            if (_servoController != null)
            {
                _servoController.MakeAngle((int)Math.Round(value));
                UpdateAngleViews();
            }
        }

        private void UpdateAngleViews()
        {
            if (_servoController != null)
            {
                AngleSlider.Value = _servoController.Angle;
                AngleBlock.Text = _servoController.Angle.ToString();
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
    }
}
