﻿namespace Barriot.Application.Services.Entities
{
    public class PollService : IConfigurableService
    {
        private readonly System.Timers.Timer _timer;

        public PollService()
        {
            _timer = new(5000)
            {
                AutoReset = true,
                Enabled = true,
            };
        }

        public async Task ConfigureAsync()
        {
            _timer.Elapsed += async (_, x) => await OnElapsedAsync(x);
            _timer.Start();
            await Task.CompletedTask;
        }

        private async Task OnElapsedAsync(System.Timers.ElapsedEventArgs e)
        {
            await PollEntity.DeleteManyAsync();
        }
    }
}
