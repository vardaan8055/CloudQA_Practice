using NUnit.Framework;
using CloudQA_Practice.Pages;

namespace CloudQA_Practice.Tests
{
    // Three small tests targeting different field types.
    // Tests are written to look like a junior engineer: clear, readable, minimal helpers.
    public class PracticeTests : BaseTest
    {
        [Test]
        public void FirstName_Field_AllowsTyping_AndPersists()
        {
            var page = new AutomationPracticePage(Driver);
            page.Go();

            var name = "Vardaan"; // sample input
            page.SetFirstName(name);

            Assert.AreEqual(name, page.GetFirstName(), "First name should be persisted in field.");
        }

        [Test]
        public void Email_Field_AcceptsEmail_InputAndShowsValue()
        {
            var page = new AutomationPracticePage(Driver);
            page.Go();

            var email = "vardaan.test@example.com";
            page.SetEmail(email);

            Assert.AreEqual(email, page.GetEmail(), "Email should be persisted in the email field.");
        }

        [Test]
        public void Country_Select_AllowsSelection_ByVisibleText()
        {
            var page = new AutomationPracticePage(Driver);
            page.Go();

            // choose a common option (will fallback to partial matches)
            page.SelectCountry("India");

            StringAssert.Contains("India", page.GetSelectedCountry(), "Selected country should contain 'India'.");
        }
    }
}
