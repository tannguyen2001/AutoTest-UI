using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationBService.Src
{
    public interface IAutomationService
    {
        void SetupAutoTest();
        void StartAutomationTest(ref int testStepNumber, BackgroundWorker worker);
        void ChangePathExcel(string path);
        Tuple<int, int> GetDataProgressBar();
        int GetStepNumberValue();
    }
}
