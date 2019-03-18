namespace SkillsBundle.Function.Models
{

    public class VSTSLongrunningOperationContext
    {
        public string OperationFriendlyName { get; set; }
        public string Url { get; set; }
        public string VstsInstance { get; set; }
        public string VstsPAT { get; set; }

        public int MaxRetryCount { get; set; }
        public int CheckIntervalInSeconds { get; set; }
    }
}