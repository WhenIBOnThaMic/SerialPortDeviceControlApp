
using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SerialPortDeviceControlApp
{
    public class MainForm : Form
    {
        // UI Controls
        private ComboBox comboBoxPorts, comboBoxBaudrate;
        private Button buttonScanPorts, buttonConnect, buttonDisconnect;
        private Label labelCellCount;
        private Chart chartReal, chartImag;
        private Panel panelGraphReal, panelGraphImag;
        private Button buttonZoomInReal, buttonZoomOutReal, buttonPanLeftReal, buttonPanRightReal;
        private Button buttonZoomInImag, buttonZoomOutImag, buttonPanLeftImag, buttonPanRightImag;
        private Panel panelPump, panelMagnet;
        private Button buttonPumpOn, buttonPumpOff, buttonPumpInc, buttonPumpDec;
        private Button buttonMagnetOn, buttonMagnetOff;
        private Label labelPumpSpeed;

        // Serial
        private SerialPort serialPort = new SerialPort();
        private int cellCount = 1;
        private int pumpSpeed = 0;

        public MainForm()
        {
            this.Text = "Serial Device Control & Graph";
            this.Width = 1200;
            this.Height = 800;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Left panel: controls
            var leftPanel = new Panel { Left = 0, Top = 0, Width = 320, Height = this.Height, Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom };

            comboBoxPorts = new ComboBox { Left = 10, Top = 10, Width = 120 };
            comboBoxBaudrate = new ComboBox { Left = 150, Top = 10, Width = 120 };
            comboBoxBaudrate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            comboBoxBaudrate.SelectedIndex = 0;
            buttonScanPorts = new Button { Left = 10, Top = 40, Width = 120, Text = "Quét cổng" };
            buttonConnect = new Button { Left = 150, Top = 40, Width = 120, Text = "Kết nối" };
            buttonDisconnect = new Button { Left = 10, Top = 70, Width = 120, Text = "Ngắt kết nối" };

            leftPanel.Controls.Add(comboBoxPorts);
            leftPanel.Controls.Add(comboBoxBaudrate);
            leftPanel.Controls.Add(buttonScanPorts);
            leftPanel.Controls.Add(buttonConnect);
            leftPanel.Controls.Add(buttonDisconnect);

            // Cell count
            labelCellCount = new Label {
                Left = 10,
                Top = 110,
                Width = 300,
                Height = 30,
                Text = "Số lượng tế bào: " + cellCount,
                Font = new Font("Arial", 12),
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            leftPanel.Controls.Add(labelCellCount);

            // Pump controls
            panelPump = new Panel { Left = 10, Top = 150, Width = 300, Height = 140 };
            var labelPump = new Label { Text = "Bơm vi lưu", Font = new Font("Arial", 12, FontStyle.Bold), Left = 0, Top = 0, Width = 300 };
            buttonPumpOn = new Button { Left = 10, Top = 30, Width = 120, Text = "Bật bơm" };
            buttonPumpOff = new Button { Left = 150, Top = 30, Width = 120, Text = "Tắt bơm" };
            buttonPumpInc = new Button { Left = 10, Top = 70, Width = 120, Text = "Tăng tốc" };
            buttonPumpDec = new Button { Left = 150, Top = 70, Width = 120, Text = "Giảm tốc" };
            labelPumpSpeed = new Label { Left = 10, Top = 110, Width = 260, Height = 20, Text = "Tốc độ bơm: 0", Font = new Font("Arial", 10) };
            panelPump.Controls.Add(labelPump);
            panelPump.Controls.Add(buttonPumpOn);
            panelPump.Controls.Add(buttonPumpOff);
            panelPump.Controls.Add(buttonPumpInc);
            panelPump.Controls.Add(buttonPumpDec);
            panelPump.Controls.Add(labelPumpSpeed);
            leftPanel.Controls.Add(panelPump);

            // Magnet controls
            panelMagnet = new Panel { Left = 10, Top = 300, Width = 300, Height = 80 };
            var labelMagnet = new Label { Text = "Nam châm điện", Font = new Font("Arial", 12, FontStyle.Bold), Left = 0, Top = 0, Width = 300 };
            buttonMagnetOn = new Button { Left = 10, Top = 30, Width = 120, Text = "Bật nam châm" };
            buttonMagnetOff = new Button { Left = 150, Top = 30, Width = 120, Text = "Tắt nam châm" };
            panelMagnet.Controls.Add(labelMagnet);
            panelMagnet.Controls.Add(buttonMagnetOn);
            panelMagnet.Controls.Add(buttonMagnetOff);
            leftPanel.Controls.Add(panelMagnet);

            this.Controls.Add(leftPanel);

            // Right panel: charts
            var rightPanel = new Panel { Left = leftPanel.Width, Top = 0, Width = this.Width - leftPanel.Width - 20, Height = this.Height, Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom };

            panelGraphReal = new Panel { Left = 10, Top = 20, Width = rightPanel.Width - 20, Height = (rightPanel.Height - 100) / 2 };
            chartReal = new Chart { Left = 0, Top = 0, Width = panelGraphReal.Width - 120, Height = panelGraphReal.Height - 40, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
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

            panelGraphImag = new Panel { Left = 10, Top = panelGraphReal.Bottom + 20, Width = rightPanel.Width - 20, Height = (rightPanel.Height - 100) / 2 };
            chartImag = new Chart { Left = 0, Top = 0, Width = panelGraphImag.Width - 120, Height = panelGraphImag.Height - 40, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
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

            // Event handlers
            buttonScanPorts.Click += (s, e) => ScanPorts();
            buttonConnect.Click += (s, e) => ConnectSerial();
            buttonDisconnect.Click += (s, e) => DisconnectSerial();
            buttonPumpOn.Click += (s, e) => SendCommand("PUMP_ON");
            buttonPumpOff.Click += (s, e) => SendCommand("PUMP_OFF");
            buttonPumpInc.Click += (s, e) => ChangePumpSpeed(1);
            buttonPumpDec.Click += (s, e) => ChangePumpSpeed(-1);
            buttonMagnetOn.Click += (s, e) => SendCommand("MAGNET_ON");
            buttonMagnetOff.Click += (s, e) => SendCommand("MAGNET_OFF");
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

            serialPort.DataReceived += SerialPort_DataReceived;
            ScanPorts();
        }

        private void ScanPorts()
        {
            comboBoxPorts.Items.Clear();
            comboBoxPorts.Items.AddRange(SerialPort.GetPortNames());
        }

        private void ConnectSerial()
        {
            if (comboBoxPorts.SelectedItem == null || comboBoxBaudrate.SelectedItem == null) return;
            serialPort.PortName = comboBoxPorts.SelectedItem.ToString();
            serialPort.BaudRate = int.Parse(comboBoxBaudrate.SelectedItem.ToString());
            try { serialPort.Open(); } catch { MessageBox.Show("Không thể kết nối"); }
        }

        private void DisconnectSerial()
        {
            if (serialPort.IsOpen) serialPort.Close();
        }

        private void SendCommand(string cmd)
        {
            if (serialPort.IsOpen) serialPort.WriteLine(cmd);
        }

        private void ChangePumpSpeed(int delta)
        {
            pumpSpeed += delta;
            labelPumpSpeed.Text = $"Tốc độ bơm: {pumpSpeed}";
            SendCommand(delta > 0 ? "PUMP_INC" : "PUMP_DEC");
        }

        private void InitChart(Chart chart, string title)
        {
            chart.ChartAreas.Clear();
            chart.Series.Clear();
            chart.Titles.Clear();
            chart.Titles.Add(title);
            var area = new ChartArea();
            chart.ChartAreas.Add(area);
            var series = new Series { ChartType = SeriesChartType.Line };
            chart.Series.Add(series);
        }

        private void Chart_MouseWheel(Chart chart, MouseEventArgs e)
        {
            if (e.Delta > 0) ZoomChart(chart, 0.8);
            else ZoomChart(chart, 1.25);
        }

        private void ZoomChart(Chart chart, double factor)
        {
            var area = chart.ChartAreas[0];
            double min = area.AxisX.ScaleView.ViewMinimum;
            double max = area.AxisX.ScaleView.ViewMaximum;
            double center = (min + max) / 2;
            double newRange = (max - min) * factor;
            area.AxisX.ScaleView.Zoom(center - newRange / 2, center + newRange / 2);
        }

        private void PanChart(Chart chart, double percent)
        {
            var area = chart.ChartAreas[0];
            double range = area.AxisX.ScaleView.ViewMaximum - area.AxisX.ScaleView.ViewMinimum;
            double shift = range * percent;
            area.AxisX.ScaleView.Zoom(area.AxisX.ScaleView.ViewMinimum + shift, area.AxisX.ScaleView.ViewMaximum + shift);
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string line = serialPort.ReadLine();
                // Expecting x,y
                var parts = line.Split(',');
                if (parts.Length == 2 && double.TryParse(parts[0], out double real) && double.TryParse(parts[1], out double imag))
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        chartReal.Series[0].Points.AddXY(DateTime.Now, real);
                        chartImag.Series[0].Points.AddXY(DateTime.Now, imag);
                        // Example cell count logic: increment if real > threshold
                        if (real > 100) cellCount++;
                        labelCellCount.Text = $"Số lượng tế bào: {cellCount}";
                    });
                }
            }
            catch { }
        }
    }
}
