using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Controls;
using System.Windows.Media;
using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using DataAccess.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SEP490_G17_Desktop.ControlChart
{
    /// <summary>
    /// Interaction logic for GPACapstoneControl.xaml
    /// </summary>
    public partial class GPACapstoneControl : UserControl
    {
        private readonly IRepositoryGPACapstone repositoryGPACapstone = new RepositoryGPACapstone();
        public SeriesCollection SeriesCollection { get; set; }
        public ObservableCollection<string> Labels { get; set; }
        public GPACapstoneControl()
        {
            InitializeComponent();
            SeriesCollection = new SeriesCollection();
            Labels = new ObservableCollection<string>();
            ConfigChart();
            //UpdateChart(majorFirst);
            //UpdateChart("SE", 2024);
            this.DataContext = this;
        }

        public void UpdateChart(string major, int year)
        {
            List<GPAScoreRange> listData = repositoryGPACapstone.GetValueGPACapstoneByMajorAndYear(major, year);
            ChartValues<ObservableValue>  values = new ChartValues<ObservableValue>();
            ObservableCollection<string> labels = new ObservableCollection<string>();
            foreach (GPAScoreRange range in listData)
            {
                values.Add(new ObservableValue(range.Count));
                Labels.Add(range.ScoreRange);
            }
            DrawChartbyMajor(major, values, year);
        }

        private void DrawChartbyMajor(string major, ChartValues<ObservableValue> values, int year)
        {
            Random random = new Random();

            Color randomColor = Color.FromArgb(255, (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));

            var lineSeries = new LineSeries()
            {
                Title = "Students/Score" + major + year,
                Values = values,
                Stroke = new SolidColorBrush(randomColor),
                Fill = new SolidColorBrush(Color.FromArgb(50, randomColor.R, randomColor.G, randomColor.B)),
                LineSmoothness = 0
            };
            SeriesCollection.Add(lineSeries);
        }
        public void RemoveAllSeries()
        {
            var lineSeriesToRemove = SeriesCollection.ToList();
            foreach (var item in lineSeriesToRemove)
            {
                SeriesCollection.Remove(item);
            }
        }

        public void RemoveSersiesByMajorEndYear(string major, string year)
        {
            var lineSeriesToRemove = SeriesCollection.Where(series => series.Title.Contains(major + year)).ToList();
            var lineSeriesSliderToRemove = SeriesCollection.Where(series => series.Title.Contains("Slider")).ToList();
            foreach (var item in lineSeriesToRemove)
            {
                SeriesCollection.Remove(item);
            }
            foreach (var item in lineSeriesSliderToRemove)
            {
                SeriesCollection.Remove(item);
            }
        }
        public async Task UpdateChartSlider(string major, int year)
        {
            List<GPAScoreRange> listData = repositoryGPACapstone.GetValueGPACapstoneByMajorAndYear(major, year);
            ChartValues<ObservableValue> values = new ChartValues<ObservableValue>();
            ObservableCollection<string> labels = new ObservableCollection<string>();
            foreach (GPAScoreRange range in listData)
            {
                values.Add(new ObservableValue(range.Count));
                if (Labels != null)
                {
                    Labels.Add(range.ScoreRange);
                }
            }
            await UpdateFirstLineSeriesValues(major, values, year);
        }
        private async Task UpdateFirstLineSeriesValues(string major, ChartValues<ObservableValue> values, int year)
        {
            if (SeriesCollection.Any())
            {
                var lineSeries = SeriesCollection[0].Values as ChartValues<ObservableValue>;

                for (int i = 0; i < lineSeries.Count && i < values.Count; i++)
                {
                    // Lấy giá trị hiện tại của lineSeries[i] và giá trị mới từ values[i]
                    int currentValue = (int)lineSeries[i].Value;
                    int targetValue = (int)values[i].Value;
                    int start = 0;
                    int end = 0;
                    if (currentValue > targetValue)
                    {
                        start = targetValue;
                        end = targetValue;
                    }
                    else if (currentValue < targetValue)
                    {
                        start = currentValue;
                        end = targetValue;
                    }
                    else
                    {
                        start = currentValue;
                        end = currentValue;
                    }
                    UpdateSliderTask(lineSeries, i, start, end);
                }
            }
            else
            {
                ObservableValue[] zeroValuesArray = new ObservableValue[values.Count];
                for (int i = 0; i < values.Count; i++)
                {
                    zeroValuesArray[i] = new ObservableValue(0);
                }

                ChartValues<ObservableValue> valueZero = new ChartValues<ObservableValue>(zeroValuesArray);
                Random random = new Random();

                Color randomColor = Color.FromArgb(255, (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));

                var lineSeries = new LineSeries()
                {
                    Title = "Students/Score" + major + " Slider",
                    Values = valueZero,
                    Stroke = new SolidColorBrush(randomColor),
                    Fill = new SolidColorBrush(Color.FromArgb(50, randomColor.R, randomColor.G, randomColor.B)),
                    LineSmoothness = 0
                };
                SeriesCollection.Add(lineSeries);

                for (int i = 0; i < values.Count; i++)
                {
                    int targetValue = (int)values[i].Value;
                    for (int j = 0; j <= targetValue; j++)
                    {
                        lineSeries.Values[i] = new ObservableValue(j);
                        await Task.Delay(100);
                    }
                }
            }
        }
        private async Task UpdateSliderTask(ChartValues<ObservableValue>? lineSeries, int i, int start, int end)
        {
            for (int j = start; j <= end; j++)
            {
                lineSeries[i] = new ObservableValue(j);
                await Task.Delay(200);
            }
        }

        public void ViewInformation(GPACapstoneViewInformation gPACapstoneViewInformation)
        {
            var lineSeriesToRemove = SeriesCollection.Where(s => s is LineSeries).ToList();
            gPACapstoneViewInformation.Data = lineSeriesToRemove;
            gPACapstoneViewInformation.Labels = Labels;
            gPACapstoneViewInformation.UpdateChart();
        }

        public void BackViewInformation()
        {
            var lineSeriesToRemove = SeriesCollection.Where(s => s is ColumnSeries);
            foreach (var lineSeries in lineSeriesToRemove)
            {
                SeriesCollection.Remove(lineSeries);
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
