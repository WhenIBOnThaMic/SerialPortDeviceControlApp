using System;
using System.IO.Ports;
using System.Linq;

public class SerialPortManager
{
    private SerialPort _serialPort;

    public string[] GetAvailablePorts()
    {
        return SerialPort.GetPortNames();
    }

    public void ChangePort(string portName)
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            _serialPort.Close();
        }

        _serialPort = new SerialPort(portName);
    }

    public bool Connect()
    {
        try
        {
            if (_serialPort != null)
            {
                _serialPort.Open();
                _serialPort.DataReceived += DataReceivedHandler;
                return true;
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log them)
        }
        return false;
    }

    public void Disconnect()
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            _serialPort.DataReceived -= DataReceivedHandler;
            _serialPort.Close();
        }
    }

    private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            string data = _serialPort.ReadLine();
            // Process the received data (e.g., notify the UI or update graphs)
        }
    }
}