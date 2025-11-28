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

            // Determine if we are in CI (GitHub Actions sets GITHUB_ACTIONS=true)
            var isCI = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true"
                       || Environment.GetEnvironmentVariable("CI") == "true";

            if (isCI)
            {
                // Headless + CI flags
                options.AddArgument("--headless=new");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--window-size=1200,900");

                // Try to point to the system chromium binary (common locations)
                var possiblePaths = new[] { "/usr/bin/chromium-browser", "/usr/bin/chromium", "/usr/bin/google-chrome-stable" };
                foreach (var p in possiblePaths)
                {
                    if (File.Exists(p))
                    {
                        options.BinaryLocation = p;
                        break;
                    }
                }

                // If we didn’t find a binary, we still proceed and let driver error log show details
            }
            else
            {
                // Local: keep headed so a junior can see browser.
                // Uncomment if you prefer local headless:
                // options.AddArgument("--headless=new");
            }

            // Create a driver service using /usr/bin (system chromedriver) when on linux/CI
            ChromeDriverService service = null;
            try
            {
                // If chromedriver is installed via apt, it is located at /usr/bin/chromedriver
                var driverPath = "/usr/bin";
                if (Directory.Exists(driverPath))
                {
                    service = ChromeDriverService.CreateDefaultService(driverPath);
                    service.SuppressInitialDiagnosticInformation = true;
                    service.HideCommandPromptWindow = true;
                    // enable verbose logging if you want to debug: service.LogPath = "chromedriver.log"; service.EnableVerboseLogging = true;
                }
            }
            catch
            {
                // fallback to default service (NuGet chromedriver) if anything fails
            }

            Driver = service != null ? new ChromeDriver(service, options) : new ChromeDriver(options);
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
