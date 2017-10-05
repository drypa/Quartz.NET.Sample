using System;
using System.Threading.Tasks;
using Quartz.Impl;

namespace Quartz.NETSample
{
    public sealed class ScheduleManager
    {
        private static readonly TriggerKey TriggerKey = new TriggerKey("trigger");
        private readonly IJobDetail _job = JobBuilder.Create<SimpleJob>().Build();
        private int _intervalInSeconds = 10;
        private bool _isEnabled = true;
        private IScheduler _scheduler;
        private DateTimeOffset _startDate = DateTimeOffset.UtcNow.AddYears(-20);

        public ScheduleManager()
        {
            _scheduler = CreateScheduler().GetAwaiter().GetResult();
        }

        public async Task Update(bool isEnabled, int intervalInSeconds, DateTimeOffset startDate)
        {
            _isEnabled = isEnabled;
            _intervalInSeconds = intervalInSeconds;
            _startDate = startDate;
            if (!isEnabled)
            {
                await StopScheduler();
                return;
            }

            if (_scheduler.IsShutdown)
            {
                _scheduler = await CreateScheduler();
            }
            else
            {
                var trigger = CreateTrigger();
                await _scheduler.RescheduleJob(trigger.Key, trigger);
            }

            await _scheduler.Start();
        }

        private async Task StopScheduler()
        {
            if (_scheduler.IsShutdown)
                return;
            await _scheduler.Shutdown(true);
        }

        public async Task TriggerScheduler()
        {
            if (_isEnabled)
            {
                await Console.Out.WriteLineAsync("Let's work!");
                await _scheduler.Start();
            }
            else
            {
                await Console.Out.WriteLineAsync("Nothing to do!");
                await _scheduler.Shutdown();
            }
        }


        private async Task<IScheduler> CreateScheduler()
        {
            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();
            var trigger = CreateTrigger();
            await scheduler.ScheduleJob(_job, trigger);
            return scheduler;
        }

        private ITrigger CreateTrigger()
        {
            return TriggerBuilder.Create()
                .StartAt(_startDate)
                .WithIdentity(TriggerKey)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(_intervalInSeconds)
                    .RepeatForever())
                .Build();
        }
    }

    public class SimpleJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync("Greetings from SimpleJob!");
        }
    }
}