// TrashTaf © 2024 by RF@EggNine.com All Rights Reserved
using OpenQA.Selenium;

namespace TrashTaf.XUnit
{
    public class ExampleTests
    {
        [Fact, TestCase(1)]
        public void VerifyTestCaseOneIsFound() => TrashTafTestAdapter.Execute((ctx, webDriver) =>
        {
            // Arrange
            webDriver.Navigate().GoToUrl("https://github.com/rf-eggnine/TrashTaf/Issues");
            // Act
            var element = webDriver.FindElement(By.CssSelector("#partial-discussion-header .js-issue-title.markdown-title"));
            // Assert
            Assert.Equal("Verify Test Case 1 is found", element.Text);
        });
    }
}