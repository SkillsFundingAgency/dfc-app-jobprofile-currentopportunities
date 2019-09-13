using Microsoft.Extensions.Configuration;
using System.Net;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ViewTests
{
    public class TestBase
    {
        private readonly string viewRootPath;
        private readonly IConfigurationRoot configuration;

        public TestBase()
        {
            var config = new ConfigurationBuilder();
            config.AddJsonFile("appsettings.json");
            configuration = config.Build();

            viewRootPath = configuration["ViewRootPath"];
        }

        public string ViewRootPath => viewRootPath;

        protected string HtmlEncode(string value)
        {
            return WebUtility.HtmlEncode(value);
        }
    }
}
