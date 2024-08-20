// TrashTaf © 2024 by RF@EggNine.com All Rights Reserved
using OpenQA.Selenium;
using System.Diagnostics;

namespace TrashTaf.XUnit
{
    public class ExampleTests
    {
        [Fact, TestCase(1)]
        public void VerifyTestCaseOneIsFound() => TrashTafTestAdapter.Execute((ctx, webDriver) =>
        {
            // Arrange
            webDriver.Navigate().GoToUrl("https://github.com/login");
            Task.Delay(1000).Wait();
            IWebElement usernameField = webDriver.FindElement(By.Id("login_field"));
            usernameField.SendKeys(ctx.Username);
            IWebElement passwordField = webDriver.FindElement(By.Id("password"));
            passwordField.SendKeys(ctx.Password);
            IWebElement signInButton = webDriver.FindElement(By.XPath("//input[@value='Sign in']"));
            signInButton.Click();
            Task.Delay(1000).Wait();
            Debug.WriteLine((webDriver as IJavaScriptExecutor).ExecuteScript("return document.body.getInnerHTML()"));
            webDriver.Navigate().GoToUrl("https://github.com/rf-eggnine/TrashTaf/issues/1");
            // Act
            var element = webDriver.FindElement(By.CssSelector("#partial-discussion-header .js-issue-title.markdown-title"));
            // Assert
            Assert.Equal("Verify Test Case 1 is found", element.Text);
        });
    }
}