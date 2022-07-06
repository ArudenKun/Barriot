namespace Barriot.Application.Services
{
    /// <summary>
    ///     Represents a type of service that exposes <see cref="ConfigureAsync"/> and allows for member calling without injection.
    /// </summary>
    /// <remarks>
    ///     This service pattern does not allow re-injection into other services, as the global injection type is non-injectable.
    /// </remarks>
    public interface IConfigurableService
    {
        /// <summary>
        ///     Configures the service and populates injected members. Any necessary startup code flow can be executed here.
        /// </summary>
        /// <returns></returns>
        public Task ConfigureAsync();
    }
}
