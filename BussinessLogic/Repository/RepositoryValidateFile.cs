using BusinessLogic.IRepository;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{

    public class RepositoryValidateFile : IRepositoryValidateFile
    {
        void IRepositoryValidateFile.CloseExcelProcesses(string filePath)
        {
            try
            {
                var fileName = Path.GetFileName(filePath);
                var test = System.Diagnostics.Process.GetProcessesByName("EXCEL");
                foreach (var process in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                {
                    if (process.MainWindowTitle.Contains(fileName))
                    {
                        process.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error closing Excel processes: " + ex.Message);
            }
        }
    }
}
