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
    class AddEmployeeTest
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
        {
            // --------------------------------------------------------------------------------------
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

            Thread.Sleep(2000);

            // Get the files path
            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string dataPath = path + @"\data\employees.csv";
            string resultPath = path + @"\result\employee-result.csv";
            
            using (var dataFile = new StreamReader(dataPath))
            {
                using (var resultFile = new StreamWriter(resultPath))
                {
                    // Read fields
                    string title = dataFile.ReadLine();
                    int indexId = 0, indexFirstname = 1, indexMiddlename = 2,
                        indexLastname = 3, indexUsername = 4, indexPassword = 5,
                        indexConfirmPassword = 6, indexExpectedResult = 8;
                    int index = 1;
                    bool isPass = false;

                    //Read contents
                    while (dataFile.EndOfStream == false)
                    {
                        var line = dataFile.ReadLine();
                        var values = line.Split(',');
                        StringBuilder result = new StringBuilder();
                        result.Append($"TC#{index}, ");
                        result.Append($"Input: {values[indexId]}-{values[indexFirstname]}-{values[indexMiddlename]}-" +
                            $"{values[indexLastname]}-{values[indexUsername]}-{values[indexPassword]}, ");
                        result.Append($"Expected output: {values[indexExpectedResult]}, ");

                        // Add employee
                        driver.FindElement(By.LinkText("Add Employee")).Click();
                        {
                            var element = driver.FindElement(By.LinkText("Add Employee"));
                            Actions builder = new Actions(driver);
                            builder.MoveToElement(element).Perform();
                        }

                        driver.FindElement(By.Name("firstName")).Click();
                        driver.FindElement(By.Name("firstName")).SendKeys(values[indexFirstname]);
                        driver.FindElement(By.Name("middleName")).Click();
                        driver.FindElement(By.Name("middleName")).SendKeys(values[indexMiddlename]);
                        driver.FindElement(By.Name("lastName")).Click();
                        driver.FindElement(By.Name("lastName")).SendKeys(values[indexLastname]);
                        
                        var idInput = driver.FindElement(By.CssSelector(".orangehrm-employee-container > div.orangehrm-employee-form > div:nth-child(1) > div.oxd-grid-2.orangehrm-full-width-grid > div > div > div:nth-child(2) > input"));
                        idInput.SendKeys(Keys.Control + "a");
                        idInput.SendKeys(Keys.Delete);
                        idInput.SendKeys(values[indexId]);
                        driver.FindElement(By.CssSelector(".orangehrm-employee-container > div.orangehrm-employee-form > div.oxd-form-row.user-form-header > div > label > span")).Click();
                        Thread.Sleep(1000);
                        
                        driver.FindElement(By.CssSelector(".orangehrm-employee-container > div.orangehrm-employee-form > div:nth-child(4) > div > div:nth-child(1) > div > div:nth-child(2) > input")).SendKeys(values[indexUsername]);
                        driver.FindElement(By.CssSelector(".orangehrm-employee-container > div.orangehrm-employee-form > div:nth-child(4) > div > div:nth-child(2) > div > div.--status-grouped-field > div:nth-child(1) > div:nth-child(2) > div > label > span")).Click();
                        driver.FindElement(By.CssSelector(".orangehrm-employee-container > div.orangehrm-employee-form > div.oxd-form-row.user-password-row > div > div.oxd-grid-item.oxd-grid-item--gutters.user-password-cell > div > div:nth-child(2) > input")).SendKeys(values[indexPassword]);
                        driver.FindElement(By.CssSelector(".orangehrm-employee-container > div.orangehrm-employee-form > div.oxd-form-row.user-password-row > div > div:nth-child(2) > div > div:nth-child(2) > input")).SendKeys(values[indexConfirmPassword]);

                        // Get actual output
                        result.Append("- Actual output: ");
                        
                        // Fullname input message
                        var firstnameMessage = driver.FindElements(By.CssSelector("#app > div.oxd-layout > div.oxd-layout-container > div.oxd-layout-context > div > div > form > div.orangehrm-employee-container > div.orangehrm-employee-form > div:nth-child(1) > div.oxd-grid-1.orangehrm-full-width-grid > div > div > div.--name-grouped-field > div:nth-child(1) > span"));
                        if (firstnameMessage.Count > 0)
                        {
                            isPass = true;
                            result.Append("FirstName Input: " + firstnameMessage.ElementAtOrDefault(0).Text + ", ");
                        }
                        
                        // Lastname input message
                        var lastnameMessage = driver.FindElements(By.CssSelector("#app > div.oxd-layout > div.oxd-layout-container > div.oxd-layout-context > div > div > form > div.orangehrm-employee-container > div.orangehrm-employee-form > div:nth-child(1) > div.oxd-grid-1.orangehrm-full-width-grid > div > div > div.--name-grouped-field > div:nth-child(3) > span"));
                        if (lastnameMessage.Count > 0)
                        {
                            isPass = true;
                            result.Append("LastName Input: " + lastnameMessage.ElementAtOrDefault(0).Text + ", ");
                        }
                        
                        // ID input message
                        var idMessage = driver.FindElements(By.CssSelector("#app > div.oxd-layout > div.oxd-layout-container > div.oxd-layout-context > div > div > form > div.orangehrm-employee-container > div.orangehrm-employee-form > div:nth-child(1) > div.oxd-grid-2.orangehrm-full-width-grid > div > div > span"));
                        if (idMessage.Count > 0)
                        {
                            isPass = true;
                            result.Append("ID Input: " + idMessage.ElementAtOrDefault(0).Text + ", ");
                        }
                        
                        // username input message
                        var usernameMessage = driver.FindElements(By.CssSelector("#app > div.oxd-layout > div.oxd-layout-container > div.oxd-layout-context > div > div > form > div.orangehrm-employee-container > div.orangehrm-employee-form > div:nth-child(4) > div > div:nth-child(1) > div > span"));
                        if (usernameMessage.Count > 0)
                        {
                            isPass = true;
                            result.Append("Username Input: " + usernameMessage.ElementAtOrDefault(0).Text + ", ");
                        }
                        
                        // password input message
                        var passwordMessage = driver.FindElements(By.CssSelector("#app > div.oxd-layout > div.oxd-layout-container > div.oxd-layout-context > div > div > form > div.orangehrm-employee-container > div.orangehrm-employee-form > div.oxd-form-row.user-password-row > div > div.oxd-grid-item.oxd-grid-item--gutters.user-password-cell > div > span"));
                        if (passwordMessage.Count > 0)
                        {
                            isPass = true;
                            result.Append("Password Input: " + passwordMessage.ElementAtOrDefault(0).Text + ", ");
                        }
                        
                         // confirmPassword input message
                        var confirmPasswordMessage = driver.FindElements(By.CssSelector("#app > div.oxd-layout > div.oxd-layout-container > div.oxd-layout-context > div > div > form > div.orangehrm-employee-container > div.orangehrm-employee-form > div.oxd-form-row.user-password-row > div > div:nth-child(2) > div > span"));
                        if (confirmPasswordMessage.Count > 0)
                        {
                            isPass = true;
                            result.Append("Confirm Password Input: " + confirmPasswordMessage.ElementAtOrDefault(0).Text + ", ");
                        }

                        // Click save employee button
                        driver.FindElement(By.CssSelector("#app > div.oxd-layout > div.oxd-layout-container > div.oxd-layout-context > div > div > form > div.oxd-form-actions > button.oxd-button.oxd-button--medium.oxd-button--secondary.orangehrm-left-space")).Click();
                        Thread.Sleep(2000);

                        // Get message result
                        var messageElement = driver.FindElements(By.CssSelector("p.oxd-toast-content-text"));

                        if (messageElement.Count > 0)
                        {
                            result.Append(messageElement.ElementAtOrDefault(0).Text + ", ");
                        }

                        Thread.Sleep(6000);

                        driver.FindElement(By.LinkText("Employee List")).Click();
                        Thread.Sleep(2000);

                        var listIdElement = driver.FindElements(By.CssSelector(".oxd-table-card .oxd-table-row .oxd-table-cell:nth-child(2) div"));
                        for (int i = 0; i < listIdElement.Count; i++)
                        {
                            var elementId = listIdElement.ElementAt(i).Text;
                            if (elementId == values[indexId])
                            {
                                result.Append("Result: Pass");
                                isPass = true;
                                break;
                            }
                        }
                        if (isPass == false)
                        {
                            result.Append("Result: Fail");
                        }

                        resultFile.WriteLine(result.ToString());
                        index += 1;

                        driver.FindElement(By.LinkText("Employee List")).Click();
                    }
                }
            }
            Thread.Sleep(5000);
        }

        [TearDown]
        public void CloseTest()
        {
            driver.Quit();
        }
    }
}
