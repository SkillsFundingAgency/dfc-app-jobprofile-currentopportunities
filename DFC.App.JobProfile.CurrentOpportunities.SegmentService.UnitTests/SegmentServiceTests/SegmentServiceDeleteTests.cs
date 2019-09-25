﻿using DFC.App.JobProfile.CurrentOpportunities.Data.Contracts;
using DFC.App.JobProfile.CurrentOpportunities.Data.Models;
using DFC.App.JobProfile.CurrentOpportunities.DraftSegmentService;
using FakeItEasy;
using System;
using System.Net;
using Xunit;

namespace DFC.App.JobProfile.CurrentOpportunities.SegmentService.UnitTests.SegmentServiceTests
{
    [Trait("Segment Service", "Delete Tests")]
    public class SegmentServiceDeleteTests
    {
        private readonly ICosmosRepository<CurrentOpportunitiesSegmentModel> repository;
        private readonly IDraftCurrentOpportunitiesSegmentService draftCurrentOpportunitiesSegmentService;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;

        public SegmentServiceDeleteTests()
        {
            repository = A.Fake<ICosmosRepository<CurrentOpportunitiesSegmentModel>>();
            draftCurrentOpportunitiesSegmentService = A.Fake<DraftCurrentOpportunitiesSegmentService>();
            currentOpportunitiesSegmentService = new CurrentOpportunitiesSegmentService(repository, draftCurrentOpportunitiesSegmentService);
        }

        [Fact]
        public void CareerPathSegmentServiceDeleteReturnsSuccessWhenSegmentDeleted()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            const int partitionKey = 0;
            var expectedResult = true;

            A.CallTo(() => repository.DeleteAsync(documentId, partitionKey)).Returns(HttpStatusCode.NoContent);

            // act
            var result = currentOpportunitiesSegmentService.DeleteAsync(documentId, partitionKey).Result;

            // assert
            A.CallTo(() => repository.DeleteAsync(documentId, partitionKey)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void CareerPathSegmentServiceDeleteReturnsNullWhenSegmentNotDeleted()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            const int partitionKey = 0;
            var expectedResult = false;

            A.CallTo(() => repository.DeleteAsync(documentId, partitionKey)).Returns(HttpStatusCode.BadRequest);

            // act
            var result = currentOpportunitiesSegmentService.DeleteAsync(documentId, partitionKey).Result;

            // assert
            A.CallTo(() => repository.DeleteAsync(documentId, partitionKey)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void CareerPathSegmentServiceDeleteReturnsFalseWhenMissingRepository()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            const int partitionKey = 0;
            var careerPathSegmentModel = A.Fake<CurrentOpportunitiesSegmentModel>();
            var expectedResult = false;

            A.CallTo(() => repository.DeleteAsync(documentId, partitionKey)).Returns(HttpStatusCode.FailedDependency);

            // act
            var result = currentOpportunitiesSegmentService.DeleteAsync(documentId, partitionKey).Result;

            // assert
            A.CallTo(() => repository.DeleteAsync(documentId, partitionKey)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}