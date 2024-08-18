// TrashTaf © 2024 by RF@EggNine.com All Rights Reserved

namespace TrashTaf.XUnit
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
        public Exception Exception;
        public bool IsHeadless;
    }
}
