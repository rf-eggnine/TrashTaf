//  ©️ 2024 by RF@EggNine.com All Rights Reserved
﻿using Eggnine.TrashTas.XUnit.SkipAttributes;
using System.Globalization;

namespace Eggnine.TrashTas.XUnit.Tests
{
    public class ConfigurationTests
    {
        [SkippableFact, TestCase(14), Priority(2), SkipIfOsIsNot("windows"), SkipIfBrowserIsNot("chrome")]
        public void VerifyTestRunsOnWindowsWithChrome() => TestAdapter.Execute((ctx) =>
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            Assert.Equal("windows", ctx.OperatingSystemName, comparer);
            Assert.Equal("chrome", ctx.BrowserName, comparer);
        });
        [SkippableFact, TestCase(21), Priority(2), SkipIfOsIsNot("windows"), SkipIfBrowserIsNot("firefox")]
        public void VerifyTestRunsOnWindowsWithFirefox() => TestAdapter.Execute((ctx) =>
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            Assert.Equal("windows", ctx.OperatingSystemName, comparer);
            Assert.Equal("firefox", ctx.BrowserName, comparer);
        });
        [SkippableFact, TestCase(22), Priority(2), SkipIfOsIsNot("windows"), SkipIfBrowserIsNot("edge")]
        public void VerifyTestRunsOnWindowsWithEdge() => TestAdapter.Execute((ctx) =>
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            Assert.Equal("windows", ctx.OperatingSystemName, comparer);
            Assert.Equal("edge", ctx.BrowserName, comparer);
        });
        [SkippableFact, TestCase(19), Priority(2), SkipIfOsIsNot("ubuntu"), SkipIfBrowserIsNot("chrome")]
        public void VerifyTestRunsOnUbuntuWithChrome() => TestAdapter.Execute((ctx) =>
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            Assert.Equal("ubuntu", ctx.OperatingSystemName, comparer);
            Assert.Equal("chrome", ctx.BrowserName, comparer);
        });
        [SkippableFact, TestCase(20), Priority(2), SkipIfOsIsNot("ubuntu"), SkipIfBrowserIsNot("firefox")]
        public void VerifyTestRunsOnUbuntuWithFirefox() => TestAdapter.Execute((ctx) =>
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            Assert.Equal("ubuntu", ctx.OperatingSystemName, comparer);
            Assert.Equal("firefox", ctx.BrowserName, comparer);
        });
    }
}
