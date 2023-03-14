using AutomationCLogic.Repository;
using AutomationCLogic.Services;
using AutomationCLogic.Stores;
using AutomationCLogic;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AutomationCLogic.Models;
using System.Windows.Forms;
using System.ComponentModel;

namespace AutomationCLogic.AutomationLogic
{
    public class AutomationLogic
    {

        private static volatile AutomationLogic instance; //instance 
        private static readonly object lockObject = new object();
        ServiceProvider _serviceProvider;
        public ServiceProvider ServiceProvider => _serviceProvider;

        private string pathExcelFile = "";
        public int minStepProgress { set; get; }
        public int maxStepProgress { set; get; }
        public int stepProgressing { set; get; }

        private AutomationLogic() // để la private để không thể khởi tạo từ bên ngoài
        {
            
            
        }


        public static AutomationLogic GetInstance() //lấy ra instance
        {
            lock (lockObject) // khoá lại để chạy lần lượt
            {
                if (instance == null) // check thêm 1 lần nữa
                {
                    instance = new AutomationLogic();
                }
                return instance;
            }
        }

        private void SetValueProgressBar()
        {
            var testCaseStore = _serviceProvider.GetService<TestCaseStore>();
            minStepProgress = 0;
            stepProgressing = 0;
            maxStepProgress = testCaseStore.GetStepCountFromExcel();
        }

        public void ChangePathExcelFile(string pathExcelFile)
        {
            this.pathExcelFile = pathExcelFile;
        }

        public void SetupAutoTest()
        {
            this.StartUp();
            this.SetValueProgressBar();
        }

        public void StartAutoTest(ref int testStepNumber, BackgroundWorker worker)
        {
            var scriptService = _serviceProvider.GetService<ExecutorScriptService>();
            scriptService.Run(ref testStepNumber,worker);
            Console.ReadLine();
        }


        private void StartUp()
        {
            ServiceCollection serviceProvider = new ServiceCollection();
            serviceProvider.AddSingleton<ServiceProvider>(s => _serviceProvider);
            serviceProvider.AddTransient<DapperContext>();
            serviceProvider.AddSingleton<DesktopSession>();
            serviceProvider.AddSingleton<ExecutorScriptService>();
            serviceProvider.AddSingleton<ExcelConfigReader>(s =>
            {
                return new ExcelConfigReader(pathExcelFile);
            });
            serviceProvider.AddSingleton<TestCaseStore>();
            serviceProvider.AddSingleton<ResourceStore>();
            serviceProvider.AddSingleton<IConfiguration>(s => new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

            _serviceProvider = serviceProvider.BuildServiceProvider();
        }

        public void ChangePathSaveExport(string sSelectedPath)
        {

        }
    }
}
