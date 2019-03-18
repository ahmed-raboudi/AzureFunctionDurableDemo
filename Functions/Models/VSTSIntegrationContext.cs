namespace SkillsBundle.Function.Models
{

    public class VSTSIntegrationContext
    {
        public string UserOID { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string AzureLocation { get; set; }
        public string VstsInstance { get; set; }
        public string VstsPAT { get; set; }
        public string SonarToken { get; set; }
        public string ChallengeSourceGitUrl { get; set; }
        public string ChallengeSourceGitEndPointId { get; set; }
        public string SonarEndPointId { get; set; }
        public string ChallengeName { get; set; }
        public string ChallengeProjectName
        {
            get
            {
                var sufix = UserOID.Split('-')[0];
                return ChallengeName + sufix;
            }
        }
        public string ChallengeProjectDescription { get; set; }
        public string SourceControlType { get; set; }
        public string TemplateTypeId { get; set; }
        public string VstsUserId { get; set; }
        public string ChallengeCreatedProjectId { get; set; }
    }
}