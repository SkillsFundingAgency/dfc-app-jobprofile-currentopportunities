namespace DFC.App.JobProfile.CurrentOpportunities.Data.Models.PatchModels
{
    public class PatchSocDataModel : BasePatchModel
    {
        public string SocCode { get; set; }

        public string Description { get; set; }

        public string ONetOccupationalCode { get; set; }

        public string UrlName { get; set; }
    }
}