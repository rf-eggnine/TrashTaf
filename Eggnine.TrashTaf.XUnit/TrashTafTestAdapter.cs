// TrashTaf © 2024 by RF@EggNine.com All Rights Reserved
using Eggnine.TrashTaf.XUnit.SkipAttributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Safari;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;

namespace Eggnine.TrashTaf.XUnit
{
    public class TrashTafTestAdapter
    {
        public TrashContext TrashContext;
        private Exception firstException;
        private Exception secondException;
        private Exception thirdException;

        public TrashTafTestAdapter()
        {
            TrashContext = new();
            using Stream stream = File.OpenRead("appSettings.json");
            using StreamReader reader = new StreamReader(stream);
            JToken token = JsonConvert.DeserializeObject<JToken>(reader.ReadToEnd());
            foreach (JToken child in token.Children())
            {
                if (child is JProperty property)
                {
                    switch (property.Name)
                    {
                        case "browserName":
                            TrashContext.BrowserName = property.Value.ToString();
                            break;
                        case "browserMajorVersion":
                            TrashContext.BrowserMajorVersion = property.Value.ToString();
                            break;
                        case "operatingSystemNameAndMajorVersion":
                            var splitOsAndVersion = property.Value.ToString().Split("-");
                            TrashContext.OperatingSystemName = splitOsAndVersion[0];
                            TrashContext.OperatingSystemMajorVersion = splitOsAndVersion[1];
                            break;
                        case "isHeadless":
                            bool.TryParse(property.Value.ToString(), out TrashContext.IsHeadless);
                            break;
                        case "gitHubUsername":
                            TrashContext.GitHubUsername = property.Value.ToString();
                            break;
                        case "gitHubPassword":
                            TrashContext.GitHubPassword = property.Value.ToString();
                            break;
                        case "databaseConnectionString":
                            TrashContext.DatabaseConnectionString = property.Value.ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
            TrashContext.LogMessage($"Browser: {TrashContext.BrowserName} {TrashContext.BrowserMajorVersion}");
            TrashContext.LogMessage($"Operating System: {TrashContext.OperatingSystemName} {TrashContext.OperatingSystemMajorVersion}");
        }

        /// <summary>
        /// Provides integration points for supported features before and after the test
        /// </summary>
        /// <param name="test"></param>
        /// <exception cref="Exception"></exception>
        public void ExecuteTest(Action<TrashContext> test)
        {
            var ctx = TrashContext;
            ctx.RunDateTime = DateTime.UtcNow;
            ctx.StartTimeMs = GetUnixTimeStamp(ctx.RunDateTime);
            var stackTrace = new StackTrace();
            var testMethod = stackTrace.GetFrame(2).GetMethod();
            ctx.TestName = testMethod.Name;
            ctx.ClassName = testMethod.ReflectedType.Name;
            ctx.SetTestCaseId(testMethod);
            ctx.SetPriority(testMethod);
            TrashContext.LogMessage($"Begining pre-execution for test case #{ctx.TestCaseId} {ctx.ClassName}.{ctx.TestName}");
            IEnumerable<SkipIf> skipIfs = testMethod.GetCustomAttributes().Where(a => a is SkipIf).Cast<SkipIf>();
            TrashContext.LogMessage($"Checking if test case should be skipped");
            foreach (SkipIf skipIf in skipIfs)
            {
                skipIf.True(ctx);
            }

            while (true)
            {
                try
                {
                    TrashContext.LogMessage("Starting WebDriver");
                    WebDriver webDriver;
                    Func<WebDriver> webDriverFunc;
                    switch (ctx.BrowserName)
                    {
                        case "chrome":
                            var chromeService = ChromeDriverService.CreateDefaultService();
                            chromeService.LogPath = "chromelog.txt";
                            chromeService.EnableVerboseLogging = true;
                            var chromeOptions = new ChromeOptions();
                            if (ctx.IsHeadless)
                            {
                                chromeOptions.AddArgument("--headless=new");
                            }
                            webDriverFunc = () => new ChromeDriver(chromeService, chromeOptions);
                            break;
                        case "firefox":
                            var firefoxOptions = new FirefoxOptions();
                            if (ctx.IsHeadless)
                            {
                                firefoxOptions.AddArgument("--headless");
                            }
                            webDriverFunc = () => new FirefoxDriver();
                            break;
                        case "edge":
                            if (!ctx.OperatingSystemName.Equals("Windows", StringComparison.OrdinalIgnoreCase))
                            {
                                throw new Exception("Edge only runs on Windows");
                            }
                            var edgeOptions = new EdgeOptions();
                            if (ctx.IsHeadless)
                            {
                                edgeOptions.AddArgument("--headless=new");
                            }
                            webDriverFunc = () => new EdgeDriver(edgeOptions);
                            break;
                        case "safari":
                            if (!ctx.OperatingSystemName.Equals("MacOs", StringComparison.OrdinalIgnoreCase))
                            {
                                throw new Exception("Safari only runs on MacOs");
                            }
                            if (ctx.IsHeadless)
                            {
                                throw new Exception("Safari doesn't support headless mode");
                            }
                            webDriverFunc = () => new SafariDriver();
                            break;
                        case "android":
                            var androidOptions = new AppiumOptions();
                            if (ctx.IsHeadless)
                            {
                                androidOptions.AddAdditionalAppiumOption("isHeadless", true);
                            }
                            webDriverFunc = () => new AndroidDriver(androidOptions);
                            break;
                        case "ios":
                            var iosOptions = new AppiumOptions();
                            if (ctx.IsHeadless)
                            {
                                iosOptions.AddAdditionalAppiumOption("isHeadless", true);
                            }
                            webDriverFunc = () => new IOSDriver(iosOptions);
                            break;
                        default:
                            webDriverFunc = () => null;
                            break;
                    };
                    ctx.WebDriver = StartWebDriverWithRetries(ctx, webDriverFunc);
                    TrashContext.LogMessage("WebDriver started");
                    try
                    {
                        TrashContext.LogMessage("Executing test action");
                        test(ctx);

                        TrashContext.LogMessage("Begining post execution");
                        ctx.StopTimeMs = GetUnixTimeStamp(DateTime.UtcNow);
                        RecordTestSuccess(ctx);
                        break;
                    }
                    catch (Exception ex)
                    {
                        ctx.LogMessage($"Test failed: {ctx.ClassName}.{ctx.TestName} with exception {ex.GetType().FullName} because {ex.Message}");
                        ctx.Exception = ex;
                        throw;
                    }
                    finally
                    {
                        ctx.LogMessage("Tearing down WebDriver");
                        ctx.WebDriver?.Quit();
                        ctx.WebDriver = null;
                    }
                }
                catch (Exception ex)
                {
                    ctx.LogMessage($"test {ctx.ClassName}.{ctx.TestName} failed with {ex.GetType().Name} stating {ex.Message}");
                    if (firstException != null)
                    {
                        if (secondException != null)
                        {
                            if (firstException.GetType() == secondException.GetType() && secondException.GetType() == ex.GetType())
                            {
                                if (firstException.Message.Equals(secondException.Message) && secondException.Message.Equals(ex.Message))
                                {
                                    ctx.StopTimeMs = GetUnixTimeStamp(DateTime.UtcNow);
                                    throw;
                                }
                            }
                            firstException = ex;
                            secondException = null;
                        }
                        else
                        {
                            if (ex.GetType() == firstException.GetType() && ex.Message.Equals(firstException.Message))
                            {
                                secondException = ex;
                            }
                            else
                            {
                                firstException = ex;
                                secondException = null;
                            }
                        }
                    }
                    else
                    {
                        firstException = ex;
                    }
                }
            }
            ctx.StopTimeMs = GetUnixTimeStamp(DateTime.UtcNow);
        }

        private void RecordTestSuccess(TrashContext ctx)
        {
            NpgsqlCommand command = new();
            command.Connection = new NpgsqlConnection(ctx.DatabaseConnectionString);
            command.CommandText = "INSERT INTO testRuns (testName, className, runDateTime, durationMs, result, exceptionType, exceptionMessage, operatingSystemName, operatingSystemVersion, browserName, browserVersion, logMessages)" +
                                              " VALUES (@testName, @className, @runDateTime, @durationMs, @result, @exceptionType, @exceptionMessage, @operatingSystemName, @operatingSystemVersion, @browserName, @browserVersion, @logMessages)";
            command.Parameters.Add(CreateParameter(command, "testName", ctx.TestName));
            command.Parameters.Add(CreateParameter(command, "className", ctx.ClassName));
            command.Parameters.Add(CreateParameter(command, "runDateTime", ctx.RunDateTime));
            command.Parameters.Add(CreateParameter(command, "durationMs", ctx.StopTimeMs - ctx.StartTimeMs));
            command.Parameters.Add(CreateParameter(command, "result", ctx.Result));
            command.Parameters.Add(CreateParameter(command, "exceptionType", ctx.Exception?.GetType().FullName ?? string.Empty));
            command.Parameters.Add(CreateParameter(command, "exceptionMessage", ctx.Exception?.Message ?? string.Empty));
            command.Parameters.Add(CreateParameter(command, "operatingSystemName", ctx.OperatingSystemName));
            command.Parameters.Add(CreateParameter(command, "operatingSystemVersion", ctx.OperatingSystemMajorVersion));
            command.Parameters.Add(CreateParameter(command, "browserName", ctx.BrowserName));
            command.Parameters.Add(CreateParameter(command, "browserVersion", ctx.BrowserMajorVersion));
            command.Parameters.Add(CreateParameter(command, "logMessages", ctx.GetLogMessages()));
            Assert.Equal(1, command.ExecuteNonQuery());
        }

        private DbParameter CreateParameter(NpgsqlCommand command, string name, object value)
        {
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }

        private long GetUnixTimeStamp(DateTime time)
        {
            return new DateTimeOffset(time).ToUnixTimeSeconds();
        }

        /// <summary>
        /// Starts the WebDriver with retry-able exceptions
        /// </summary>
        /// <param name="webDriverFunc"></param>
        /// <returns></returns>
        private WebDriver StartWebDriverWithRetries(TrashContext ctx, Func<WebDriver> webDriverFunc)
        {
            int i = 0;
            while (true)
            {
                try
                {
                    return webDriverFunc();
                }
                catch (WebDriverException)
                {
                    ctx.LogMessage($"WebDriver start failed on attempt {i}");
                    if (i > 4)
                        throw;
                    i++;
                }
            }
        }

        /// <summary>
        /// Convenience method for Tests to call
        /// </summary>
        /// <param name="test"></param>
        public static void Execute(Action<TrashContext> test) => new TrashTafTestAdapter().ExecuteTest(test);
    }
}
