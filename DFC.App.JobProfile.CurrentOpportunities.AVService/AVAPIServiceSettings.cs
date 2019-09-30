using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.CurrentOpportunities.AVService
{
    public class AVAPIServiceSettings
    {
        public string FAAEndPoint { get; set; }

        public string FAASubscriptionKey { get; set; }

        public int FAAPageSize { get; set; }

        public int FAAMaxPagesToTryPerMapping { get; set; }

        public string FAASortBy { get; set; }
    }
}
