using DataAccess.FileControl;
using DataAccess.Models;
using LiveCharts.Wpf.Charts.Base;
using Microsoft.Win32;
using SEP490_G17_Desktop.ControlChart;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SEP490_G17_Desktop
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ChartBaseController : Window
    {
        private string majorFirst;
        public const string CheckBox_Name = "checkbox";
        public string ToolName { get; set; }
        List<CheckBox> listCheckBoxMajor = new List<CheckBox>();
        CheckBox isCheckedMajor = new CheckBox();
        List<int> yearChecked = new List<int>();
        int numberClick = 0;
        private GPACapstoneViewInformation gPACapstoneViewInformation;

        public ChartBaseController(object sender)
        {
            InitializeComponent();
            Button button = (Button)sender;
            ToolName = button.Content.ToString();
            switch (ToolName)
            {
                case "GPA Capstone":
                    //controlAction.Visibility = Visibility.Hidden;
                    //nameTool.Text = ToolName;
                    break;
                case "":
                    break;
                default:
                    break;
            }
            CreateCheckboxYear();
            this.DataContext = this;
        }
        private void Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            this.isCheckedMajor = checkbox;
            var nameCheckbox = checkbox.Name.ToString();
            var splitCheckboxName = nameCheckbox.Split("_");
            var major = splitCheckboxName[1];
            var year = splitCheckboxName[splitCheckboxName.Length - 1];
            //userControlChart.UpdateChart(major, Convert.ToInt32(year));
        }
        private void Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            var nameCheckbox = checkbox.Name.ToString();
            var splitCheckboxName = nameCheckbox.Split("_");
            var major = splitCheckboxName[1];
            var year = splitCheckboxName[splitCheckboxName.Length - 1];
            this.isCheckedMajor = new CheckBox();
            //userControlChart.RemoveSersiesByMajorEndYear(major, year);
        }

        private void CheckboxYear_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            var year = checkbox.Content.ToString();
            this.yearChecked.Add(Convert.ToInt32(year));
            this.yearChecked = this.yearChecked.OrderByDescending(x => x).ToList();
            //slider.Maximum = this.yearChecked.First();
            //slider.Minimum = this.yearChecked.Last();
            CreateCheckboxMajor(Convert.ToInt32(year));
        }
        private void CheckboxYear_UnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            var year = checkbox.Content.ToString();
            this.yearChecked.Remove(Convert.ToInt32(year));
            if (this.yearChecked.Count() > 0)
            {
                //slider.Maximum = this.yearChecked.First();
                //slider.Minimum = this.yearChecked.Last();
            }

            // clear checkbox Major tương ứng với checkbox vừa unchecked
            //var checkboxToRemove = this.checkboxListBox.Items.OfType<CheckBox>()
            //                                    .Where(cb => cb.Content.ToString().Contains(year)).ToList();
            //foreach (var item in checkboxToRemove)
            //{
            //    var splitItem = item.Name.Split("_");
            //    this.checkboxListBox.Items.Remove(item);
            //    listCheckBoxMajor.Remove(item);
            //    userControlChart.RemoveSersiesByMajorEndYear(splitItem[1], splitItem[2]);
            //}

        }

        private void CreateCheckboxYear()
        {
            List<GradesGPA> readCsvTask = FileControl.Instance.GetObjectList();

            var listCheckboxMajor = readCsvTask.Select(x => x.MemberMajor).Distinct().ToList();
            var listYear = readCsvTask.Select(x => x.Year).Distinct().OrderByDescending(x => x).ToList();
            foreach (var item in listYear)
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Content = item;
                checkbox.Name = CheckBox_Name + item;
                checkbox.Checked += CheckboxYear_Checked;
                checkbox.Unchecked += CheckboxYear_UnChecked;
                //checkboxYear.Items.Add(checkbox);
            }
        }
        private void CreateCheckboxMajor(int year)
        {
            List<GradesGPA> readCsvTask = FileControl.Instance.GetObjectList();

            var CheckboxMajor = readCsvTask.Where(x => x.Year == year).Select(x => x.MemberMajor).Distinct().ToList();
            this.majorFirst = CheckboxMajor[0];
            foreach (var item in CheckboxMajor)
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Content = item + "(" + year + ")";
                checkbox.Name = CheckBox_Name + "_" + item + "_" + year;
                checkbox.Checked += Checkbox_Checked;
                checkbox.Unchecked += Checkbox_Unchecked;
                //checkboxListBox.Items.Add(checkbox);
                listCheckBoxMajor.Add(checkbox);
            }
        }
        private async void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            int checkedCount = 0;
            string major = "";
            foreach (var item in listCheckBoxMajor)
            {
                if (item.IsChecked == true)
                {
                    var splitCheckboxName = item.Name.Split("_");
                    major = splitCheckboxName[1];

                    checkedCount++;
                }
                if (checkedCount >= 2)
                {
                    MessageBox.Show("Please select one Major to perform the slider function!");
                    return;
                }
            }

            if (checkedCount == 0)
            {
                MessageBox.Show("Please select Major!");
                return;
            }

            //userControlChart.RemoveAllSeries();
            this.yearChecked = this.yearChecked.OrderByDescending(x => x).ToList();

            // Check checked Year
            if (this.yearChecked.Count() == 0)
            {
                MessageBox.Show("Please select Year!");
                return;
            }


            //for (int i = this.yearChecked.Last(); i <= this.yearChecked.First(); i++)
            //{
            //    await userControlChart.UpdateChartSlider(major, i);
            //    slider.Value = i;
            //    await Task.Delay(1000);
            //}
        }

        private async void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string nameCheckbox = "";
            foreach (var item in listCheckBoxMajor)
            {
                if (item.IsChecked == true)
                {
                    nameCheckbox = item.Name.ToString();
                }
            }

            if (nameCheckbox != "" && isCheckedMajor != null)
            {
                var splitCheckboxName = nameCheckbox.Split("_");
                var major = splitCheckboxName[1];
                //await userControlChart.UpdateChartSlider(major, (int)e.NewValue);
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    SaveFileDialog saveDialog = new SaveFileDialog();
            //    saveDialog.Filter = "PNG Files (*.png)|*.png";

            //    if (saveDialog.ShowDialog() == true)
            //    {
            //        chart.Background = Brushes.White;
            //        chart.UpdateLayout();
            //        RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)chart.ActualWidth, (int)chart.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            //        renderBitmap.Render(chart);

            //        PngBitmapEncoder pngEncoder = new PngBitmapEncoder();

            //        pngEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            //        using (var stream = File.Create(saveDialog.FileName))
            //        {
            //            pngEncoder.Save(stream);
            //        }

            //        MessageBox.Show("Export Done!");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }
        private void ViewInformation_Click(object sender, RoutedEventArgs e)
        {
            //foreach (var child in listCheckBoxMajor)
            //{
            //    child.IsEnabled = false;
            //}
            //foreach (var child in checkboxYear.Items)
            //{
            //    if (child is CheckBox uiElement)
            //    {
            //        uiElement.IsEnabled = false;
            //    }
            //}
            //playBtn.Visibility = Visibility.Hidden;
            //slider.Visibility = Visibility.Hidden;
            //Button button = (Button)sender;
            //button.Click -= ViewInformation_Click;
            //button.Click += Back_Click;
            //button.Content = "Back";
            //if ("GPA Capstone".Equals(ToolName))
            //{
            //    if (gPACapstoneViewInformation == null)
            //    {
            //        gPACapstoneViewInformation = new GPACapstoneViewInformation();
            //    }
            //    userControlChart.Visibility = Visibility.Hidden;
            //    bool flag = false;
            //    foreach (var child in testcontrol.Children)
            //    {
            //        if (child is GPACapstoneViewInformation gpaControl)
            //        {
            //            if (gpaControl.Name == "gPACapstoneViewInformation")
            //            {
            //                gpaControl.Visibility = Visibility.Visible;
            //                flag = true;
            //                if (numberClick > 0)
            //                {
            //                    userControlChart.ViewInformation(gPACapstoneViewInformation);
            //                }
            //            }
            //            else
            //            {
            //                gpaControl.Visibility = Visibility.Hidden;
            //            }
            //        }
            //    }
            //    if (!flag)
            //    {
            //        gPACapstoneViewInformation.Name = "gPACapstoneViewInformation";
            //        gPACapstoneViewInformation.Height = 300;
            //        gPACapstoneViewInformation.Visibility = Visibility.Visible;
            //        testcontrol.Children.Add(gPACapstoneViewInformation);
            //        userControlChart.ViewInformation(gPACapstoneViewInformation);
            //        numberClick++;
            //    }
            //}
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            //foreach (var child in listCheckBoxMajor)
            //{
            //    child.IsEnabled = true;
            //}
            //foreach (var child in checkboxYear.Items)
            //{
            //    if (child is CheckBox uiElement)
            //    {
            //        uiElement.IsEnabled = true;
            //    }
            //}

            //playBtn.Visibility = Visibility.Visible;
            //slider.Visibility = Visibility.Visible;
            //Button button = (Button)sender;
            //button.Click += ViewInformation_Click;
            //button.Click -= Back_Click;
            //button.Content = "ViewInformation";
            //userControlChart.Visibility = Visibility.Visible;
            //foreach (var child in testcontrol.Children)
            //{
            //    if (child is GPACapstoneViewInformation gpaControl && gpaControl.Name == "gPACapstoneViewInformation")
            //    {
            //        gpaControl.Visibility = Visibility.Hidden;
            //        break;
            //    }
            //}
        }

        private void ExportList_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Coming Soon!");
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Coming Soon!");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

       
    }
}
