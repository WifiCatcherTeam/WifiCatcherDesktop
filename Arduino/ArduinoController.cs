using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Documents;

namespace WifiCatcherDesktop.Arduino
{
    public class ArduinoController : IDisposable
    {
        public static int LowestServoAngle = 40;
        public static int HighestServoAngle = 170;
        public static int InitServoAngle = 100;

        private const int ServoDelay = 200;

        private readonly string _portName;
        public string PortName
        {
            get { return _portName; }
        }

        public bool IsConnected
        {
            get { return _port != null && _port.IsOpen; }
        }

        public int Angle { get; private set; }

        private CatcherState _catcherState;
        public CatcherState CatcherState
        {
            get { return _catcherState; }
        }

        private SerialPort _port;

        public ArduinoController(string portName)
        {
            _portName = portName;
        }

        public void Connect()
        {
            if (IsConnected)
                Disconnect();

            _port = new SerialPort(_portName, 9600, Parity.None, 8, StopBits.One);
            _port.Open();
            MakeAngle(InitServoAngle);
            MakeState(CatcherState.None);
        }

        public void Disconnect()
        {
            if (!IsConnected) return;
            _port.Close();
        }

        public bool MakeAngle(int angle)
        {
            if (!IsConnected)
                throw new Exception("You need Connect() before");

            var corrected = false;
            if (angle < LowestServoAngle)
            {
                angle = LowestServoAngle;
                corrected = true;
            }
            if (angle > HighestServoAngle)
            {
                angle = HighestServoAngle;
                corrected = true;
            }

            Angle = angle;

            byte[] bytes = {0, (byte)Angle};
            _port.Write(bytes, 0, bytes.Length);

            Thread.Sleep(ServoDelay); // ждём чтобы серва успела повернуться

            return !corrected;
        }

        public void MakeState(CatcherState state)
        {
            if (!IsConnected)
                throw new Exception("You need Connect() before");

            _catcherState = state;

            var stateVal = (byte) _catcherState;
            byte[] bytes = { 1, stateVal };
            _port.Write(bytes, 0, bytes.Length);
        }

        public static List<string> GetAvailablePortNames()
        {
            return SerialPort.GetPortNames().ToList();
        }

        public void Dispose()
        {
            Disconnect();
        }

        ~ArduinoController()
        {
            Dispose();
        }
    }
}
