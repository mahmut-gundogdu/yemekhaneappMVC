using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EuYemekApp
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
           

            IJobDetail job = JobBuilder.Create<WebPushJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(8, 25))
                    //.InTimeZone( TimeZoneInfo.FindSystemTimeZoneById() ) // istanbul ne acaba
                  )
                .Build();

            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
        }

    }
}