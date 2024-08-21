using Eggnine.TrashTaf.XUnit.SkipAttributes;
using System.Globalization;

namespace Eggnine.TrashTaf.XUnit.Tests
{
    public class ConfigurationTests
    {
        [SkippableFact, TestCase(14), Priority(2), SkipIfOsIsNot("windows"), SkipIfBrowserIsNot("chrome")]
        public void VerifyTestRunsOnWindowsWithChrome() => TrashTafTestAdapter.Execute((ctx, webDriver) =>
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            Assert.Equal("windows", ctx.OperatingSystemName, comparer);
            Assert.Equal("chrome", ctx.BrowserName, comparer);
        });
        [SkippableFact, TestCase(21), Priority(2), SkipIfOsIsNot("windows"), SkipIfBrowserIsNot("firefox")]
        public void VerifyTestRunsOnWindowsWithFirefox() => TrashTafTestAdapter.Execute((ctx, webDriver) =>
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            Assert.Equal("windows", ctx.OperatingSystemName, comparer);
            Assert.Equal("firefox", ctx.BrowserName, comparer);
        });
        [SkippableFact, TestCase(22), Priority(2), SkipIfOsIsNot("windows"), SkipIfBrowserIsNot("edge")]
        public void VerifyTestRunsOnWindowsWithEdge() => TrashTafTestAdapter.Execute((ctx, webDriver) =>
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            Assert.Equal("windows", ctx.OperatingSystemName, comparer);
            Assert.Equal("edge", ctx.BrowserName, comparer);
        });
        [SkippableFact, TestCase(19), Priority(2), SkipIfOsIsNot("ubuntu"), SkipIfBrowserIsNot("chrome")]
        public void VerifyTestRunsOnUbuntuWithChrome() => TrashTafTestAdapter.Execute((ctx, webDriver) =>
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            Assert.Equal("ubuntu", ctx.OperatingSystemName, comparer);
            Assert.Equal("chrome", ctx.BrowserName, comparer);
        });
        [SkippableFact, TestCase(20), Priority(2), SkipIfOsIsNot("ubuntu"), SkipIfBrowserIsNot("firefox")]
        public void VerifyTestRunsOnUbuntuWithFirefox() => TrashTafTestAdapter.Execute((ctx, webDriver) =>
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            Assert.Equal("ubuntu", ctx.OperatingSystemName, comparer);
            Assert.Equal("firefox", ctx.BrowserName, comparer);
        });
    }
}
