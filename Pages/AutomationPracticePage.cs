using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace CloudQA_Practice.Pages
{
    // Simple Page Object Model for AutomationPracticeForm page
    // The selectors try to be resilient: use label text, visible placeholder or nearby text
    public class AutomationPracticePage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public AutomationPracticePage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(8));
        }

        // Navigate to the page
        public void Go()
        {
            _driver.Navigate().GoToUrl("https://app.cloudqa.io/home/AutomationPracticeForm");
        }

        // Helper: find input by its label text (handles cases where attributes change)
        private IWebElement FindInputByLabel(string labelText)
        {
            // Find the label containing the text, then find the associated input.
            // Many junior engineers use friendly, readable XPaths for robustness.
            var label = _wait.Until(d => d.FindElement(By.XPath($"//label[contains(normalize-space(.), '{labelText}')]") ));
            // If label has 'for' attribute, use it
            var forAttr = label.GetAttribute("for");
            if (!string.IsNullOrEmpty(forAttr))
            {
                return _driver.FindElement(By.Id(forAttr));
            }
            // otherwise, look for input inside the same container or following-sibling
            var input = label.FindElement(By.XPath(".//following::input[1] | .//following::textarea[1] | .//following::select[1]"));
            return input;
        }

        // Field operations
        public void SetFirstName(string value)
        {
            var input = FindInputByLabel("First Name");
            input.Clear();
            input.SendKeys(value);
        }

        public string GetFirstName()
        {
            var input = FindInputByLabel("First Name");
            return input.GetAttribute("value") ?? string.Empty;
        }

        public void SetEmail(string value)
        {
            // Try multiple fallback strategies for email field: placeholder, label, type=email
            try
            {
                var byType = _driver.FindElement(By.CssSelector("input[type='email']"));
                byType.Clear();
                byType.SendKeys(value);
                return;
            }
            catch { /* fallback below */ }

            var input = FindInputByLabel("Email");
            input.Clear();
            input.SendKeys(value);
        }

        public string GetEmail()
        {
            try
            {
                var byType = _driver.FindElement(By.CssSelector("input[type='email']"));
                return byType.GetAttribute("value") ?? string.Empty;
            }
            catch { }

            var input = FindInputByLabel("Email");
            return input.GetAttribute("value") ?? string.Empty;
        }

        public void SelectCountry(string countryName)
        {
            // Find select by label text "Country" and select by visible text
            var label = _wait.Until(d => d.FindElement(By.XPath("//label[contains(normalize-space(.), 'Country')]") ));
            var forAttr = label.GetAttribute("for");
            IWebElement selectEl = null;
            if (!string.IsNullOrEmpty(forAttr))
            {
                selectEl = _driver.FindElement(By.Id(forAttr));
            }
            else
            {
                selectEl = label.FindElement(By.XPath(".//following::select[1]" ));
            }

            var select = new SelectElement(selectEl);
            // Try exact first, else try partial match
            try { select.SelectByText(countryName); }
            catch
            {
                foreach (var opt in select.Options)
                {
                    if (opt.Text.IndexOf(countryName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        select.SelectByText(opt.Text);
                        break;
                    }
                }
            }
        }

        public string GetSelectedCountry()
        {
            try
            {
                var label = _driver.FindElement(By.XPath("//label[contains(normalize-space(.), 'Country')]"));
                var forAttr = label.GetAttribute("for");
                IWebElement selectEl = null;
                if (!string.IsNullOrEmpty(forAttr))
                {
                    selectEl = _driver.FindElement(By.Id(forAttr));
                }
                else
                {
                    selectEl = label.FindElement(By.XPath(".//following::select[1]" ));
                }
                var select = new SelectElement(selectEl);
                return select.SelectedOption.Text;
            }
            catch { return string.Empty; }
        }
    }
}
