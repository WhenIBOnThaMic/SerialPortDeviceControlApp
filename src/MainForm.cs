using System;
using System.Collections.Generic;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SerialPortDeviceControlApp
{

    public class MainForm : Form
    {
        private ComboBox comboBoxPorts;
        private ComboBox comboBoxBaudrate;
        private Button buttonScanPorts;
        private Button buttonConnect;
        private Button buttonDisconnect;
        private Chart chartReal;
        private Chart chartImag;
        private Label labelCellCount;
        private Button buttonPumpOn;
        private Button buttonPumpOff;
        private Button buttonPumpInc;
        private Button buttonPumpDec;
        private Button buttonMagnetOn;
        private Button buttonMagnetOff;
        private SerialPort serialPort;
        private int cellCount = 0;
        private List<double> timeData = new List<double>();
        private List<double> realData = new List<double>();
        private List<double> imagData = new List<double>();
        private double time = 0;

        public MainForm()
        {
            this.Text = "Serial Device Control & Graph";
            this.Width = 1200;
            this.Height = 700;

            comboBoxPorts = new ComboBox { Left = 20, Top = 20, Width = 150 };
            comboBoxBaudrate = new ComboBox { Left = 180, Top = 20, Width = 100 };
            comboBoxBaudrate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            comboBoxBaudrate.SelectedIndex = 0;
            buttonScanPorts = new Button { Left = 290, Top = 20, Width = 80, Text = "Quét cổng" };
            buttonConnect = new Button { Left = 380, Top = 20, Width = 80, Text = "Kết nối" };
            buttonDisconnect = new Button { Left = 470, Top = 20, Width = 80, Text = "Ngắt" };
            labelCellCount = new Label { Left = 20, Top = 60, Width = 300, Text = "Số lượng tế bào: 0", Font = new Font("Arial", 12) };
            chartReal = new Chart { Left = 20, Top = 100, Width = 550, Height = 400, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            chartImag = new Chart { Left = 600, Top = 100, Width = 550, Height = 400, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            InitChart(chartReal, "Trở kháng thực");
            InitChart(chartImag, "Trở kháng ảo");
            buttonPumpOn = new Button { Left = 20, Top = 520, Width = 100, Text = "Bật bơm" };
            buttonPumpOff = new Button { Left = 130, Top = 520, Width = 100, Text = "Tắt bơm" };
            buttonPumpInc = new Button { Left = 240, Top = 520, Width = 100, Text = "Tăng tốc" };
            buttonPumpDec = new Button { Left = 350, Top = 520, Width = 100, Text = "Giảm tốc" };
            buttonMagnetOn = new Button { Left = 600, Top = 520, Width = 120, Text = "Bật nam châm" };
            buttonMagnetOff = new Button { Left = 730, Top = 520, Width = 120, Text = "Tắt nam châm" };
            this.Controls.Add(comboBoxPorts);
            this.Controls.Add(comboBoxBaudrate);
            this.Controls.Add(buttonScanPorts);
            this.Controls.Add(buttonConnect);
            this.Controls.Add(buttonDisconnect);
            this.Controls.Add(labelCellCount);
            this.Controls.Add(chartReal);
            this.Controls.Add(chartImag);
            this.Controls.Add(buttonPumpOn);
            this.Controls.Add(buttonPumpOff);
            this.Controls.Add(buttonPumpInc);
            this.Controls.Add(buttonPumpDec);
            this.Controls.Add(buttonMagnetOn);
            this.Controls.Add(buttonMagnetOff);
            serialPort = new SerialPort();
            serialPort.DataReceived += SerialPort_DataReceived;
            buttonScanPorts.Click += ButtonScanPorts_Click;
            buttonConnect.Click += ButtonConnect_Click;
            buttonDisconnect.Click += ButtonDisconnect_Click;
            buttonPumpOn.Click += (s, e) => SendCommand("PUMP_ON");
            buttonPumpOff.Click += (s, e) => SendCommand("PUMP_OFF");
            buttonPumpInc.Click += (s, e) => SendCommand("PUMP_INC");
            buttonPumpDec.Click += (s, e) => SendCommand("PUMP_DEC");
            buttonMagnetOn.Click += (s, e) => SendCommand("MAGNET_ON");
            buttonMagnetOff.Click += (s, e) => SendCommand("MAGNET_OFF");
            chartReal.MouseWheel += Chart_MouseWheel;
            chartImag.MouseWheel += Chart_MouseWheel;
            ScanPorts();
        }

        private void InitChart(Chart chart, string title)
        {
            chart.ChartAreas.Clear();
            var area = new ChartArea("MainArea");
            area.AxisX.Title = "Thời gian (s)";
            area.AxisY.Title = title;
            area.CursorX.IsUserEnabled = true;
            area.CursorX.IsUserSelectionEnabled = true;
            area.CursorY.IsUserEnabled = true;
            area.CursorY.IsUserSelectionEnabled = true;
            area.AxisX.ScaleView.Zoomable = true;
            area.AxisY.ScaleView.Zoomable = true;
            chart.ChartAreas.Add(area);
            chart.Series.Clear();
            var series = new Series("Data")
            {
                ChartType = SeriesChartType.Line,
                Color = title.Contains("thực") ? Color.Blue : Color.Red
            };
            chart.Series.Add(series);
        }

        private void Chart_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = sender as Chart;
            var area = chart.ChartAreas[0];
            try
            {
                if (e.Delta < 0)
                {
                    area.AxisX.ScaleView.ZoomReset();
                    area.AxisY.ScaleView.ZoomReset();
                }
                else
                {
                    double xMin = area.AxisX.ScaleView.ViewMinimum;
                    double xMax = area.AxisX.ScaleView.ViewMaximum;
                    double yMin = area.AxisY.ScaleView.ViewMinimum;
                    double yMax = area.AxisY.ScaleView.ViewMaximum;
                    double xZoom = (xMax - xMin) * 0.8;
                    double yZoom = (yMax - yMin) * 0.8;
                    area.AxisX.ScaleView.Zoom(xMin, xMin + xZoom);
                    area.AxisY.ScaleView.Zoom(yMin, yMin + yZoom);
                }
            }
            catch { }
        }

        private void ScanPorts()
        {
            comboBoxPorts.Items.Clear();
            comboBoxPorts.Items.AddRange(SerialPort.GetPortNames());
            if (comboBoxPorts.Items.Count > 0)
                comboBoxPorts.SelectedIndex = 0;
        }

        private void ButtonScanPorts_Click(object sender, EventArgs e)
        {
            ScanPorts();
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            if (comboBoxPorts.SelectedItem != null && comboBoxBaudrate.SelectedItem != null)
            {
                serialPort.PortName = comboBoxPorts.SelectedItem.ToString();
                serialPort.BaudRate = int.Parse(comboBoxBaudrate.SelectedItem.ToString());
                try
                {
                    serialPort.Open();
                    MessageBox.Show("Đã kết nối!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void ButtonDisconnect_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                MessageBox.Show("Đã ngắt kết nối!");
            }
        }

        private void SendCommand(string cmd)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine(cmd);
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string line = serialPort.ReadLine();
                var parts = line.Trim().Split(',');
                if (parts.Length == 2 &&
                    double.TryParse(parts[0], out double real) &&
                    double.TryParse(parts[1], out double imag))
                {
                    time += 0.1;
                    timeData.Add(time);
                    realData.Add(real);
                    imagData.Add(imag);
                    if (real > 1000) cellCount++;
                    this.Invoke((MethodInvoker)delegate
                    {
                        chartReal.Series["Data"].Points.AddXY(time, real);
                        chartImag.Series["Data"].Points.AddXY(time, imag);
                        if (chartReal.Series["Data"].Points.Count > 200)
                            chartReal.Series["Data"].Points.RemoveAt(0);
                        if (chartImag.Series["Data"].Points.Count > 200)
                            chartImag.Series["Data"].Points.RemoveAt(0);
                        labelCellCount.Text = $"Số lượng tế bào: {cellCount}";
                    });
                }
            }
            catch { }
        }
    }
}