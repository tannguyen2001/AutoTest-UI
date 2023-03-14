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
        Task SetupAutoTest();
        void StartAutomationTest(ref int testStepNumber, BackgroundWorker worker);
        Task ChangePathExcel(string path);
        Task<Tuple<int, int>> GetDataProgressBar();
        int GetStepNumberValue();
    }
}
