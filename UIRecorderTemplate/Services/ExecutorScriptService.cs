using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using AutomationCLogic.Models;
using AutomationCLogic.Repository;
using AutomationCLogic.Services;
using AutomationCLogic.Stores;
using System.Windows;
using System.IO;
using Microsoft.SqlServer.Server;
using Aquality.Selenium.Core.Elements;
using System.Threading.Tasks;
using OpenQA.Selenium.Interactions;
using System.Collections;
using System.Drawing;
using System.ComponentModel;

namespace AutomationCLogic
{
    public interface IScriptService
    {
        void Run(ref int testStepNumber, BackgroundWorker worker);
    }

    /// <summary>
    /// Lớp chạy các test case
    /// </summary>
    public class ExecutorScriptService : IScriptService
    {
        private readonly DesktopSession _desktopSession;
        private readonly ResourceStore _resourceStore;
        private readonly TestCaseStore _testCaseStore;
        private readonly DapperContext _dapperContext;

        private WindowsElement _controlFocus = null;

        public ExecutorScriptService(ResourceStore resourceStore, TestCaseStore testCaseStore, DesktopSession desktopSession, DapperContext dapperContext)
        {
            _resourceStore = resourceStore;
            _testCaseStore = testCaseStore;
            _dapperContext = dapperContext;
            _desktopSession = desktopSession;
        }

        public void ReturnToMenu()
        {
            var messageBox = _desktopSession.FindElementByAbsoluteXPath("/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"infoplus ERP \"][@AutomationId=\"MainForm\"]/Window[@Name=\":: infoplus ERP :: Hộp tin nhắn\"][@AutomationId=\"MessageBoxNormal\"]");
            var menu = _desktopSession.FindElementByAbsoluteXPath("/Pane[@ClassName=\"#32769\"][@Name=\"Desktop 1\"]/Window[@Name=\"infoplus ERP \"][@AutomationId=\"MainForm\"]/Pane[@AutomationId=\"RuntimeMainBoard\"]/Pane[starts-with(@ClassName,\"WindowsForms10\")]/Pane[starts-with(@ClassName,\"WindowsForms10\")]/Pane[starts-with(@ClassName,\"WindowsForms10\")]/Pane[@AutomationId=\"_SptContainer\"]/Pane[starts-with(@ClassName,\"WindowsForms10\")]/Pane[starts-with(@ClassName,\"WindowsForms10\")]/Pane[starts-with(@ClassName,\"WindowsForms10\")]");
            if (messageBox != null)
            {
                messageBox.SendKeys(Keys.Escape);
            }
            menu.SendKeys(Keys.Escape);
            menu.SendKeys(Keys.Escape);
            menu.SendKeys(Keys.Escape);
        }
        public void Run(ref int testStepNumber, BackgroundWorker worker)
        {
            foreach (var testCase in _testCaseStore.TestCases)
            {
                int count = 0;
                foreach (var testStep in testCase)
                {
                    ExecuteTestStep(testStep);
                    testStepNumber++;
                    count++;
                    worker.ReportProgress(testStepNumber);
                    if (!testStep.IsPassed)
                    {
                        testStepNumber += (testCase.Count - count);
                        worker.ReportProgress(testStepNumber);
                        break;
                    }
                }
                
                ReturnToMenu();
            }
            ExportTestCaseResult();
        }

