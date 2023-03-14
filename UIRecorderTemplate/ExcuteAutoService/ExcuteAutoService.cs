using AutomationCLogic.Models;
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

namespace UIRecorderTemplate.ExcuteAutoService
{
    public class ExcuteAutoService
    {
        static ServiceProvider _serviceProvider;
        public ServiceProvider ServiceProvider => _serviceProvider;
        public List<TestCase> GetTestStep()
        {
            var listTestCase = _serviceProvider.GetService<TestCaseStore>().GenericData;
            return listTestCase;
        }

        public void StartUp()
        {
            ServiceCollection serviceProvider = new ServiceCollection();

            serviceProvider.AddSingleton<ServiceProvider>(s => _serviceProvider);

            serviceProvider.AddTransient<DapperContext>();
            serviceProvider.AddSingleton<DesktopSession>();
            serviceProvider.AddSingleton<ExecutorScriptService>();
            serviceProvider.AddSingleton<ExcelConfigReader>(s =>
            {
                return new ExcelConfigReader("AutoTestCaseTemplate.xlsx");
            });
            serviceProvider.AddSingleton<TestCaseStore>();
            serviceProvider.AddSingleton<ResourceStore>();
            serviceProvider.AddSingleton<IConfiguration>(s => new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

            _serviceProvider = serviceProvider.BuildServiceProvider();
        }
    }
}
