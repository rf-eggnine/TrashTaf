// TrashTaf © 2024 by RF@EggNine.com All Rights Reserved

using System.Reflection;

namespace Eggnine.TrashTaf.XUnit
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
        public string Username;
        public string Password;
        public Exception Exception;
        public bool IsHeadless;

        public int Priority { get; internal set; }

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
