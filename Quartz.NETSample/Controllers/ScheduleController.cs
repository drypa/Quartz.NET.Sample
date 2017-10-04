using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Quartz.NETSample.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly ScheduleManager _scheduleManager;

        public ScheduleController(ScheduleManager scheduleManager)
        {
            _scheduleManager = scheduleManager;
        }

        [HttpPost]
        public async Task Update([FromBody]UpdateParameters parameters)
        {
            await _scheduleManager.Update(parameters.IsEnabled, parameters.IntervalInSeconds, DateTimeOffset.FromUnixTimeMilliseconds(parameters.StartDate));
        }
    }

    public class UpdateParameters
    {
        public bool IsEnabled { get; set; }
        public int IntervalInSeconds { get; set; }
        public long StartDate { get; set; }
    }
}