using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace CloudQA_Practice.Tests
{
    // Basic test base: opens Chrome, provides WebDriver to tests.
    // Kept intentionally simple (junior-friendly).
    public class BaseTest
    {
        protected IWebDriver Driver { get; private set; }

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            // Run headed by default - junior engineers often prefer seeing the browser.
            Driver = new ChromeDriver(options);
            Driver.Manage().Window.Size = new System.Drawing.Size(1200, 900);
        }

        [TearDown]
        public void Teardown()
        {
            try
            {
                Driver.Quit();
            }
            catch { }
        }
    }
}
