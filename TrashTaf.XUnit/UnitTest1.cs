// TrashTaf © 2024 by RF@EggNine.com All Rights Reserved
using OpenQA.Selenium;

namespace TrashTaf.XUnit
{
    public class UnitTest1
    {
        [Fact, TestCase(1)]
        public void Test1() => Adapter.Execute((ctx, postman, webDriver) =>
        {
            // Arrange
            webDriver.Navigate().GoToUrl("https://github.com/rf-EggNine/TrashTaf/Issues");
            // Act
            var element = webDriver.FindElement(By.CssSelector("#partial-discussion-header .js-issue-title.markdown-title"));
            // Assert
            Assert.Equal("Verify Test Case 1 is found", element.Text);
        });
    }
}