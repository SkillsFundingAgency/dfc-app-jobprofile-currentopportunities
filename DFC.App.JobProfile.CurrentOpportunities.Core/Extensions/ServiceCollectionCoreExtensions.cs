﻿using DFC.App.JobProfile.CurrentOpportunities.Data.HttpClientPolicies;
using DFC.App.JobProfile.CurrentOpportunities.Data.HttpClientPolicies.Polly;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Mime;

namespace DFC.App.JobProfile.CurrentOpportunities.Core.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionCoreExtensions
    {
        public static IServiceCollection AddPolicies(
            this IServiceCollection services,
            IPolicyRegistry<string> policyRegistry,
            string keyPrefix,
            CorePolicyOptions policyOptions)
        {
            if (policyOptions == null)
            {
                throw new ArgumentException("policyOptions cannot be null", nameof(policyOptions));
            }

            if (policyRegistry == null)
            {
                throw new ArgumentException("policyRegistry cannot be null", nameof(policyRegistry));
            }

            policyRegistry.Add(
                $"{keyPrefix}_{nameof(CorePolicyOptions.HttpRetry)}",
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        policyOptions.HttpRetry.Count,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.BackoffPower, retryAttempt))));

            policyRegistry.Add(
                $"{keyPrefix}_{nameof(CorePolicyOptions.HttpCircuitBreaker)}",
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: policyOptions.HttpCircuitBreaker.ExceptionsAllowedBeforeBreaking,
                        durationOfBreak: policyOptions.HttpCircuitBreaker.DurationOfBreak));

            return services;
        }

        public static IServiceCollection AddHttpClient<TClient, TImplementation, TClientOptions>(
                    this IServiceCollection services,
                    IConfiguration configuration,
                    string configurationSectionName,
                    string retryPolicyName,
                    string circuitBreakerPolicyName)

                    where TClient : class
                    where TImplementation : class, TClient
                    where TClientOptions : CoreClientOptions, new()
        {
            return services
            .Configure<TClientOptions>(configuration.GetSection(configurationSectionName))
            .AddHttpClient<TClient, TImplementation>()
            .ConfigureHttpClient((sp, options) =>
            {
                var httpClientOptions = sp
                .GetRequiredService<IOptions<TClientOptions>>()
                .Value;
                options.BaseAddress = httpClientOptions.BaseAddress;
                options.Timeout = httpClientOptions.Timeout;
                options.DefaultRequestHeaders.Clear();
                options.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    AllowAutoRedirect = false,
                };
            })
            .AddPolicyHandlerFromRegistry($"{configurationSectionName}_{retryPolicyName}")
            .AddPolicyHandlerFromRegistry($"{configurationSectionName}_{circuitBreakerPolicyName}")
            .Services;
        }
    }
}