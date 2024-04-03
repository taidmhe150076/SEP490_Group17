using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;


namespace SEP490_G17_Desktop.ControlChart
{
    /// <summary>
    /// Interaction logic for GPACapstoneViewInformation.xaml
    /// </summary>
    public partial class GPACapstoneViewInformation : UserControl
    {
        public IEnumerable<object> Data {  get; set; }
        public SeriesCollection SeriesCollection { get; set; }
        public ObservableCollection<string> Labels { get; set; }
        public GPACapstoneViewInformation()
        {
            InitializeComponent();
            SeriesCollection = new SeriesCollection();
            Labels = new ObservableCollection<string>();
            ConfigChart();
            this.DataContext = this;
        }
        public void UpdateChart()
        {
            if (Data.Count() > 0)
            {
                RemoveAllSeries();
                foreach (LineSeries lineSeries in Data.ToList())
                {
                    var valueTrans = lineSeries.Values;

                    if (lineSeries != null)
                    {
                        var lineSeriesFillBrush = (lineSeries as LineSeries).Fill as SolidColorBrush;
                        if (lineSeriesFillBrush != null)
                        {
                            var lineSeriesFillColor = lineSeriesFillBrush.Color;

                            var columnSeries = new ColumnSeries
                            {
                                Title = lineSeries.Title,
                                Values = valueTrans,
                                Fill = new SolidColorBrush(lineSeriesFillColor), // Sử dụng màu fill của LineSeries cho ColumnSeries
                                Stroke = Brushes.Black, // Màu viền của cột
                                StrokeThickness = 1, // Độ dày của viền
                                DataLabels = true,
                                LabelPoint = point => point.Y != 0 ? point.Y.ToString() : ""
                            };

                            SeriesCollection.Add(columnSeries);
                        }
                    }
                }
            }
            else
            {
                RemoveAllSeries();
                this.Visibility = System.Windows.Visibility.Visible;
            }
        }
        public void RemoveAllSeries()
        {
            var lineSeriesToRemove = SeriesCollection.ToList();
            foreach (var item in lineSeriesToRemove)
            {
                SeriesCollection.Remove(item);
            }
        }
        private void ConfigChart()
        {
            chart.AxisY[0].MinValue = 0;
            chart.AxisY[0].Separator.Step = 1;
            chart.AxisX[0].MinValue = 0;
            chart.AxisX[0].Separator.Step = 1;
        }
    }
}
