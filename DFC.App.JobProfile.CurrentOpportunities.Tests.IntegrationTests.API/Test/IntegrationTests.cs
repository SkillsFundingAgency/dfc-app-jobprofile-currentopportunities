using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Model.APIResponse;
using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Model.ServiceBusMessage;
using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support;
using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.API;
using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.API.RestFactory;
using DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.ServiceBus.ServiceBusFactory;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Test
{
    public class IntegrationTests : SetUpAndTearDown
    {
        private CurrentOpportunitiesAPI currentOpportunitiesAPI;

        [SetUp]
        public void SetUp()
        {
            this.currentOpportunitiesAPI = new CurrentOpportunitiesAPI(new RestClientFactory(), new RestRequestFactory(), this.AppSettings);
        }

        [Test]
        public async Task JobProfileSocMessageUpdatesTheResponseFromCurrentOpportunitiessAPI()
        {
            var socCode = this.CommonAction.RandomString(5);
            var jobprofileSoc = new JobProfileSoc()
            {
                Id = Guid.NewGuid().ToString(),
                SOCCode = socCode,
                Description = "This is an automated SOC code",
                ONetOccupationalCode = this.CommonAction.RandomString(10),
                UrlName = socCode,
                ApprenticeshipFramework = new List<ApprenticeshipFramework>() { new ApprenticeshipFramework() {
                    Id = Guid.NewGuid().ToString(),
                    Description = "This is an automated apprenticeship framework",
                    Title = "This is an automated apprenticeship framework title",
                    Url = $"https://{this.CommonAction.RandomString(10)}.com/",
                } },
                ApprenticeshipStandards = new List<Model.ServiceBusMessage.ApprenticeshipStandard>() { new Model.ServiceBusMessage.ApprenticeshipStandard() {
                    Id = Guid.NewGuid().ToString(),
                    Description = "This is an automated apprenticeship standard",
                    Title = "This is an automated apprenticeship standard title",
                    Url = $"https://{this.CommonAction.RandomString(10)}.com/",
                } },
                JobProfileId = this.JobProfile.JobProfileId,
                JobProfileTitle = this.JobProfile.Title,
                Title = "This is an automated SOC code title"
            };

            var messageBody = this.CommonAction.ConvertObjectToByteArray(jobprofileSoc);
            var message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Published", "JobProfileSoc");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(true);

            var response = await this.currentOpportunitiesAPI.GetById<CurrentOpportunitiesAPIResponse>(this.JobProfile.JobProfileId).ConfigureAwait(true);
            Assert.AreEqual(jobprofileSoc.ApprenticeshipFramework[0].Id, response.Data.apprenticeships.frameworks[0].Id);
        }

        [Test]
        public async Task ApprenticeshipStandardsMessageUpdatesTheResponseFromCurrentOpportunitiessAPI()
        {
            var apprenticeshipStandard = new ApprenticeshipStandards()
            {
                Id = this.JobProfile.SocCodeData.ApprenticeshipStandards[0].Id,
                Description = "This is an updated apprenticeship standard",
                JobProfileId = this.JobProfile.JobProfileId,
                JobProfileTitle = this.JobProfile.Title,
                SOCCode = this.JobProfile.SocCodeData.SOCCode,
                SOCCodeClassificationId = this.JobProfile.SocCodeData.Id,
                Title = "This is an updated apprenticeship standard title",
                Url = $"https://{this.CommonAction.RandomString(10)}.com/",
            };

            var messageBody = this.CommonAction.ConvertObjectToByteArray(apprenticeshipStandard);
            var message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Published", "apprenticeship-standards");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(true);

            var response = await this.currentOpportunitiesAPI.GetById<CurrentOpportunitiesAPIResponse>(this.JobProfile.JobProfileId).ConfigureAwait(true);
            Assert.AreEqual(apprenticeshipStandard.Description, response.Data.apprenticeships.standards[0].description);
        }

        [Test]
        public async Task ApprenticeshipFrameworksMessageUpdatesTheResponseFromCurrentOpportunitiessAPI()
        {
            var apprenticeshipFramework = new ApprenticeshipFrameworks()
            {
                Id = this.JobProfile.SocCodeData.ApprenticeshipFramework[0].Id,
                Description = "This is an updated apprenticeship framework",
                JobProfileId = this.JobProfile.JobProfileId,
                JobProfileTitle = this.JobProfile.Title,
                SOCCode = this.JobProfile.SocCodeData.SOCCode,
                SOCCodeClassificationId = this.JobProfile.SocCodeData.Id,
                Title = "This is an updated apprenticeship framework title",
                Url = $"https://{this.CommonAction.RandomString(10)}.com/",
            };

            var messageBody = this.CommonAction.ConvertObjectToByteArray(apprenticeshipFramework);
            var message = new MessageFactory().Create(this.JobProfile.JobProfileId, messageBody, "Published", "apprenticeship-frameworks");
            await this.ServiceBus.SendMessage(message).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(true);

            var response = await this.currentOpportunitiesAPI.GetById<CurrentOpportunitiesAPIResponse>(this.JobProfile.JobProfileId).ConfigureAwait(true);
            Assert.AreEqual(apprenticeshipFramework.Description, response.Data.apprenticeships.frameworks[0].Description);
        }
    }
}