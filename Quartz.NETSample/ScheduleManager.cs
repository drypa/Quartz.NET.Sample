using System;
using System.Runtime.InteropServices.ComTypes;
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
        //private ITrigger _trigger;
        private static JobKey jobKey = JobKey.Create("job");
        private static TriggerKey triggerKey = new TriggerKey("trigger");
        private IJobDetail _job = JobBuilder.Create<SimpleJob>().WithIdentity(jobKey).Build();

        public async Task Update(bool isEnabled, int intervalInSeconds, DateTimeOffset startDate)
        {
            var needChangeEnabled = _isEnabled != isEnabled;
            _isEnabled = isEnabled;
            var needChangeSchedule = _intervalInSeconds != intervalInSeconds || _startDate != startDate;

            if (needChangeEnabled)
            {
                if (isEnabled)
                {
                    await _scheduler.ResumeJob(jobKey);
                }
                else
                {
                    await _scheduler.PauseJob(jobKey);
                }
            }
            if (!isEnabled)
            {
                return;
            }
            _intervalInSeconds = intervalInSeconds;
            _startDate = startDate;
            if (needChangeSchedule)
            {
                var trigger = await _scheduler.GetTrigger(triggerKey);
                TriggerBuilder triggerBuilder = trigger.GetTriggerBuilder();
                var newTrigger = CreateTrigger(triggerBuilder);

                await _scheduler.RescheduleJob(triggerKey, newTrigger);
            }

        }

        public ScheduleManager()
        {
            _scheduler = CreateScheduler().GetAwaiter().GetResult();
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
            var trigger = CreateTrigger(TriggerBuilder.Create());

            await scheduler.ScheduleJob(_job, trigger);
            return scheduler;
        }

        private ITrigger CreateTrigger(TriggerBuilder triggerBuilder)
        {
            return triggerBuilder
                .StartAt(_startDate)
                .WithIdentity(triggerKey)
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