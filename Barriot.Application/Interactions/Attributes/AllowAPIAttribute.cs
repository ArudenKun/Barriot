namespace Barriot.Application.Interactions.Attributes
{
    /// <summary>
    ///     Represents an attribute that configures if a method is allowed to make API calls.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class AllowAPIAttribute : Attribute
    {
        public bool AllowAPI { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="AllowAPIAttribute"/> with provided value of <paramref name="allowApiCalls"/>.
        /// </summary>
        /// <param name="allowApiCalls">Defines if the command in question is allowed to make API calls.</param>
        public AllowAPIAttribute(bool allowApiCalls)
        {
            AllowAPI = allowApiCalls;
        }
    }
}
