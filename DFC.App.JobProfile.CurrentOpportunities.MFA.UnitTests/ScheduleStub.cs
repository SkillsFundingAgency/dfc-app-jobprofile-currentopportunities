using Microsoft.Azure.WebJobs.Extensions.Timers;
using System;

namespace DFC.App.JobProfile.CurrentOpportunities.MFA.UnitTests
{
    public class ScheduleStub : TimerSchedule
    {  
        public override DateTime GetNextOccurrence(DateTime now)
        {
            throw new NotImplementedException();
        }
    }
}
