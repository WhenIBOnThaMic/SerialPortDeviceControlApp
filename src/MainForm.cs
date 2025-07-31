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
        // Controls for settings
        private ComboBox comboBoxPorts;
        private ComboBox comboBoxBaudrate;
        private Button buttonScanPorts;
        private Button buttonConnect;
        private Button buttonDisconnect;
        private Label labelSetting;
        private Panel panelSetting;

        // Controls for pump
        private Button buttonPumpOn;
        private Button buttonPumpOff;
        private Button buttonPumpInc;
        private Button buttonPumpDec;
        private Label labelPump;
        private Panel panelPump;

        // Controls for magnet
        private Button buttonMagnetOn;
        private Button buttonMagnetOff;
        private Label labelMagnet;
        private Panel panelMagnet;

        // Cell count
        private Label labelCellCount;

        // Graph controls
        private Chart chartReal;
        private Chart chartImag;
        private Panel panelGraphReal;
        private Panel panelGraphImag;
        private Button buttonZoomInReal;
        private Button buttonZoomOutReal;
        private Button buttonPanLeftReal;
        private Button buttonPanRightReal;
        private Button buttonZoomInImag;
        private Button buttonZoomOutImag;
        private Button buttonPanLeftImag;
        private Button buttonPanRightImag;

        // Serial
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
            this.Height = 800;
            this.StartPosition = FormStartPosition.CenterScreen;

            // --- Left panel for controls ---
            var leftPanel = new Panel { Left = 0, Top = 0, Width = 320, Height = this.Height, Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom };
            int y = 20;

            // Setting máy group
            panelSetting = new Panel { Left = 10, Top = y, Width = 300, Height = 120 };
            labelSetting = new Label { Text = "Setting máy", Font = new Font("Arial", 12, FontStyle.Bold), Left = 0, Top = 0, Width = 300 };
            comboBoxPorts = new ComboBox { Left = 10, Top = 30, Width = 120 };
            comboBoxBaudrate = new ComboBox { Left = 140, Top = 30, Width = 80 };
            comboBoxBaudrate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            comboBoxBaudrate.SelectedIndex = 0;
            buttonScanPorts = new Button { Left = 10, Top = 70, Width = 80, Text = "Quét cổng" };
            buttonConnect = new Button { Left = 100, Top = 70, Width = 80, Text = "Kết nối" };
            buttonDisconnect = new Button { Left = 190, Top = 70, Width = 80, Text = "Ngắt" };
            panelSetting.Controls.Add(labelSetting);
            panelSetting.Controls.Add(comboBoxPorts);
            panelSetting.Controls.Add(comboBoxBaudrate);
            panelSetting.Controls.Add(buttonScanPorts);
            panelSetting.Controls.Add(buttonConnect);
            panelSetting.Controls.Add(buttonDisconnect);
            leftPanel.Controls.Add(panelSetting);
            y += panelSetting.Height + 10;

            // Bơm group
            panelPump = new Panel { Left = 10, Top = y, Width = 300, Height = 120 };
            labelPump = new Label { Text = "Bơm", Font = new Font("Arial", 12, FontStyle.Bold), Left = 0, Top = 0, Width = 300 };
            buttonPumpOn = new Button { Left = 10, Top = 30, Width = 120, Text = "Bật bơm" };
            buttonPumpOff = new Button { Left = 150, Top = 30, Width = 120, Text = "Tắt bơm" };
            buttonPumpInc = new Button { Left = 10, Top = 70, Width = 120, Text = "Tăng tốc" };
            buttonPumpDec = new Button { Left = 150, Top = 70, Width = 120, Text = "Giảm tốc" };
            panelPump.Controls.Add(labelPump);
            panelPump.Controls.Add(buttonPumpOn);
            panelPump.Controls.Add(buttonPumpOff);
            panelPump.Controls.Add(buttonPumpInc);
            panelPump.Controls.Add(buttonPumpDec);
            leftPanel.Controls.Add(panelPump);
            y += panelPump.Height + 10;

            // Nam Châm group
            panelMagnet = new Panel { Left = 10, Top = y, Width = 300, Height = 80 };
            labelMagnet = new Label { Text = "Nam Châm", Font = new Font("Arial", 12, FontStyle.Bold), Left = 0, Top = 0, Width = 300 };
            buttonMagnetOn = new Button { Left = 10, Top = 30, Width = 120, Text = "Bật nam châm" };
            buttonMagnetOff = new Button { Left = 150, Top = 30, Width = 120, Text = "Tắt nam châm" };
            panelMagnet.Controls.Add(labelMagnet);
            panelMagnet.Controls.Add(buttonMagnetOn);
            panelMagnet.Controls.Add(buttonMagnetOff);
            leftPanel.Controls.Add(panelMagnet);
            y += panelMagnet.Height + 10;

            // Cell count label
            labelCellCount = new Label { Left = 10, Top = y, Width = 300, Height = 40, Text = "Số lượng tế bào: 0", Font = new Font("Arial", 12) };
            leftPanel.Controls.Add(labelCellCount);

            this.Controls.Add(leftPanel);

            // --- Right panel for graphs ---
            var rightPanel = new Panel { Left = leftPanel.Width, Top = 0, Width = this.Width - leftPanel.Width - 20, Height = this.Height, Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom };

            // Real graph panel
            panelGraphReal = new Panel { Left = 10, Top = 20, Width = rightPanel.Width - 20, Height = (rightPanel.Height - 60) / 2 };
            chartReal = new Chart { Left = 0, Top = 0, Width = panelGraphReal.Width - 120, Height = panelGraphReal.Height - 10, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            InitChart(chartReal, "Trở kháng thực");
            buttonZoomInReal = new Button { Left = chartReal.Width + 10, Top = 10, Width = 100, Text = "Zoom In" };
            buttonZoomOutReal = new Button { Left = chartReal.Width + 10, Top = 50, Width = 100, Text = "Zoom Out" };
            buttonPanLeftReal = new Button { Left = chartReal.Width + 10, Top = 90, Width = 100, Text = "←" };
            buttonPanRightReal = new Button { Left = chartReal.Width + 10, Top = 130, Width = 100, Text = "→" };
            panelGraphReal.Controls.Add(chartReal);
            panelGraphReal.Controls.Add(buttonZoomInReal);
            panelGraphReal.Controls.Add(buttonZoomOutReal);
            panelGraphReal.Controls.Add(buttonPanLeftReal);
            panelGraphReal.Controls.Add(buttonPanRightReal);
            rightPanel.Controls.Add(panelGraphReal);

            // Imag graph panel
            panelGraphImag = new Panel { Left = 10, Top = panelGraphReal.Bottom + 20, Width = rightPanel.Width - 20, Height = (rightPanel.Height - 60) / 2 };
            chartImag = new Chart { Left = 0, Top = 0, Width = panelGraphImag.Width - 120, Height = panelGraphImag.Height - 10, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            InitChart(chartImag, "Trở kháng ảo");
            buttonZoomInImag = new Button { Left = chartImag.Width + 10, Top = 10, Width = 100, Text = "Zoom In" };
            buttonZoomOutImag = new Button { Left = chartImag.Width + 10, Top = 50, Width = 100, Text = "Zoom Out" };
            buttonPanLeftImag = new Button { Left = chartImag.Width + 10, Top = 90, Width = 100, Text = "←" };
            buttonPanRightImag = new Button { Left = chartImag.Width + 10, Top = 130, Width = 100, Text = "→" };
            panelGraphImag.Controls.Add(chartImag);
            panelGraphImag.Controls.Add(buttonZoomInImag);
            panelGraphImag.Controls.Add(buttonZoomOutImag);
            panelGraphImag.Controls.Add(buttonPanLeftImag);
            panelGraphImag.Controls.Add(buttonPanRightImag);
            rightPanel.Controls.Add(panelGraphImag);

            this.Controls.Add(rightPanel);

            // Serial port
            serialPort = new SerialPort();
            serialPort.DataReceived += SerialPort_DataReceived;

            // Events
            buttonScanPorts.Click += ButtonScanPorts_Click;
            buttonConnect.Click += ButtonConnect_Click;
            buttonDisconnect.Click += ButtonDisconnect_Click;
            buttonPumpOn.Click += (s, e) => SendCommand("PUMP_ON");
            buttonPumpOff.Click += (s, e) => SendCommand("PUMP_OFF");
            buttonPumpInc.Click += (s, e) => SendCommand("PUMP_INC");
            buttonPumpDec.Click += (s, e) => SendCommand("PUMP_DEC");
            buttonMagnetOn.Click += (s, e) => SendCommand("MAGNET_ON");
            buttonMagnetOff.Click += (s, e) => SendCommand("MAGNET_OFF");

            // Graph controls
            buttonZoomInReal.Click += (s, e) => ZoomChart(chartReal, 0.8);
            buttonZoomOutReal.Click += (s, e) => ZoomChart(chartReal, 1.25);
            buttonPanLeftReal.Click += (s, e) => PanChart(chartReal, -0.2);
            buttonPanRightReal.Click += (s, e) => PanChart(chartReal, 0.2);
            buttonZoomInImag.Click += (s, e) => ZoomChart(chartImag, 0.8);
            buttonZoomOutImag.Click += (s, e) => ZoomChart(chartImag, 1.25);
            buttonPanLeftImag.Click += (s, e) => PanChart(chartImag, -0.2);
            buttonPanRightImag.Click += (s, e) => PanChart(chartImag, 0.2);

            chartReal.MouseWheel += (s, e) => Chart_MouseWheel(chartReal, e);
            chartImag.MouseWheel += (s, e) => Chart_MouseWheel(chartImag, e);

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

        private void Chart_MouseWheel(Chart chart, MouseEventArgs e)
        {
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

        private void ZoomChart(Chart chart, double factor)
        {
            var area = chart.ChartAreas[0];
            double xMin = area.AxisX.ScaleView.ViewMinimum;
            double xMax = area.AxisX.ScaleView.ViewMaximum;
            double yMin = area.AxisY.ScaleView.ViewMinimum;
            double yMax = area.AxisY.ScaleView.ViewMaximum;
            double xCenter = (xMin + xMax) / 2.0;
            double yCenter = (yMin + yMax) / 2.0;
            double xRange = (xMax - xMin) * factor;
            double yRange = (yMax - yMin) * factor;
            area.AxisX.ScaleView.Zoom(xCenter - xRange / 2, xCenter + xRange / 2);
            area.AxisY.ScaleView.Zoom(yCenter - yRange / 2, yCenter + yRange / 2);
        }

        private void PanChart(Chart chart, double percent)
        {
            var area = chart.ChartAreas[0];
            double xMin = area.AxisX.ScaleView.ViewMinimum;
            double xMax = area.AxisX.ScaleView.ViewMaximum;
            double xRange = xMax - xMin;
            double shift = xRange * percent;
            area.AxisX.ScaleView.Zoom(xMin + shift, xMax + shift);
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