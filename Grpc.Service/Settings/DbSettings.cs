namespace Grpc.Service.Settings
{
    public class DbSettings
    {
        public const string SectionName = "DbCredentials:SqlServer";

        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Database { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? ConnectionString { get; set; }
    }
}
