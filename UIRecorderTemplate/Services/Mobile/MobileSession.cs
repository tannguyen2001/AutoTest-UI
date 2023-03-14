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

namespace AutomationCLogic.Services
{
    public class MobileSession
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723/";
        WindowsDriver<WindowsElement> desktopSession;

        public MobileSession()
        {
            desktopSession = CreateSessionForAlreadyRunningApp("InfoERPMain"); // InfoERPMain
        }

        ~MobileSession()
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
                Verb = "runas"
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

        public WindowsDriver<WindowsElement> MobileSessionElement
        {
            get { return desktopSession; }
        }

        public WindowsElement FindElementById(string id, int nTryCount = 2)
        {
            WindowsElement uiTarget = null;

            while (nTryCount-- > 0)
            {
                try
                {
                    uiTarget = desktopSession.FindElementByAccessibilityId(id);
                }
                catch
                {
                }

                if (uiTarget != null)
                {
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(2000);
                }
            }

            return uiTarget;
        }

        // xử lý xpath
        private void FormatXPath(ref string xPath)
        {
            // cắt start xpath
            var xPathSPlit = xPath.Split('/');
            int xPathSPlitLength = xPathSPlit.Length;
            xPath = string.Join("/", xPathSPlit,2,xPathSPlitLength-2);
            xPath = "/" + xPath;

            // loại bỏ xpath lỗi
            string errorXPath = "[@AutomationId=\"\"[Editor\"]";
            string errorXPath2 = "[@Name=\"&lt;&lt;\"]";
            int indexErrorXpathDataGrid = xPath.IndexOf("[position()=");
            if (indexErrorXpathDataGrid > 0)
            {
                xPath = xPath.Remove(indexErrorXpathDataGrid, 14);
            }
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
        }

        public WindowsElement FindElementByAbsoluteXPath(string xPath, int nTryCount = 2)
        {
            WindowsElement uiTarget = null;

            // xử lý xpath
            FormatXPath(ref xPath);
                while (nTryCount-- > 0)
                {
                    try
                    {
                        uiTarget = desktopSession.FindElementByXPath(xPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    if (uiTarget != null)
                    {

                        break;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(2000);
                    }
                }
            return uiTarget;
        }

        public IList<WindowsElement> FindElementsByAbsoluteXPath(string xPath, int nTryCount = 2)
        {
            IList<WindowsElement> uiTarget = null;
            // xử lý xpath
            FormatXPath(ref xPath);
            while (nTryCount-- > 0)
            {
                try
                {
                    uiTarget = desktopSession.FindElementsByXPath(xPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (uiTarget != null)
                {
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(2000);
                }
            }

            return uiTarget;
        }
    }
}
