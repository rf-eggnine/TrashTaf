namespace Eggnine.TrashTaf.XUnit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Priority : Attribute
    {
        public Priority(int priority)
        {

        }
    }
}