        private void CopyTextToClipboard(string text)
        {
            Thread thread = new Thread(() =>
            {
                Clipboard.SetText(text);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void ExportTestCaseResult()
        {
            Console.WriteLine("Export to excel...");

            StringBuilder sb = new StringBuilder();

            foreach (var testCase in _testCaseStore.TestCases)
            {
                sb.AppendLine($"----------------------------");
                string testCaseResult = testCase.Count(s => !s.IsPassed) > 0? "FAIL" : "PASS";
                sb.AppendLine($"Test Case {testCase.Name} : {testCaseResult}");
                foreach (var step in testCase.OrderBy(s => s.Step))
                {
                    string testStepResult = step.IsPassed ? "PASS" : "FAIL";
                    sb.AppendLine($"Step {step.Step} : {testStepResult}");
                }
            }

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            string pathOutput = "";
            using (ExcelPackage excelPackage = new ExcelPackage("./TestCaseTemplate.xlsx"))
            {
                ExcelWorksheet excelWorkSheet = excelPackage.Workbook.Worksheets.First(); // lấy sheet1
                // TestCase test1 = listTestCase[0];
                int count = 0; // đếm xem đã dùng được bao nhiêu dòng
                for (int countTest = 0; countTest < _testCaseStore.TestCases.Count; countTest++) // lặp để lấy data
                {
                    excelWorkSheet.Cells[count + 19, 1].Value = _testCaseStore.TestCases[countTest].Name; // set name test case
                    excelWorkSheet.Cells[count + 19, 2].Value = _testCaseStore.TestCases[countTest].Description; // set mô tả testcase
                    int countStep; // số step
                    int countStepPass = 0;
                    for (countStep = 0; countStep < _testCaseStore.TestCases[countTest].Count; countStep++) // lặp để lấy data
                    {
                        excelWorkSheet.Cells[countStep + count + 19, 3].Value = _testCaseStore.TestCases[countTest][countStep].Step + ". " + _testCaseStore.TestCases[countTest][countStep].Description;
                        excelWorkSheet.Cells[countStep + count + 19, 4].Value = _testCaseStore.TestCases[countTest][countStep].IsPassed? "PASS":"FAIL";
                        if (_testCaseStore.TestCases[countTest][countStep].IsPassed)
                        {
                            countStepPass++;
                        }
                    }
                    excelWorkSheet.Cells[countStep + count + 18, 7].Value = countStepPass==countStep ? "Pass" : "Fail";
                    string mergeCellsName = String.Format("A{0}:A{1}", count + 19, countStep + count + 18); // table cần merge
                    string mergeCellsDescription = String.Format("B{0}:B{1}", count + 19, countStep + count + 18); // table cần merge
                    excelWorkSheet.Cells[mergeCellsName].Merge = true; // merge cells
                    excelWorkSheet.Cells[mergeCellsDescription].Merge = true; // merge cells
                    count += countStep; // set dòng khi hết vòng lặp

                }

                pathOutput = AppDomain.CurrentDomain.BaseDirectory + "Configurations\\Output\\" + DateTime.Now.ToString("ddMMyyyyHHss") + ".xlsx";
                Stream streamOutput = File.Create(pathOutput);
                excelPackage.SaveAs(streamOutput);
                streamOutput.Close();
            }

            Console.WriteLine(sb.ToString());
        }

        private void ExecuteTestStep(TestStep testStep)
        {
            WindowsElement control = null;
            IList<WindowsElement> listControl = null;
            string inputSource = _resourceStore.ReplaceResourceValue(testStep.InputParameter.Source.ToString()).ToString();
            string expectedSource = _resourceStore.ReplaceResourceValue(testStep.ExpectedParameter.Source.ToString()).ToString();
            if (!string.IsNullOrEmpty(inputSource))
            {
                if (testStep.InputParameter.SourceType == Enums.InputSourceType.XPATH)
                {
                    var xpath = inputSource.Replace("\\", string.Empty).Trim().Trim('\"');
                    control = _desktopSession.FindElementByAbsoluteXPath(xpath);
                }

                else if (testStep.InputParameter.SourceType == Enums.InputSourceType.ID)
                {
                    control = _desktopSession.FindElementById(inputSource);
                }

                if (control == null)
                {
                    testStep.ActualValue = null;
                    testStep.IsPassed = false;
                    testStep.Note = "Cant find element!";
                    return;
                }
            }

            if (testStep.InputParameter.ActionType == Enums.ActionType.CLICK)
            {
                control.Click();
                // expected
                if (testStep.ExpectedParameter.ActionType == Enums.ActionType.SELF_CHECK)
                {
                    if (testStep.ExpectedParameter.ExpectedSourceParam1 == Enums.ExpectedSourceParamType.PROPERTY)
                    {
                        if (testStep.ExpectedParameter.ExpectedSourceParam2 == Enums.PropertyType.Enable)
                        {
                            var controlEnable = control.Enabled;

                            testStep.ActualValue = controlEnable;
                        }
                    }
                }
                else if (testStep.ExpectedParameter.ActionType == Enums.ActionType.WAIT_UNTIL)
                {
                    if (testStep.ExpectedParameter.ExpectedSourceParam1 == Enums.ExpectedSourceParamType.PROPERTY)
                    {
                        if (testStep.ExpectedParameter.ExpectedSourceParam2 == Enums.PropertyType.TEXT)
                        {
                            var xpath = testStep.ExpectedParameter.Source.ToString().Replace("\\", string.Empty).Trim().Trim('\"');
                            var control2 = _desktopSession.FindElementByAbsoluteXPath(xpath);
                            if(control2 != null)
                            {
                                control2.Click();
                                control2.SendKeys(Keys.Enter);
                                testStep.IsPassed = true;
                            }
                            else if(control2.Text != testStep.InputParameter.Value.ToString())
                            {
                                testStep.IsPassed = false;
                            }
                        }
                        else if(testStep.ExpectedParameter.ExpectedSourceParam2 == Enums.PropertyType.Enable)
                        {
                            var control2 = _desktopSession.FindElementByAbsoluteXPath(testStep.ExpectedParameter.Source.ToString().Replace("\\", string.Empty).Trim().Trim('\"'));
                            if(control2 == null)
                            {
                                testStep.IsPassed = false;
                            }
                        }
                    }
                }
                else
                {
                    testStep.IsPassed = true;
                }
            }
            else if (testStep.InputParameter.ActionType == Enums.ActionType.CLICK_INDEX)
            {
                var xpath = inputSource.Replace("\\", string.Empty).Trim().Trim('\"');
                listControl = _desktopSession.FindElementsByAbsoluteXPath(xpath);
                int index = int.Parse(testStep.InputParameter.Value.ToString());
                control = listControl[index - 1];
                _controlFocus = listControl[index - 1];
                control.Click();
                testStep.IsPassed = true;
            }
            else if (testStep.InputParameter.ActionType == Enums.ActionType.DELETE)
            {
                control = _controlFocus;
                control.SendKeys(Keys.Control + Keys.Delete);
                testStep.IsPassed = true;
            }
            else if (testStep.InputParameter.ActionType == Enums.ActionType.PASTE)
            {
                string inputValue = testStep.InputParameter.Value.ToString().Replace("\\t", "\t");
                CopyTextToClipboard(inputValue);
                control = _controlFocus;
                control.SendKeys(Keys.Control + "v");
                testStep.IsPassed = true;
            }
            else if (testStep.InputParameter.ActionType == Enums.ActionType.SEND_KEYS)
            {
                // input
                string inputValue = testStep.InputParameter.Value.ToString();
                if (inputValue.StartsWith("Keys."))
                {
                    string keySend = string.Empty;

                    string keyInput = testStep.InputParameter.Value.ToString().Split('.')[1];

                    switch (keyInput)
                    {
                        case nameof(Keys.Escape):
                            keySend = Keys.Escape;
                            break;
                        case nameof(Keys.Enter):
                            keySend = Keys.Enter;
                            break;
                    }

                    if (!string.IsNullOrEmpty(keySend))
                    {
                        control.SendKeys(keySend);
                    }
                }
                else if (inputValue.StartsWith("$"))
                {
                    inputValue = inputValue.ToLower();
                    inputValue = inputValue.Replace(" ","");
                    inputValue = inputValue.Replace("$", "");
                    if (inputValue.Contains("random("))
                    {
                        inputValue = inputValue.Replace("random(","");
                        if (inputValue.Contains("int,"))
                        {
                            inputValue = inputValue.Replace("int,", "");
                            string numberString = "";
                            Random random = new Random();
                            foreach (Char c in inputValue)
                            {
                                if (Char.IsDigit(c))
                                {
                                    numberString += c;
                                }
                            }
                            int numberInt = int.Parse(numberString);
                            string inputResult = "";
                            for(int i = 0; i < numberInt; i++)
                            {
                                inputResult += random.Next(9);
                            }
                            control.SendKeys(inputResult);
                        }
                        else if(inputValue.Contains("string,"))
                        {
                            inputValue = inputValue.Replace("string", "");
                            string numberString = "";
                            Random random = new Random();
                            foreach (Char c in inputValue)
                            {
                                if (Char.IsDigit(c))
                                {
                                    numberString += c;
                                }
                            }
                            int numberInt = int.Parse(numberString);
                            string inputResult = "";
                            const string TEXT = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                            for (int i = 0; i < numberInt; i++)
                            {
                                inputResult += TEXT[random.Next(TEXT.Length - 1)];
                            }
                            control.SendKeys(inputResult);
                        }
                        else if (inputValue.Contains("decimal,")){
                            inputValue = inputValue.Replace("decimal,","");
                            var listString = inputValue.Split(',');
                            string numberString1 = "";
                            foreach(char c in listString[0])
                            {
                                if (Char.IsDigit(c))
                                {
                                    numberString1 += c;
                                }
                            }
                            string numberString2 = "";
                            foreach (char c in listString[1])
                            {
                                if (Char.IsDigit(c))
                                {
                                    numberString2 += c;
                                }
                            }
                            for(int i = 0; i < int.Parse(numberString2); i++)
                            {
                                
                            }
                            string inputString = $"{numberString1},{numberString2}";
                            control.SendKeys(inputString);
                        }
                    }
                }
                else
                {
                    control.Click();
                    control.SendKeys(inputValue);
                }

                testStep.IsPassed = true;
            }
            else if (testStep.InputParameter.ActionType == Enums.ActionType.CTRL_INSERT)
            {
                control.Click();
                control.SendKeys(Keys.Control + Keys.Insert);
                testStep.IsPassed = true;
            }
            else if (testStep.ExpectedParameter.ActionType == Enums.ActionType.QUERY_SQL)
            {
                // check sql
                string sql = _resourceStore.ReplaceResourceValue(expectedSource).ToString();
                var dtResult = _dapperContext.GetDataTableBySqlQuery(sql);
                if (dtResult != null)
                {
                    testStep.ActualValue = dtResult;

                    var dtExpected = _resourceStore.ReplaceResourceValue(testStep.ExpectedParameter.Value.ToString()
                        .Replace("^", string.Empty).Replace("#", string.Empty)) as DataTable;

                    // Kiểm tra số lượng dòng
                    testStep.IsPassed = dtResult.Rows.Count == dtExpected.Rows.Count;
                    // Kiểm tra nội dung
                    if (testStep.IsPassed)
                    {
                        for (int i = 0; i < dtExpected.Rows.Count; i++)
                        {
                            foreach(DataColumn dc in dtExpected.Columns)
                            {
                                // Nếu không bao gồm cột
                                if (!dtResult.Columns.Contains(dc.ColumnName))
                                {
                                    testStep.IsPassed = false;
                                    return;
                                }

                                if (dtResult.Rows[i][dc.ColumnName].ToString() != dtExpected.Rows[i][dc.ColumnName].ToString())
                                {
                                    testStep.IsPassed = false;
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        testStep.IsPassed = false;
                        return;
                    }
                }
            }
        }
    }
}
