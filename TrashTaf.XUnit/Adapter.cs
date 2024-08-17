// TrashTaf © 2024 by RF@EggNine.com All Rights Reserved
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;

namespace TrashTaf.XUnit
{
    public class Adapter
    {
        public Adapter() { }

        public void ExecuteTest(Action<TrashContext, Postman, WebDriver> test)
        {
            var ctx = new TrashContext();
            var stackTrace = new StackTrace();
            ctx.TestName = stackTrace.GetFrame(2).GetMethod().Name;
            ctx.ClassName = stackTrace.GetFrame(2).GetMethod().ReflectedType.Name;
            ctx.TestCaseId = (int)stackTrace.GetFrame(2).GetMethod().CustomAttributes.First(a => a.AttributeType == typeof(TestCase)).ConstructorArguments[0].Value;
            var postman = new Postman();
            WebDriver webDriver = new ChromeDriver();
            test(ctx, postman, webDriver);
        }

        public static void Execute(Action<TrashContext, Postman, WebDriver> test) => new Adapter().ExecuteTest(test);
    }
}
