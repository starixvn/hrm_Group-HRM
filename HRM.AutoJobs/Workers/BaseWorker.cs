namespace HRM.AutoJobs.Workers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Cronos;
    using Models;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class BaseWorker : BackgroundService
    {
        private readonly ILogger logfile;
        private readonly Fanex.Logging.ILogger logger;
        protected JobSetting jobSetting;

        public BaseWorker(ILogger<BaseWorker> logfile, Fanex.Logging.ILogger logger)
        {
            this.logfile = logfile;
            this.logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logfile.LogInformation($"{jobSetting.JobName}: Start");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var cronExpression = CronExpression.Parse(jobSetting.CronExpression);
                var currentUtcTime = DateTimeOffset.UtcNow.UtcDateTime;
                var occurenceTime = cronExpression.GetNextOccurrence(currentUtcTime);

                var delay = occurenceTime.GetValueOrDefault() - currentUtcTime;
                await Task.Delay(delay, stoppingToken);

                await RunJobAsync();
            }
        }

        private async Task RunJobAsync()
        {
            try
            {
                if (jobSetting.Enabled)
                {
                    logfile.LogInformation($"{jobSetting.JobName}: Doing...");
                    await DoWorkAsync();
                    logfile.LogInformation($"{jobSetting.JobName}: Done.");
                }
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync($"{jobSetting.JobName}: Error - {ex}");
            }
        }

        public virtual async Task DoWorkAsync()
        {
            await Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logfile.LogInformation($"{jobSetting.JobName}: Stop");

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            logfile.LogInformation($"{jobSetting.JobName}: Disposed");
            GC.SuppressFinalize(this);
            base.Dispose();
        }
    }
}