namespace SkillsBundle.Function.Models
{
    public class VSTSProject
    {
        public string Id { get; set; }
        public string VstsInstance { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SourceControlType { get; set; } = "Git";
        public string TemplateTypeId { get; set; } = "6b724908-ef14-45cf-84f8-768b5384da45";
    }
}