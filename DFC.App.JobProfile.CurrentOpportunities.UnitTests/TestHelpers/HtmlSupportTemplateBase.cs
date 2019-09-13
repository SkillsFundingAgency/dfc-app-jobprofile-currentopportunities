using RazorEngine.Templating;

namespace DFC.App.JobProfile.CurrentOpportunities.UnitTests.ViewTests
{
    public class HtmlSupportTemplateBase<T> : TemplateBase<T>
    {
        public HtmlSupportTemplateBase()
        {
            Html = new RazorHtmlHelper();
        }

        public RazorHtmlHelper Html { get; set; }
    }
}
