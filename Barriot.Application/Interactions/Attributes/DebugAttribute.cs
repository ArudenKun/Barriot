namespace Barriot.Application.Interactions.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DebugAttribute : Attribute
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="DebugAttribute"/> class which will be handled at runtime for module creation and actively logging command execution flow.
        /// </summary>
        public DebugAttribute()
        {

        }
    }
}
