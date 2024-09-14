//  © 2024 by RF@EggNine.com All Rights Reserved

using OpenQA.Selenium;
using System.Reflection;
using System.Text;

namespace Eggnine.TrashTas.XUnit
{
    public class TrashContext
    {
        public Dictionary<string, object> Properties;
        public string TestName;
        public string ClassName;
        public int TestCaseId;
        public string BrowserName;
        public string BrowserMajorVersion;
        public string OperatingSystemName;
        public string OperatingSystemMajorVersion;
        public string GitHubUsername;
        public string GitHubPassword;
        public Exception Exception;
        public bool IsHeadless;

        public int Priority { get; internal set; }
        public long StartTimeMs { get; internal set; }
        public long StopTimeMs { get; internal set; }
        public DateTime RunDateTime { get; internal set; }
        public string Result { get; internal set; }
        public StringBuilder LogMessages { get; private set; } = new();
        public WebDriver? WebDriver { get; internal set; }
        public string DatabaseConnectionString { get; internal set; }

        public void LogMessage(string message)
        {
            Console.WriteLine(message);
            LogMessages.AppendLine(message);
            try
            {
                (WebDriver as IJavaScriptExecutor)?.ExecuteScript($"console.log('{message}');");
            }
            catch (Exception e)
            {
                string message2 = $"Error logging to web driver console {e.GetType().FullName} with message {e.Message}";
                Console.WriteLine(message2);
                LogMessages.AppendLine(message2);
            }
        }

        internal object GetLogMessages()
        {
            return LogMessages.ToString();
        }

        internal void SetPriority(MethodBase testMethod)
        {
            Priority = (int)testMethod.CustomAttributes.First(a => a.AttributeType == typeof(Priority)).ConstructorArguments[0].Value;
        }

        internal void SetTestCaseId(MethodBase testMethod)
        {
            TestCaseId = (int)testMethod.CustomAttributes.First(a => a.AttributeType == typeof(TestCase)).ConstructorArguments[0].Value;
        }
    }
}
