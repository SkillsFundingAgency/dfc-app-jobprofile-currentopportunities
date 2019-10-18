using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.FindACourseClient.Models.Configuration
{
    public class CourseSearchClientSettings
    {

        public CourseSearchSvcSettings courseSearchSvcSettings { get; set; }

        public CourseSearchAuditCosmosDbSettings courseSearchAuditCosmosDbSettings { get; set; }

    }
}
