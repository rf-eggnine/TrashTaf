// TrashTaf © 2024 by RF@EggNine.com All Rights Reserved
using Eggnine.TrashTaf.XUnit.SkipAttributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Safari;
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
                        case "username":
                            TrashContext.Username = property.Value.ToString();
                            break;
                        case "password":
                            TrashContext.Password = property.Value.ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
            Console.WriteLine($"Browser: {TrashContext.BrowserName} {TrashContext.BrowserMajorVersion}");
            Console.WriteLine($"Operating System: {TrashContext.OperatingSystemName} {TrashContext.OperatingSystemMajorVersion}");
        }

        /// <summary>
        /// Provides integration points for supported features before and after the test
        /// </summary>
        /// <param name="test"></param>
        /// <exception cref="Exception"></exception>
        public void ExecuteTest(Action<TrashContext, WebDriver> test)
        {
            var ctx = TrashContext;
            var stackTrace = new StackTrace();
            var testMethod = stackTrace.GetFrame(2).GetMethod();
            ctx.TestName = testMethod.Name;
            ctx.ClassName = testMethod.ReflectedType.Name;
            ctx.SetTestCaseId(testMethod);
            ctx.SetPriority(testMethod);
            Console.WriteLine($"Begining pre-execution for test case #{ctx.TestCaseId} {ctx.ClassName}.{ctx.TestName}");
            IEnumerable<SkipIf> skipIfs = testMethod.GetCustomAttributes().Where(a => a is SkipIf).Cast<SkipIf>();
            Console.WriteLine($"Checking if test case should be skipped");
            foreach (SkipIf skipIf in skipIfs)
            {
                skipIf.True(ctx);
            }

            while (true)
            {
                try
                {
                    Console.WriteLine("Starting WebDriver");
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
                            throw new Exception($"Unknown browser {ctx.BrowserName} please use chrome, firefox, edge, safari, android, or ios");
                    };
                    webDriver = StartWebDriverWithRetries(webDriverFunc);
                    Console.WriteLine("WebDriver started");
                    (webDriver as IJavaScriptExecutor).ExecuteScript("console.log('WebDriver started');");
                    try
                    {
                        Console.WriteLine("Executing test action");
                        (webDriver as IJavaScriptExecutor).ExecuteScript("console.log('Executing test action');");
                        test(ctx, webDriver);

                        Console.WriteLine("Begining post execution");
                        (webDriver as IJavaScriptExecutor).ExecuteScript("console.log('Begining post execution');");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Test failed: {ctx.ClassName}.{ctx.TestName} with exception {ex.GetType().FullName} because {ex.Message}");
                        ctx.Exception = ex;
                        throw;
                    }
                    finally
                    {
                        Console.WriteLine("Tearing down WebDriver");
                        webDriver.Quit();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"test {ctx.ClassName}.{ctx.TestName} failed with {ex.GetType().Name} stating {ex.Message}");
                    if (firstException != null)
                    {
                        if (secondException != null)
                        {
                            if (firstException.GetType() == secondException.GetType() && secondException.GetType() == ex.GetType())
                            {
                                if (firstException.Message.Equals(secondException.Message) && secondException.Message.Equals(ex.Message))
                                    throw;
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
        }

        /// <summary>
        /// Starts the WebDriver with retry-able exceptions
        /// </summary>
        /// <param name="webDriverFunc"></param>
        /// <returns></returns>
        private WebDriver StartWebDriverWithRetries(Func<WebDriver> webDriverFunc)
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
                    Console.WriteLine($"WebDriver start failed on attempt {i}");
                    if (i > 4)
                        throw;
                }
                i++;
            }
        }

        /// <summary>
        /// Convenience method for Tests to call
        /// </summary>
        /// <param name="test"></param>
        public static void Execute(Action<TrashContext, WebDriver> test) => new TrashTafTestAdapter().ExecuteTest(test);
    }
}
