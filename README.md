# Digital First Careers - Current Oppotunities app

This project provides a Current Oppotunities app for use in the Job Profiles app, which is a part of the Composite UI (Shell application) to dynamically output markup from CarCurrent Oppotunities data sources.

Details of the Job Profiles application may be found here https://github.com/SkillsFundingAgency/dfc-app-jobprofiles

Details of the Composite UI application may be found here https://github.com/SkillsFundingAgency/dfc-composite-shell

This Current Oppotunities app runs in two flavours:

* Current Oppotunities segments
* Draft Current Oppotunities segments

## Getting Started

This is a self-contained Visual Studio 2019 solution containing a number of projects (web application, service and repository layers, with associated unit test and integration test projects).

### Installing

Clone the project and open the solution in Visual Studio 2019.

## List of dependencies

|Item	|Purpose|
|-------|-------|
|Sitefinity |Content management system |
|Azure Cosmos DB | Document storage |
|Find an Apprenticeship|API to extract available apprenticeships|
|Find a Course|Service to search and extract courses|

## Local Config Files

Once you have cloned the public repo you need to rename the appsettings files by removing the -template part from the configuration file names listed below.

| Location | Repo Filename | Rename to |
|-------|-------|-------|
| DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp | local.settings-template.json | local.settings.json |
| DFC.App.CurrentOppotunities.IntegrationTests | appsettings-template.json | appsettings.json |
| DFC.App.CurrentOppotunities | appsettings-template.json | appsettings.json |

## Configuring to run locally

The project contains a number of "appsettings-template.json" / "local.settings.json" files which contain sample appsettings for the web app and the integration test projects. To use these files, copy them to "appsettings.json" / "local.settings.json" and edit and replace the configuration item values with values suitable for your environment.

By default, the appsettings include a local Azure Cosmos Emulator configuration using the well known configuration values. These may be changed to suit your environment if you are not using the Azure Cosmos Emulator. 
In addition -

### Sitefinity configuration settings will need to be edited.

|File                                       |Setting                        |Example value                      |
|------------------------------------------|------------------------------|----------------------------------|
| appsettings.json     | SitefinityApi.AuthTokenEndpoint      |< your domain authentication endpoint >  |
| appsettings.json     | SitefinityApi.SitefinityApiUrlBase     |http://< your domain api base endpoint >  |
| appsettings.json     | SitefinityApi.SitefinityApiDataEndpoint |< your domain api data endpoint >  |
| appsettings.json     | SitefinityApi.ClientId           | < can be obtained from sitefinity  >|
| appsettings.json     | SitefinityApi.ClientSecret       | < generate it with sitefinity >     |
| appsettings.json     | SitefinityApi.Username           | < sitefinity username >             |
| appsettings.json     | SitefinityApi.Password           | < sitefinity password >             |
| appsettings.json     | SitefinityApi.Scopes             | < authentication protocol> OpenId   |

### Function app settings will need to be edited.

|File                                       |Setting                        |Example value                      |
|------------------------------------------|------------------------------|----------------------------------|
| local.settings.json     | Values.RefreshApprenticeshipsCron      | "0 0 3 * * *" |
| local.settings.json     | Values.RefreshCoursesCron      | "0 0 5 * * *" |
| local.settings.json     | RefreshClientOptions.BaseAddress      | https://localhost (url to local DFC.App.JobProfile.CurrentOpportunities app |

### Course search client settings will need to be edited.

|File                                       |Setting                        |Example value                      |
|------------------------------------------|------------------------------|----------------------------------|
| appsettings.json     | CourseSearchSvc.APIKey      | 55e116d6-2f64-47ae-b753-468ed36d7827 |

### Apprenticeship API configuration settings will need to be edited.

|File                                       |Setting                        |Example value                      |
|------------------------------------------|------------------------------|----------------------------------|
| appsettings.json     | AVAPIService.FAAEndPoint      |https://apprenticeships.gov.uk/apprenticeships  |
| appsettings.json     | AVAPIService.FAASubscriptionKey      | 62cd0465d3e243b768ed87843bb4c5c3  |

## Running locally

To run this product locally, you will need to configure the list of dependencies. Once configured and the configuration files updated, it should be F5 to run and debug locally. The application can be run using IIS Express or full IIS.

To run the project, start the web application. Once running, browse to the main entrypoint which is the "https://localhost:44322/segments". This will list all of the Current Oppotunities segments available and from here, you can navigate to the individual Current Oppotunities segments.

The Current Oppotunities app is designed to be run from within the Job Profiles app, which in turn is run from within the Composite UI, therefore running the Current Oppotunities app outside of the other apps will only show simple views of the data.

## Deployments

This Current Oppotunities app will be deployed as an individual deployment for consumption by the Composite UI.

## Assets

CSS, JS, images and fonts used in this site can found in the following repository https://github.com/SkillsFundingAgency/dfc-digital-assets

## Built With

* Microsoft Visual Studio 2019
* .Net Core 2.2

## References

Please refer to https://github.com/SkillsFundingAgency/dfc-digital for additional instructions on configuring individual components like Sitefinity and Cosmos.

