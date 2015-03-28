using System;
using System.IO.Ports;

namespace WifiCatcherDesktop.Arduino
{
    public class ArduinoServoController : IDisposable
    {
        public static int LowestAngle = 40;
        public static int HighestAngle = 170;

        private readonly string _portName;
        public string PortName
        {
            get { return _portName; }
        }

        private SerialPort _port;

        private int _angle = LowestAngle;
        public int Angle { get { return _angle; } }

        public ArduinoServoController(string portName)
        {
            _portName = portName;
        }

        public void Connect()
        {
            _port = new SerialPort(_portName, 9600, Parity.None, 8, StopBits.One);
            _port.Open();
        }

        public bool SetAngle(int angle)
        {
            if (_port == null)
                throw new Exception("You need Connect() before");

            bool corrected = false;
            if (angle < LowestAngle)
            {
                angle = LowestAngle;
                corrected = true;
            }
            if (angle > HighestAngle)
            {
                angle = HighestAngle;
                corrected = true;
            }

            _angle = angle;

            byte[] bytes = {(byte)_angle};
            _port.Write(bytes, 0, bytes.Length);
            return !corrected;
        }

        public void Dispose()
        {
            if (_port != null && _port.IsOpen)
            {
                _port.Close();
            }
        }

        ~ArduinoServoController()
        {
            Dispose();
        }
    }
}
