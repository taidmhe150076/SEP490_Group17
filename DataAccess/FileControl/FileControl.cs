using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.VisualBasic.FileIO;
using System.Text;
using System.Collections.Generic;

namespace DataAccess.FileControl
{
    public class FileControl
    {
        private static FileControl instance;
        public Dictionary<string, object> InputValidationData { get; set; }

        public Dictionary<string, object> InputValidationNum { get; set; }
        public string FileName { get; set; }
        public string ErrorKey { get; set; }
        public string ErrorValue { get; set; }
        public string ErrorColum { get; set; }

        public delegate Dictionary<string, object> CheckDataDelegate(string[] values, string line);

        private static readonly object lockObject = new object();

        private List<GradesGPA> objectList;

        private FileControl()
        {
            this.objectList = new List<GradesGPA>();
        }

        public static FileControl Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new FileControl();
                        }
                    }
                }
                return instance;
            }
        }
        public async Task<List<GradesGPA>> LoadDataFromFile(string filePath, CheckDataDelegate delegateExec)
        {
            try
            {
                var result = await ConvertAllCsvToJson(filePath, delegateExec);
                var csvResult = (List<Dictionary<string, object>>)result["csvResultObjectList"];

                objectList = csvResult.Select(csvResult =>
                    new GradesGPA
                    {
                        Group = csvResult["Group"].ToString(),
                        GroupMajor = csvResult["GroupMajor"].ToString(),
                        GroupMember = csvResult["GroupMember"].ToString(),
                        MemberMajor = csvResult["MemberMajor"].ToString(),
                        Gpa = Convert.ToDouble(csvResult["Gpa"]),
                        Semester_no = Convert.ToInt32(csvResult["Semester_no"]),
                        Semester = csvResult["Semester"].ToString(),
                        Is_Capstone = Convert.ToBoolean(csvResult["Is_Capstone"]),
                        SemesterCredit = Convert.ToInt32(csvResult["SemesterCredit"]),
                        Year = Convert.ToInt32(csvResult["Year"])
                    }
                ).ToList();
                return objectList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<GradesGPA> ReadDataAndConvertToList(string filePath)
        {
            List<GradesGPA> records;
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    records = csv.GetRecords<GradesGPA>().ToList();
                }
                return records; 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GradesGPA> GetObjectList()
        {
            return objectList;
        }
        public async Task<Dictionary<string, object>> ConvertAllCsvToJson(string csvFilePath, CheckDataDelegate delegateExec = null)
        {
            Dictionary<string, object> returnObj = new Dictionary<string, object>();

            var csvDictionaryList = new List<Dictionary<string, object>>();
            var totalRecords = 0;
            var errorRecords = 0;

            try
            {
                var readAll = await File.ReadAllLinesAsync(csvFilePath).ConfigureAwait(false);
                int maxLines = readAll.Length;
                totalRecords = maxLines;
                int lines = 0;

                var fileName = Path.GetFileName(csvFilePath);

                this.FileName = fileName;

                if (maxLines == 1)
                {
                    throw new Exception($"The line number of the file does not satisfy");
                }

                for (int i = 0; i < maxLines; ++i)
                {
                    using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(readAll[i])))
                    using (TextFieldParser textFieldParser = new TextFieldParser(stream, Encoding.UTF8))
                    {
                        textFieldParser.TextFieldType = FieldType.Delimited;
                        textFieldParser.SetDelimiters(",");

                        while (textFieldParser.EndOfData == false)
                        {
                            var values = textFieldParser.ReadFields();
                            if (i == 0)
                            {
                                // check header có tồn tại không ?
                                foreach (var value in values)
                                {
                                    if (!InputValidationData.ContainsKey(value.ToString()))
                                    {
                                        throw new Exception($"Header Invalid : This column is not in the header ( {value} )");
                                    }
                                }

                                // check thứ tự có đúng trong ValidateConfig không ?
                                for (int j = 0; j < values.Length; j++)
                                {
                                    if (!InputValidationData.ElementAt(j).Key.Equals(values[j]))
                                    {
                                        throw new Exception($"Header Invalid:The column is not at the correct index position {values[j].ToString()}");
                                    }
                                }
                            }
                            else
                            {
                                // check các Elements của body
                                if (values.Length == InputValidationData.Count())
                                {
                                    if (delegateExec != null)
                                    {
                                        var target = delegateExec(values, i.ToString());
                                        if (target != null)
                                        {
                                            csvDictionaryList.Add(target);
                                            totalRecords++;
                                        }
                                        else
                                        {
                                            throw new Exception($"Body Record Invalid:FileName: {fileName},Line: {i + 1}, Error column:{this.ErrorColum}, Error Type: {this.ErrorKey}, Error Value: {this.ErrorValue}");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception($"File Error !!!");
                                    }
                                }
                                else
                                {
                                    throw new Exception($"Body Invalid: Elements Count is Inappropriate,Line:{i}");
                                }
                            }
                        }
                    }
                }

                // set value
                returnObj.Add("totalRecords", totalRecords);
                returnObj.Add("errorRecords", errorRecords);
                returnObj.Add("csvResultObjectList", csvDictionaryList);

                return returnObj;
            }
            catch (Exception e) when (e is ArgumentException || e is MalformedLineException || e is FormatException || e is Exception)
            {
                throw;
            }

            return null;
        }

        public Dictionary<string, object> CheckInputValidation(string[] values, out bool isCheck, bool isCreat = false)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            Dictionary<string, object> dic = null;
            if (isCreat)
            {
                dic = new Dictionary<string, object>();
            }

            int index = 0;
            isCheck = true;
            this.ErrorKey = null;
            foreach (var value in values)
            {
                if (!this.InputValidationNum.ContainsKey(index.ToString()))
                {
                    continue;
                }

                var keyName = this.InputValidationNum[index.ToString()].ToString();
                if (!this.InputValidationData.ContainsKey(keyName))
                {
                    continue;
                }

                foreach (var d in (Dictionary<string, object>)this.InputValidationData[keyName])
                {
                    if (d.Key.Equals("required", StringComparison.Ordinal) && !string.IsNullOrEmpty(value))
                    {
                    }
                    else if (d.Key.Equals("maxLength", StringComparison.Ordinal) && value != null && value.Length <= Convert.ToInt32(d.Value))
                    {
                    }
                    else if (d.Key.Equals("minLength", StringComparison.Ordinal) && value != null && value.Length >= Convert.ToInt32(d.Value))
                    {
                    }
                    else if (d.Key.Equals("format", StringComparison.Ordinal))
                    {
                        if (d.Value.Equals("yyyy") && (value.Length != 4 || !DateTime.TryParseExact(value, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue)))
                        {
                            this.ErrorKey = d.Key;
                            this.ErrorValue = value;
                            isCheck = false;
                            break;
                        }
                    }
                    else if (d.Key.Equals("pattern", StringComparison.Ordinal))
                    {
                        if (d.Value.Equals("TRUE|FALSE") && (value != "TRUE" && value != "FALSE"))
                        {
                            this.ErrorKey = d.Key;
                            this.ErrorValue = value;
                            isCheck = false;
                            break;
                        }
                    }
                    else if (d.Key.Equals("GPAValueOutOfRange", StringComparison.Ordinal))
                    {
                        if (double.TryParse(value, out double gpa))
                        {
                            if (gpa >= 0 || gpa <= 10)
                            {
                                int decimalPlaces = value.Split('.').Last().Length;
                                if (decimalPlaces <= 2)
                                {
                                    // giá trị ok không check nữa 
                                }
                                else
                                {
                                    this.ErrorKey = "Only take 2 numbers after the \".\" ";
                                    this.ErrorValue = value;
                                    isCheck = false;
                                    break;
                                }
                            }
                            else
                            {
                                this.ErrorValue = value;
                                this.ErrorKey = d.Key;
                                isCheck = false;
                                break;
                            }
                        }
                        else
                        {
                            this.ErrorValue = value;
                            this.ErrorKey = d.Key;
                            isCheck = false;
                            break;
                        }
                    }
                    else
                    {
                        this.ErrorValue = value;
                        this.ErrorKey = d.Key;
                        isCheck = false;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(this.ErrorKey))
                {
                    this.ErrorColum = keyName;
                    break;
                }

                if (isCreat)
                {
                    dic.Add(keyName, values[index]);
                }

                ++index;
            }
            return dic;
        }
    }
}
