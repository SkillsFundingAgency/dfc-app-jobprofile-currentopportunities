using DFC.App.FindACourseClient;
using DFC.App.FindACourseClient.Models;
using DFC.App.FindACourseClient.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DFC.App.FindACourseClient
{
    internal static class MessageConverter
    {
        internal static CourseListInput GetCourseListRequest(string request, CourseSearchSvcSettings courseSearchSvcSettings)
        {
            var apiRequest = new CourseListInput
            {
                CourseListRequest = new CourseListRequestStructure
                {
                    CourseSearchCriteria = new SearchCriteriaStructure
                    {
                        APIKey = courseSearchSvcSettings.ApiKey,
                        SubjectKeyword = request,
                        EarliestStartDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        AttendanceModes = courseSearchSvcSettings.AttendanceModes?.Split(',')
                    },
                    RecordsPerPage = courseSearchSvcSettings.SearchPageSize,
                    PageNo = "1",
                    SortBy = SortType.A,
                    SortBySpecified = true
                }
            };

            return apiRequest;
        }

        internal static IEnumerable<CourseSumary> ConvertToCourse(this CourseListOutput apiResult)
        {
            var result = apiResult?.CourseListResponse?.CourseDetails?.Select(c =>
                new CourseSumary
                {
                    Title = c.Course.CourseTitle,
                    Provider = c.Provider.ProviderName,
                    StartDate = Convert.ToDateTime(c.Opportunity.StartDate.Item),
                    CourseId = c.Course.CourseID,
                    Location = new CourseLocation()
                    {
                        Town = (c.Opportunity.Item as VenueInfo)?.VenueAddress.Town,
                        PostCode = (c.Opportunity.Item as VenueInfo)?.VenueAddress.PostCode,
                    } 
                });

            return result ?? Enumerable.Empty<CourseSumary>();
        }


    }
}
