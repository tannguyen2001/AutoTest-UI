using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationCLogic.Stores
{
    public abstract class AbstractExcelLoader<T> where T : class
    {
        public readonly DataSet ExcelData;

        public T GenericData;

        protected AbstractExcelLoader(ExcelConfigReader configReader)
        {
            ExcelData = configReader.ExcelConfigDataSet;
            GenericData = LoadFromExcelToSpecialType();
        }

        public abstract T LoadFromExcelToSpecialType();
    }
}
