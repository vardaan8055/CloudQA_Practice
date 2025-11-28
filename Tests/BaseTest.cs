using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO; 
// using System will already be present

namespace CloudQA_Practice.Tests
{
    public class BaseTest
    {
        protected IWebDriver Driver { get; private set; }

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();

            // Detect CI environment (GitHub Actions sets GITHUB_ACTIONS=true)
            var isCI = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true"
                       || Environment.GetEnvironmentVariable("CI") == "true";

            if (isCI)
            {
                // CI-friendly flags
                options.AddArgument("--headless=new");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--window-size=1200,900");

                // Look for common chromium binary locations
                var possiblePaths = new[] { "/usr/bin/chromium-browser", "/usr/bin/chromium", "/usr/bin/google-chrome-stable" };
                foreach (var p in possiblePaths)
                {
                    if (File.Exists(p))
                    {
                        options.BinaryLocation = p;
                        break;
                    }
                }
            }
            else
            {
                // Local: keep headed so the browser is visible while developing
                // Uncomment to run local headless:
                // options.AddArgument("--headless=new");
            }

            // Try to use system chromedriver when available under /usr/bin (apt install)
            ChromeDriverService service = null;
            try
            {
                var driverPath = "/usr/bin";
                if (Directory.Exists(driverPath))
                {
                    service = ChromeDriverService.CreateDefaultService(driverPath);
                    service.SuppressInitialDiagnosticInformation = true;
                    service.HideCommandPromptWindow = true;
                    // For debugging, you can enable verbose logging:
                    // service.LogPath = "chromedriver.log";
                    // service.EnableVerboseLogging = true;
                }
            }
            catch
            {
                // ignore and fall back to default service (NuGet chromedriver)
            }

            Driver = service != null ? new ChromeDriver(service, options) : new ChromeDriver(options);
            Driver.Manage().Window.Size = new System.Drawing.Size(1200, 900);
        }

        [TearDown]
        public void Teardown()
        {
            try { Driver.Quit(); } catch { }
        }
    }
}
