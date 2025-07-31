using System;
using System.IO.Ports;

namespace SerialPortDeviceControlApp
{
    public class DeviceController
    {
        private SerialPort _serialPort;

        public DeviceController(string portName)
        {
            _serialPort = new SerialPort(portName);
            _serialPort.DataReceived += OnDataReceived;
        }

        public void Connect()
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }
            }
            catch (Exception ex)
            {
                // Handle connection errors
                Console.WriteLine($"Error connecting to device: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        public void SendCommand(string command)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.WriteLine(command);
            }
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = _serialPort.ReadLine();
            // Process received data (e.g., update graph or device status)
            Console.WriteLine($"Data received: {data}");
        }

        public bool IsConnected()
        {
            return _serialPort.IsOpen;
        }
    }
}