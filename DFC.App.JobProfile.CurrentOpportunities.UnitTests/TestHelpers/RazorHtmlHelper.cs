using RazorEngine.Text;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ViewTests
{
    public class RazorHtmlHelper
    {
        public IEncodedString Raw(string rawString)
        {
            return new RawString(rawString);
        }
    }
}
