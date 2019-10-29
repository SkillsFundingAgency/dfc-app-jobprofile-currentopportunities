using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.CurrentOpportunities.Data.ServiceBusModels.PatchModels
{
    public class PatchSocDataServiceBusModel : BaseJobProfileMessage
    {
        public Guid Id { get; set; }

        public string SocCode { get; set; }

        public string Description { get; set; }

        public string ONetOccupationalCode { get; set; }

        public string UrlName { get; set; }
    }
}