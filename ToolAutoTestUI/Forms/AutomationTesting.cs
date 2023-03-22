using Infragistics.Win;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutomationBService.Src;
using ToolAutoTestUI.Resource;

namespace AutomationAUI
{
    public partial class frmAutomationTesting : Form
    {

        private string[] listPathExcelTest;
        AutomationServices automationServices = new AutomationServices();


        public frmAutomationTesting()
        {
            automationServices = new AutomationServices();
            InitializeComponent();
        }
        

        private void btnChooseExcel_Click(object sender, EventArgs e)
        {

            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = true;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                listPathExcelTest = choofdlog.FileNames;
                //string[] arrAllFiles = choofdlog.FileNames; //used when Multiselect = true

                if (choofdlog.SafeFileNames.ToList().Count == 1)
                {
                    lblMessage.Text = choofdlog.SafeFileNames[0];
                }
                else
                {
                    string names = "";
                    foreach (var item in choofdlog.SafeFileNames)
                    {
                        names += (item + "\r\n");
                    }
                    lblMessage.Text = names;
                }

                lblMessage.Appearance.ForeColor = Color.DarkGreen;
            }


        }


        private void btnStartTesting_Click(object sender, EventArgs e)
        {
            if (listPathExcelTest != null && listPathExcelTest.ToList().Count > 0)
            {
                foreach (var pathItem in listPathExcelTest.ToList())
                {
                    automationServices.ChangePathExcel(pathItem);
                    automationServices.SetupAutoTest();
                    var dataProgress = automationServices.GetDataProgressBar();
                    progressBarTest.Value = 0;
                    int testStepNumber = 0;
                    progressBarTest.Minimum = dataProgress.Item1;
                    progressBarTest.Maximum = dataProgress.Item2;
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += (senderr, ee) =>
                    {
                        automationServices.StartAutomationTest(ref testStepNumber, worker); // Truyền đối tượng BackgroundWorker vào hàm StartAutomationTest
                        if (pathItem == listPathExcelTest[listPathExcelTest.Length - 1])
                        {
                            MessageBox.Show(TextMessage.AUTO_TEST_SUCCESS, TextMessage.NOTIFICATION, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2, MessageBoxOptions.ServiceNotification);
                        }
                    };
                    worker.ProgressChanged += (senderr, ee) =>
                    {
                        this.Invoke(new Action(() =>
                        {
                            progressBarTest.Value = ee.ProgressPercentage;
                        }));
                    };
                    worker.RunWorkerAsync();
                }
            }
            else
            {
                MessageBox.Show(TextMessage.NOT_EXIT_FILE, TextMessage.NOTIFICATION, MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Custom Description";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string sSelectedPath = fbd.SelectedPath;
                if (string.IsNullOrEmpty(sSelectedPath))
                {
                    MessageBox.Show(TextMessage.SAVE_FOLDER_NOT_EXITS, TextMessage.NOTIFICATION, MessageBoxButtons.OK);
                }
            }
        }
    }
}
