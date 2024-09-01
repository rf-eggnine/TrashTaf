// TrashTaf © 2024 by RF@EggNine.com All Rights Reserved
using OpenQA.Selenium;
using System.Diagnostics;

namespace Eggnine.TrashTaf.XUnit.Tests
{
    public class ExampleTests
    {
        [SkippableFact, TestCase(1), Priority(1)]
        public void VerifyTestCaseOneIsFoundViaWebDriver() => TrashTafTestAdapter.Execute((ctx) =>
        {
            // Arrange
            Assert.NotNull(ctx.WebDriver);
            IWebDriver webDriver = ctx.WebDriver!;
            webDriver.Navigate().GoToUrl("https://github.com/login");
            Task.Delay(1000).Wait();
            IWebElement usernameField = webDriver.FindElement(By.Id("login_field"));
            usernameField.SendKeys(ctx.GitHubUsername);
            IWebElement passwordField = webDriver.FindElement(By.Id("password"));
            passwordField.SendKeys(ctx.GitHubPassword);
            IWebElement signInButton = webDriver.FindElement(By.XPath("//input[@value='Sign in']"));
            signInButton.Click();
            Task.Delay(1000).Wait();
            Debug.WriteLine((webDriver as IJavaScriptExecutor).ExecuteScript("return document.body.innerHTML"));
            webDriver.Navigate().GoToUrl("https://github.com/rf-eggnine/TrashTaf/issues/1");
            // Act
            var element = webDriver.FindElement(By.CssSelector("#partial-discussion-header .js-issue-title.markdown-title"));
            // Assert
            Assert.Equal("Verify Test Case 1 is found", element.Text);
        });
    }
}