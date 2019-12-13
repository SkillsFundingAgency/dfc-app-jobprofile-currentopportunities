using Microsoft.Azure.WebJobs.Extensions.Timers;
using System;

namespace DFC.App.JobProfile.CurrentOpportunities.MFA.UnitTests
{
    public class ScheduleStub : TimerSchedule
    {
        public override bool AdjustForDST => throw new NotImplementedException();

        public override DateTime GetNextOccurrence(DateTime now)
        {
            throw new NotImplementedException();
        }
    }
}
