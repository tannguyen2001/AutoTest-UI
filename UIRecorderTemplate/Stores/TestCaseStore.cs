using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationCLogic.Enums;
using AutomationCLogic.Extensions;
using AutomationCLogic.Models;

namespace AutomationCLogic.Stores
{
    public class TestCaseStore : AbstractExcelLoader<List<TestCase>>
    {
        private int stepCount = 0;
        public TestCaseStore(ExcelConfigReader configReader) : base(configReader)
        {

        }

        public List<TestCase> TestCases => GenericData;

        public override List<TestCase> LoadFromExcelToSpecialType()
        {
            GenericData = new List<TestCase>();

            foreach (DataTable dt in ExcelData.Tables)
            {
                if (dt.TableName.StartsWith("TC_"))
                {
                    string testCaseName = dt.TableName;
                    string testCaseDescription = dt.Rows[0][1].ToString();

                    var testCase = new TestCase
                    {
                        Name = testCaseName,
                        Description = testCaseDescription
                    };


                    for (int i = 6; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];

                        string step = dr["Step"].ToString();

                        if (string.IsNullOrEmpty(step))
                        {
                            continue;
                        }

                        var testStep = new TestStep
                        {
                            Step = int.Parse(step),
                            Description = dr["Description"].ToString(),
                            InputParameter = new InputParameter
                            {
                                ActionType = EnumExtensions.ParseEnum<ActionType>(dr["InputActionType"].ToString()),
                                SourceType = EnumExtensions.ParseEnum<InputSourceType>(dr["InputSourceType"].ToString()),
                                Source = dr["InputSource"].ToString(),
                                Value = dr["InputValue"].ToString()
                            },
                            ExpectedParameter = new ExpectedParameter
                            {
                                ActionType = EnumExtensions.ParseEnum<ActionType>(dr["ExpectedActionType"].ToString()),
                                Source = dr["ExpectedSource"].ToString(),
                                ExpectedSourceParam1 = EnumExtensions.ParseEnum<ExpectedSourceParamType>(dr["ExpectedSourceParam1"].ToString()),
                                ExpectedSourceParam2 = EnumExtensions.ParseEnum<PropertyType>(dr["ExpectedSourceParam2"].ToString()),
                                Value = dr["ExpectedValue"].ToString()
                            }
                        };
                        testCase.Add(testStep);
                        this.stepCount++;
                    }

                    GenericData.Add(testCase);
                }
            }

            return GenericData;
        }
        public int GetStepCountFromExcel()
        {
            return this.stepCount;
        }
    }
}
