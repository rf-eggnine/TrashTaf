// TrashTaf © 2024 by RF@EggNine.com All Rights Reserved
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

namespace TrashTaf.XUnit
{
    public class Adapter
    {
        public TrashContext TrashContext;
        public Adapter()
        {
            TrashContext = new();
            using Stream stream = File.OpenRead("appSettings.json");
            using StreamReader reader = new StreamReader(stream);
            JToken token = JsonConvert.DeserializeObject<JToken>(reader.ReadToEnd());
            foreach(JToken child in token.Children())
            {
                if(child is JProperty property)
                {
                    switch(property.Name)
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
                        case "operatingSystemVMajorVersion":
                            TrashContext.OperatingSystemMajorVersion = property.Value.ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void ExecuteTest(Action<TrashContext, WebDriver> test)
        {
            var ctx = TrashContext;
            var stackTrace = new StackTrace();
            ctx.TestName = stackTrace.GetFrame(2).GetMethod().Name;
            ctx.ClassName = stackTrace.GetFrame(2).GetMethod().ReflectedType.Name;
            ctx.TestCaseId = (int)stackTrace.GetFrame(2).GetMethod().CustomAttributes.First(a => a.AttributeType == typeof(TestCase)).ConstructorArguments[0].Value;
            Console.WriteLine($"Begining pre-execution for test case #{ctx.TestCaseId} {ctx.ClassName}.{ctx.TestName}");
            Console.WriteLine("Starting WebDriver");
            WebDriver webDriver = ctx.BrowserName switch
            {
                "chrome" => new ChromeDriver(),
                "firefox" => new FirefoxDriver(),
                "edge" => new EdgeDriver(),
                "safari" => new SafariDriver(),
                "android" => new AndroidDriver(new AppiumOptions()),
                "ios" => new IOSDriver(new AppiumOptions()),
                _ => throw new Exception($"Unknown browser {ctx.BrowserName} please use chrome, firefox, edge, safari, android, or ios"),
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
                ctx.Exception = ex;
                throw;
            }
            finally
            {
                webDriver.Quit();
            }
        }

        public static void Execute(Action<TrashContext, WebDriver> test) => new Adapter().ExecuteTest(test);
    }
}
