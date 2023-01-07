using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace AutoTestWeb
{
    class AddUnitTest
    {
        IWebDriver driver;

        [SetUp]
        public void SetupTest()
        {
            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            driver = new ChromeDriver(path + @"\drivers\");
            driver.Manage().Window.Maximize();
        }

        [Test]
        public void ExecuteTest()
        { // --------------------------------------------------------------------------------------
            // --  Source Code tham khao tu folder duoc chia se boi nhom C1                        --
            // --  Link: https://drive.google.com/drive/folders/1kIM3FYp2-LUNezUqD9-P37WVPdM6jvaB  --
            // --------------------------------------------------------------------------------------

            // Opens window and go to url
            driver.Navigate().GoToUrl("http://localhost/orangehrm-5.1/web/index.php/auth/login");
            driver.Manage().Window.Size = new System.Drawing.Size(1280, 720);

            // Login
            driver.FindElement(By.Name("username")).SendKeys("tantho");
            driver.FindElement(By.Name("password")).SendKeys("TanTho123@");
            driver.FindElement(By.CssSelector(".orangehrm-login-slot")).Click();
            driver.FindElement(By.Name("password")).Click();
            driver.FindElement(By.CssSelector(".orangehrm-login-slot")).Click();
            driver.FindElement(By.CssSelector(".oxd-button")).Click();
            Thread.Sleep(1000);

            // Get the files path
            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string dataPath = path + @"\data\units.csv";
            string resultPath = path + @"\result\unit-result.csv";

            // Navigate to Structure section
            driver.FindElement(By.LinkText("Admin")).Click();
            driver.FindElement(By.XPath("//span[@class='oxd-topbar-body-nav-tab-item' and text()='Organization ']")).Click();
            driver.FindElement(By.XPath("//a[@class='oxd-topbar-body-nav-tab-link' and text()='Structure']")).Click();
            Thread.Sleep(2000);

            // Enable edit mode and click ADD button
            driver.FindElement(By.XPath("//span[@class='oxd-switch-input oxd-switch-input--active --label-left']")).Click();


            using (var dataFile = new StreamReader(dataPath))
            {
                using (var resultFile = new StreamWriter(resultPath))
                {
                    var columns = dataFile.ReadLine().Split(',');
                    resultFile.WriteLine("id,name,expected,actual,result");
                    int index = 0;
                    while (dataFile.EndOfStream == false)
                    {
                        var values = dataFile.ReadLine().Split(',');
                        var resultRow = new StringBuilder();
                        resultRow.Append($"{values[0]},{values[1]},{values[2]},");

                        // Click ADD button
                        driver.FindElement(By.XPath("//button[@class='oxd-button oxd-button--medium oxd-button--secondary org-structure-add']")).Click();
                        Thread.Sleep(1000);

                        // Fill data into inputs
                        driver.FindElement(By.XPath("(//input[@class='oxd-input oxd-input--active'])[2]")).SendKeys(values[0]);
                        Thread.Sleep(500);
                        driver.FindElement(By.XPath("(//input[@class='oxd-input oxd-input--active'])[2]")).SendKeys(values[1]);
                        Thread.Sleep(500);

                        if (values[1].Length == 0)
                            driver.FindElement(By.XPath("//button[@type='submit']")).Click();

                        // Catch error message (if any)
                        try
                        {
                            var message = driver.FindElement(By.ClassName("oxd-input-field-error-message"));
                            if (message.Text.Length > 0)
                            {
                                resultRow.Append($"{message.Text},");
                                if (message.Text.Equals(values[2]))
                                    resultRow.Append("Pass");
                                else resultRow.Append("Fail");
                                // Click cancel button
                                driver.FindElement(By.XPath("//button[@class='oxd-button oxd-button--medium oxd-button--ghost' and @type='button']")).Click();
                            }
                        }
                        catch (Exception e)
                        {
                            // Console.WriteLine(e.Message);
                            // resultFile.WriteLine("NO Error Message");
                            resultRow.Append(',');
                            if(values[2].Equals(""))
                                resultRow.Append("Pass");
                            else resultRow.Append("Fail");
                            // Click submit button
                            driver.FindElement(By.XPath("//button[@type='submit']")).Click();
                        }

                        resultFile.WriteLine(resultRow.ToString());

                        Thread.Sleep(5000);
                    }
                }
            }

            Thread.Sleep(1000);
        }

        [TearDown]
        public void CloseTest()
        {
            driver.Quit();
        }
    }
}