using System;
using System.Threading.Tasks;
using Quartz.Impl;

namespace Quartz.NETSample
{
    public sealed class ScheduleManager
    {
        private int _intervalInSeconds = 10;
        private bool _isEnabled = true;
        private IScheduler _scheduler;
        private DateTimeOffset _startDate = DateTimeOffset.UtcNow.AddYears(-20);


        public async Task Update(bool isEnabled, int intervalInSeconds, DateTimeOffset startDate)
        {
            _isEnabled = isEnabled;
            _intervalInSeconds = intervalInSeconds;
            _startDate = startDate;
            await TriggerScheduler();
        }

        public async Task TriggerScheduler()
        {
            var scheduler = await GetScheduler();
            if (_isEnabled)
            {
                
                await Console.Out.WriteLineAsync("Let's work!");

                await scheduler.Start();
            }
            else
            {
                await Console.Out.WriteLineAsync("Nothing to do!");
                await scheduler.Shutdown();
            }
        }

        private async Task<IScheduler> GetScheduler()
        {
            if (_scheduler == null)
            {
                var factory = new StdSchedulerFactory();
                _scheduler = await factory.GetScheduler();
            }

            await _scheduler.Clear();
            var job = JobBuilder.Create<SimpleJob>().Build();
            var trigger = TriggerBuilder.Create()
                .StartAt(_startDate)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(_intervalInSeconds)
                    .RepeatForever())
                .Build();

            await _scheduler.ScheduleJob(job, trigger);
            return _scheduler;
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