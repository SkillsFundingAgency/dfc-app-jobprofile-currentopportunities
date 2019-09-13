using System.Collections.Generic;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ViewTests
{
    public interface IViewRenderer
    {
        string Render(string templateValue, object model, IDictionary<string, object> viewBag);
    }
}
