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

        public void SelectCountry(string countryName){

            // Find label for Country (same approach as before)
            var label = _wait.Until(d => d.FindElement(By.XPath("//label[contains(normalize-space(.), 'Country')]")));
            var forAttr = label.GetAttribute("for");
            IWebElement target = null;
            if (!string.IsNullOrEmpty(forAttr))
            {
                target = _driver.FindElement(By.Id(forAttr));
            }
            else
            {
                target = label.FindElement(By.XPath(".//following::select[1] | .//following::input[1]"));
            }

            // If it's a select element, use SelectElement
            var tag = target.TagName?.ToLowerInvariant() ?? "";
            if (tag == "select")
            {
                var select = new SelectElement(target);
                try
                {
                    select.SelectByText(countryName);
                    return;
                }
                catch
                {
                    foreach (var opt in select.Options)
                    {
                        if (opt.Text.IndexOf(countryName, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            select.SelectByText(opt.Text);
                            return;
                        }
                    }
                }
            }

            // Otherwise assume it's an input (autocomplete). Type, wait for suggestions, choose best match.
            if (tag == "input" || tag == "textarea" || string.IsNullOrEmpty(tag))
            {
                target.Clear();
                target.SendKeys(countryName);

                // Wait briefly for suggestion dropdown to appear.
                // Common patterns: a ul/li list, or divs with role=listbox/option.
                IWebElement suggestion = null;
                try
                {
                    // try common listbox pattern
                    suggestion = _wait.Until(d =>
                    {
                        // find visible option that contains the country name
                        var opts = d.FindElements(By.XPath("//ul//li[normalize-space(.) and contains(normalize-space(.), '" + countryName + "')] | //div[@role='listbox']//div[contains(normalize-space(.), '" + countryName + "')] | //div[contains(@class,'suggest') or contains(@class,'dropdown')]//div[contains(normalize-space(.), '" + countryName + "')]"));
                        foreach (var o in opts)
                        {
                            if (o.Displayed)
                                return o;
                        }
                        return null;
                    });
                }
                catch { /* timed out, fallback to pressing Enter */ }

                if (suggestion != null)
                {
                    try
                    {
                        suggestion.Click();
                        return;
                    }
                    catch
                    {
                        // fallback to sending Enter
                    }
                }

                // fallback: press Enter to accept the typed value
                target.SendKeys(Keys.Enter);
            }
        }


        public string GetSelectedCountry(){
            try
            {
                var label = _driver.FindElement(By.XPath("//label[contains(normalize-space(.), 'Country')]"));
                var forAttr = label.GetAttribute("for");
                IWebElement target = null;
                if (!string.IsNullOrEmpty(forAttr))
                {
                    target = _driver.FindElement(By.Id(forAttr));
                }
                else
                {
                    target = label.FindElement(By.XPath(".//following::select[1] | .//following::input[1]"));
                }

                var tag = target.TagName?.ToLowerInvariant() ?? "";
                if (tag == "select")
                {
                    var select = new SelectElement(target);
                    return select.SelectedOption.Text;
                }

                // If it's an input, return its value or visible text of chosen suggestion
                if (tag == "input" || tag == "textarea" || string.IsNullOrEmpty(tag))
                {
                    var val = target.GetAttribute("value");
                    if (!string.IsNullOrEmpty(val)) return val.Trim();

                    // as a fallback, try to read a nearby display element
                    try
                    {
                        var disp = label.FindElement(By.XPath(".//following::*[contains(@class,'selected') or contains(@class,'value')][1]"));
                        if (disp != null) return disp.Text.Trim();
                    }
                    catch { }
                }
            }
            catch { }

            return string.Empty;
        }
    }
}
