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
using Xunit;

namespace Eggnine.TrashTaf.XUnit
{
    public class TrashTafTestAdapter
    {
        public TrashContext TrashContext;
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
                        case "operatingSystemName":
                            TrashContext.OperatingSystemName = property.Value.ToString();
                            break;
                        case "operatingSystemMajorVersion":
                            TrashContext.OperatingSystemMajorVersion = property.Value.ToString();
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
            ctx.TestCaseId = (int)testMethod.CustomAttributes.First(a => a.AttributeType == typeof(TestCase)).ConstructorArguments[0].Value;
            ctx.Priority = (int)testMethod.CustomAttributes.First(a => a.AttributeType == typeof(Priority)).ConstructorArguments[0].Value;
            Console.WriteLine($"Begining pre-execution for test case #{ctx.TestCaseId} {ctx.ClassName}.{ctx.TestName}");
            IEnumerable<SkipIf> skipIfs = testMethod.GetCustomAttributes().Where(a => a is SkipIf).Cast<SkipIf>();
            Console.WriteLine($"Checking if test case should be skipped");
            foreach(SkipIf skipIf in skipIfs)
            {
                skipIf.True(ctx);
            }
            Console.WriteLine("Starting WebDriver");
            WebDriver webDriver;
            switch (ctx.BrowserName)
            {
                case "chrome":
                    var chromeOptions = new ChromeOptions();
                    if (ctx.IsHeadless)
                    {
                        chromeOptions.AddArgument("--headless=new");
                    }
                    webDriver = new ChromeDriver(chromeOptions);
                    break;
                case "firefox":
                    var firefoxOptions = new FirefoxOptions();
                    if (ctx.IsHeadless)
                    {
                        firefoxOptions.AddAdditionalFirefoxOption("headless", true);
                        firefoxOptions.AddAdditionalFirefoxOption("-headless", true);
                        firefoxOptions.AddAdditionalFirefoxOption("--headless", true);
                        firefoxOptions.AddAdditionalOption("headless", true);
                        firefoxOptions.AddAdditionalOption("-headless", true);
                        firefoxOptions.AddAdditionalOption("--headless", true);
                        firefoxOptions.AddArgument("headless");
                        firefoxOptions.AddArgument("-headless");
                        firefoxOptions.AddArgument("--headless");
                    }
                    webDriver = new FirefoxDriver();
                    break;
                case "edge":
                    var edgeOptions = new EdgeOptions();
                    if (ctx.IsHeadless)
                    {
                        edgeOptions.AddArgument("--headless=new");
                    }
                    webDriver = new EdgeDriver(edgeOptions);
                    break;
                case "safari":
                    if (ctx.IsHeadless)
                    {
                        throw new Exception("Safari doesn't support headless mode");
                    }
                    webDriver = new SafariDriver();
                    break;
                case "android":
                    var androidOptions = new AppiumOptions();
                    if (ctx.IsHeadless)
                    {
                        androidOptions.AddAdditionalAppiumOption("isHeadless", true);
                    }
                    webDriver = new AndroidDriver(androidOptions);
                    break;
                case "ios":
                    var iosOptions = new AppiumOptions();
                    if (ctx.IsHeadless)
                    {
                        iosOptions.AddAdditionalAppiumOption("isHeadless", true);
                    }
                    webDriver = new IOSDriver(iosOptions);
                    break;
                default:
                    throw new Exception($"Unknown browser {ctx.BrowserName} please use chrome, firefox, edge, safari, android, or ios");
            };
            Console.WriteLine("WebDriver started");
            (webDriver as IJavaScriptExecutor).ExecuteScript("console.log('WebDriver started');");
            try
            {
                Console.WriteLine("Executing test action");
                (webDriver as IJavaScriptExecutor).ExecuteScript("console.log('Executing test action');");
                test(ctx, webDriver);
                Console.WriteLine("Begining post execution");
                (webDriver as IJavaScriptExecutor).ExecuteScript("console.log('Begining post execution');");
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

        /// <summary>
        /// Convenience method for Tests to call
        /// </summary>
        /// <param name="test"></param>
        public static void Execute(Action<TrashContext, WebDriver> test) => new TrashTafTestAdapter().ExecuteTest(test);
    }
}
