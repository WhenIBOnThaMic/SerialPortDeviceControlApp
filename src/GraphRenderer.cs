using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SerialPortDeviceControlApp
{
    public class GraphRenderer
    {
        private Chart chart;

        public GraphRenderer(Chart chart)
        {
            this.chart = chart;
            InitializeChart();
        }

        private void InitializeChart()
        {
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.ChartAreas.Add(new ChartArea("MainArea"));

            Series series = new Series("DataSeries")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 5
            };

            chart.Series.Add(series);
        }

        public void PlotDataPoint(double x, double y)
        {
            chart.Invoke((MethodInvoker)delegate
            {
                chart.Series["DataSeries"].Points.AddXY(x, y);
                chart.Invalidate();
            });
        }

        public void ClearGraph()
        {
            chart.Invoke((MethodInvoker)delegate
            {
                chart.Series["DataSeries"].Points.Clear();
                chart.Invalidate();
            });
        }

        public void UpdateGraph(List<double> xValues, List<double> yValues)
        {
            chart.Invoke((MethodInvoker)delegate
            {
                ClearGraph();
                for (int i = 0; i < xValues.Count; i++)
                {
                    chart.Series["DataSeries"].Points.AddXY(xValues[i], yValues[i]);
                }
                chart.Invalidate();
            });
        }
    }
}