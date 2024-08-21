using OpenQA.Selenium.Internal;

namespace Eggnine.TrashTaf.XUnit.SkipAttributes
{
    public abstract class SkipIf : Attribute
    {
        /// <summary>
        /// Returns true if the test should be skipped
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public abstract bool Matches(TrashContext ctx);
        
        /// <summary>
        /// Provides a reason that the test was skipped
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public abstract string Reason(TrashContext ctx);
    }
}
