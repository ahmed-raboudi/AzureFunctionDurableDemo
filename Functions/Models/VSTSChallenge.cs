namespace SkillsBundle.Function.Models
{

    public class VSTSChallenge
    {        
        public string ChallengeSourceGitUrl { get; set; }
        public string ChallengeSourceGitEndPointId { get; set; }
        public string ChallengeSourceGitPAT { get; set; } = "dftsfgin3hiw4gliq3faeh5tisqnxc2s3666xlpmhkr3ead4ux4q";
        public string ChallengeProjectName { get; set; }
        public string ChallengeProjectDescription { get; set; }
        public string SourceControlType { get; set; }
        public string TemplateTypeId { get; set; }
    }
}