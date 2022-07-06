namespace Barriot.Application.Services
{
    public class ServiceActivator
    {
        private readonly IEnumerable<IConfigurableService> _services;

        public ServiceActivator(IEnumerable<IConfigurableService> services)
        {
            _services = services;
        }

        public async Task ActivateAsync()
        {
            foreach (var service in _services)
                await service.ConfigureAsync();
        }
    }
}
