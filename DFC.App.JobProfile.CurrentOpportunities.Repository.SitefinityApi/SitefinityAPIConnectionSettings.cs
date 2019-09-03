using System;

namespace DFC.App.CareerPath.Repository.SitefinityApi
{
    public class SitefinityAPIConnectionSettings
    {
        public string AuthTokenEndpoint { get; set; }

        public string SitefinityApiUrlBase { get; set; }

        public string SitefinityApiDataEndpoint { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Scopes { get; set; }
    }
}