using BusinessLogic.IRepository;
using BusinessLogic.Repository;
using DataAccess.FileControl;
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SEP490_G17_Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRepositoryValidateFile _repositoryValidateFile = new RepositoryValidateFile();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string filePath = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    filePath = openFileDialog.FileName;

                    _repositoryValidateFile.CloseExcelProcesses(filePath);
                }
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                RepositoryValidateFile repositoryValidateFile = new RepositoryValidateFile();
                FileControl.Instance.InputValidationData = (Dictionary<string, object>)ValidateConfig.InputValidation[fileName];
                FileControl.Instance.InputValidationNum = (Dictionary<string, object>)ValidateConfig.InputValidationNumber[fileName];

                FileControl.CheckDataDelegate delegateExec = new FileControl.CheckDataDelegate(delegate (string[] values, string line)
                {
                    var dic = FileControl.Instance.CheckInputValidation(values, out bool isCheck, true);

                    if (!isCheck)
                    {
                        return null;
                    }
                    return dic;
                });

              
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("was not present in the dictionary"))
                {
                    MessageBox.Show("File Error: Invalid file name");
                    txtFilePath.Clear();
                    return;
                }
                MessageBox.Show("File Error: " + ex.Message);
                txtFilePath.Clear();
                return;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}