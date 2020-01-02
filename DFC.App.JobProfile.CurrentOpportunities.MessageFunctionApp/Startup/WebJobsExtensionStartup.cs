using AutoMapper;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Extensions;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.HttpClientPolicies;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.HttpClientPolicies.Polly;
using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
using DFC.Functions.DI.Standard;
using DFC.Logger.AppInsights.Contracts;
using DFC.Logger.AppInsights.CorrelationIdProviders;
using DFC.Logger.AppInsights.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

[assembly: WebJobsStartup(typeof(DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Startup.WebJobsExtensionStartup), "Web Jobs Extension Startup")]

namespace DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Startup
{
    [ExcludeFromCodeCoverage]
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Environment.CurrentDirectory)
               .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

            builder.AddDependencyInjection();
            builder?.Services.AddAutoMapper(typeof(WebJobsExtensionStartup).Assembly);

            builder.Services.AddSingleton(configuration.GetSection(nameof(RefreshClientOptions)).Get<RefreshClientOptions>());
            builder.Services.AddSingleton(configuration.GetSection("CurrentOpportunitiesSegmentClientOptions").Get<SegmentClientOptions>());

            var policyRegistry = builder.Services.AddPolicyRegistry();
            var policyOptions = configuration.GetSection("Policies").Get<PolicyOptions>();

            builder.Services
                .AddPolicies(policyRegistry, nameof(RefreshClientOptions), policyOptions)
                .AddHttpClient<IRefreshService, RefreshService, RefreshClientOptions>(configuration, nameof(RefreshClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker))
                .AddScoped<IHttpClientService, HttpClientService>()
                .AddScoped<IMessageProcessor, MessageProcessor>()
                .AddScoped<IMappingService, MappingService>()
                .AddScoped<IMessagePropertiesService, MessagePropertiesService>()
                .AddDFCLogging(configuration["APPINSIGHTS_INSTRUMENTATIONKEY"])
                .AddScoped<ICorrelationIdProvider, InMemoryCorrelationIdProvider>()
                .AddSingleton(new HttpClient());

            var sp = builder.Services.BuildServiceProvider();
            var mapper = sp.GetService<IMapper>();

            mapper?.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}