using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutomationCLogic.Stores;

namespace AutomationCLogic
{
    public class ResourceStore : AbstractExcelLoader<Hashtable>
    {
        public ResourceStore(ExcelConfigReader configReader) : base(configReader)
        {
            
        }
        public override Hashtable LoadFromExcelToSpecialType()
        {
            GenericData = new Hashtable();

            // Chuyển đổi tab VARIABLES
            var dtVariables = ExcelData.Tables["VARIABLES"];
            foreach (DataRow dr in dtVariables.Rows)
            {
                GenericData.Add(dr["KEY"], dr["VALUE"]);
            }

            // Chuyển đổi tab RESOURCES
            var dtResources = ExcelData.Tables["RESOURCES"];

            var groupsTable = dtResources.AsEnumerable()
                .GroupBy(r => new { KEY = r["KEY"], TYPE = r["TYPE"] });

            foreach (var group in groupsTable)
            {
                string key = group.Key.KEY.ToString();
                string type = group.Key.TYPE.ToString();

                // Đọc định dạng bảng
                if (type.StartsWith("TABLE"))
                {
                    DataTable dt = new DataTable();
                    dt.TableName = key;

                    var resource = dtResources.AsEnumerable()
                        .Where(r => r["KEY"] == group.Key.KEY
                        && r["TYPE"] == group.Key.TYPE)
                        .OrderBy(r => int.Parse(r["SORT"].ToString()));

                    int columnCount = int.Parse(type.Split('-')[1]);

                    // Tạo row header
                    DataRow rowsHeader = group.FirstOrDefault();
                    for (int i = 0; i < columnCount; i++)
                    {
                        string columName = rowsHeader[i+3].ToString();
                        dt.Columns.Add(columName);
                    }

                    // Tạo dữ liệu theo dòng
                    foreach (DataRow dr in group)
                    {
                        if (group.FirstOrDefault() == dr)
                        {
                            continue;
                        }

                        DataRow newRow = dt.NewRow();

                        for (int i = 0; i < columnCount; i++)
                        {
                            int index = i + 3;
                            string value = dr[index].ToString();

                            newRow[i] = value;
                        }

                        dt.Rows.Add(newRow);
                    }

                    GenericData.Add(key, dt);

                }

                // Đọc định dạng truy vấn sql
                else if (type.Equals("SQL"))
                {
                    // Cột số 3 trong bảng cấu hình
                    string value = group.FirstOrDefault()[3].ToString();
                    GenericData.Add(key, value);
                }
            }

            return GenericData;
        }
        public object GetResourceValue(object resource)
        {
            if (GenericData.ContainsKey(resource))
            {
                resource = GenericData[resource];
            }

            return resource;
        }

        public object ReplaceResourceValue(object resource)
        {
            if (resource.GetType() == typeof(string))
            {
                if (GenericData.ContainsKey(resource))
                {
                    resource = GenericData[resource];
                }
                // Kiểm tra xem có liên kết resource nữa không
                string pattern = @"(?<=\^)(.*?)(?=\#)";
                Regex regex = new Regex(pattern);
                var groups = regex.Matches(resource.ToString());
                foreach (Match match in groups)
                {
                    string value = ReplaceResourceValue(match.Value).ToString();
                    resource = resource.ToString().Replace(string.Format("^{0}#", match), value);
                }
            }

            return resource;
        }
    }
}
