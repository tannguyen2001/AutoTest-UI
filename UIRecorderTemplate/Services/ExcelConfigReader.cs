using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationCLogic
{
    /// <summary>
    /// Lớp đọc bộ cấu hình từ file Excel
    /// </summary>
    public class ExcelConfigReader
    {
        private DataSet _excelConfigDataSet = new DataSet();

        public DataSet ExcelConfigDataSet => _excelConfigDataSet;

        private readonly string _excelConfigFilePath;

        public ExcelConfigReader(string excelConfigFilePath)
        {
            _excelConfigFilePath = excelConfigFilePath;
            this.ReadExcelFile();
        }

        private void ReadExcelFile()
        {
            using (var stream = File.Open(_excelConfigFilePath, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                _excelConfigDataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                reader.Close();

            }
        }
    }
}
