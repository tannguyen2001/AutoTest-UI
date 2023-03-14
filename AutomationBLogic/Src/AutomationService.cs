using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationCLogic.AutomationLogic;

namespace AutomationBService.Src
{
    public class AutomationServices : IAutomationService
    {

        public object dataProgressBar = new Object();

        public async Task ChangePathExcel(string path)
        {
            AutomationLogic.GetInstance().ChangePathExcelFile(path);
        }

        public async Task<Tuple<int,int>> GetDataProgressBar()
        {
            return new Tuple<int, int>(AutomationLogic.GetInstance().minStepProgress, AutomationLogic.GetInstance().maxStepProgress);
        }

        public int GetStepNumberValue()
        {
            return AutomationLogic.GetInstance().stepProgressing;
        }

        public async Task SetupAutoTest()
        {
            AutomationLogic.GetInstance().SetupAutoTest();
            dataProgressBar = await this.GetDataProgressBar();
        }

        public void StartAutomationTest(ref int testStepNumber, BackgroundWorker worker)
        {
            AutomationLogic.GetInstance().StartAutoTest(ref testStepNumber,  worker);
          
        }




    }
}
