namespace TeamSpeakNotifier
{
    public class AppSettings
    {
        public string Environment { get; set; }
        public int JobInterval { get; set; }
        public string TeamSpeakServerIp { get; set; }
        public int TeamSpeakServerPort { get; set; }
        public string TeamSpeakUserName { get; set; }
        public string TeamSpeakPassword { get; set; }
        public string[] ToEmails { get; set; }
        public string FromEmail { get; set; }
        public string EmailServer { get; set; }
        public int EmailServerPort { get; set; }
        public string EmailServerUsername { get; set; }
        public string EmailServerPassword { get; set; }
        public bool EmailServerEnableSsl { get; set; }
        public bool EmailEnabled { get; set; }
    }
}