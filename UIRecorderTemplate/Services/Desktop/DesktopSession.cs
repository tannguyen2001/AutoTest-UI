using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using AutomationCLogic;
using AutomationCLogic.Stores;
using OpenQA.Selenium.Appium;
using NLog.LayoutRenderers;
using Microsoft.Extensions.Options;
using System.IO;
using OpenQA.Selenium.Interactions;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace AutomationCLogic.Services
{
    public class DesktopSession
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723/";
        WindowsDriver<WindowsElement> desktopSession;

        public DesktopSession()
        {
            desktopSession = CreateSessionForAlreadyRunningApp("InfoERPMain"); // InfoERPMain
        }

        ~DesktopSession()
        {
            desktopSession.Quit();
        }

        /// <summary>
        /// Create session for already running app
        /// </summary>
        /// <param name="appText">Part text of app process name or app title to search in running processes</param>
        /// <returns>Session for already running app</returns>
        /// <example>CreateSessionForAlreadyRunningApp("calc");</example>
        private static WindowsDriver<WindowsElement> CreateSessionForAlreadyRunningApp(string appText)
        {

            string pathWinAppDriver = AppDomain.CurrentDomain.BaseDirectory + "Windows Application Driver\\WinAppDriver.exe";
            var processStartInfo = new ProcessStartInfo
            {
                FileName = pathWinAppDriver,
                UseShellExecute = true,
                Verb = "runas",
                CreateNoWindow = true,
            };
            Process.Start(processStartInfo);
            IntPtr appTopLevelWindowHandle = new IntPtr();
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.IndexOf(appText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    clsProcess.MainWindowTitle.IndexOf(appText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    appTopLevelWindowHandle = clsProcess.MainWindowHandle;
                    break;
                }
            }
            var appTopLevelWindowHandleHex = appTopLevelWindowHandle.ToString("x"); //convert number to hex string
            var appCapabilities = new AppiumOptions();
            appCapabilities.AddAdditionalCapability("appTopLevelWindow", appTopLevelWindowHandleHex); // Root
            appCapabilities.AddAdditionalCapability("platformName", @"Windows");
            appCapabilities.AddAdditionalCapability("deviceName", @"WindowsPC");
            WindowsDriver<WindowsElement> appSession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
            appSession.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

            return appSession;
        }
        public WindowsDriver<WindowsElement> DesktopSessionElement
        {
            get { return desktopSession; }
        }

        private void FormatXPath(ref string xPath)
        {
            // cắt start xpath
            var xPathSPlit = xPath.Split('/');
            int xPathSPlitLength = xPathSPlit.Length;
            xPath = string.Join("/", xPathSPlit, 2, xPathSPlitLength - 2);
            xPath = "/" + xPath;

            // loại bỏ xpath lỗi
            string errorXPath = "[@AutomationId=\"\"[Editor\"]";
            string errorXPath2 = "[@Name=\"&lt;&lt;\"]";
            string errorXpath3 = "[@AutomationId=\"_EmbeddableTextBox\"]";
            //  int indexErrorXpathDataGrid = xPath.IndexOf("[position()=");
            //if (indexErrorXpathDataGrid > 0)
            //{
            //    xPath = xPath.Remove(indexErrorXpathDataGrid, 14);
            //}
            if (xPath.Contains(errorXPath)) // nếu chứa đoạn lỗi
            {
                // cắt
                //xPath = xPath.Substring(0, xPath.Length - errorXPath.Length);
                xPath = xPath.Replace(errorXPath, "");
            }
            if (xPath.Contains(errorXPath2))
            {
                xPath = xPath.Replace(errorXPath2, "");
            }
            if (xPath.Contains(errorXpath3))
            {
                xPath = xPath.Replace(errorXpath3, "");
            }
        }


        public WindowsElement FindElementById(string id, int n = 2)
        {
            WindowsElement windowElement = null;
            while (n-- > 0)
            {
                try
                {
                    windowElement = desktopSession.FindElementByAccessibilityId(id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(3000);
                }
                if (windowElement != null)
                {
                    break;
                }
            }
            return windowElement;
        }

        // xử lý xpath
      
        public WindowsElement FindElementByAbsoluteXPath(string xPath, int n = 2)
        {
            // xử lý xpath
            FormatXPath(ref xPath);
            WindowsElement windowElement = null;
            while (n-- > 0)
            {
                try
                {
                    windowElement = desktopSession.FindElementByXPath(xPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(3000);
                }
                if (windowElement != null)
                {
                    break;
                }
            }
            return windowElement;

        }

        public IList<WindowsElement> FindElementsByAbsoluteXPath(string xPath, int n= 2,bool index = false)
        {
            // xử lý xpath
            FormatXPath(ref xPath);
            if (index)
            {
                int indexErrorXpathDataGrid = xPath.IndexOf("[position()=");
                if (indexErrorXpathDataGrid > 0)
                {
                    xPath = xPath.Remove(indexErrorXpathDataGrid, 14);
                }
            }
            IList<WindowsElement> windowsElement = null;
            while (n-- > 0)
            {
                try
                {
                    windowsElement = desktopSession.FindElementsByXPath(xPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(3000);
                }
                if (windowsElement != null)
                {
                    break;
                }
                
            }
            return windowsElement;
        }
    }
}
