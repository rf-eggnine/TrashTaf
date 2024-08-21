namespace Eggnine.TrashTaf.XUnit.SkipAttributes
{
    public class SkipIfBrowserIsNot : SkipIf
    {
        public SkipIfBrowserIsNot(string browser)
        {
            Browser = browser;
        }

        public string Browser { get; }

        public override bool Matches(TrashContext ctx)
        {
            return string.Equals(Browser, ctx.BrowserName);
        }

        public override string Reason(TrashContext ctx)
        {
            return $"BrowserName is {ctx.BrowserName} and not {Browser}";
        }
    }
}
