using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DFC.App.JobProfile.CurrentOpportunities.Tests.IntegrationTests.API.Support.CommonActions
{
    public class CommonAction
    {
        private static readonly Random Random = new Random();

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public byte[] ConvertObjectToByteArray(object obj)
        {
            string serialisedContent = JsonConvert.SerializeObject(obj);
            return Encoding.ASCII.GetBytes(serialisedContent);
        }

        public T GetResource<T>(string resourceName)
        {
            var resourcesDirectory = Directory.CreateDirectory(Environment.CurrentDirectory).GetDirectories("Resource")[0];
            var files = resourcesDirectory.GetFiles();
            var selectedResource = files.FirstOrDefault(file => file.Name.ToUpperInvariant().StartsWith(resourceName.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase));
            if (selectedResource == null)
            {
                throw new Exception($"No resource with the name {resourceName} was found");
            }

            using var streamReader = new StreamReader(selectedResource.FullName);
            var content = streamReader.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
