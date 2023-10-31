namespace AuthApi.Configuration
{
    public class SecuritySettings
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string Secret { get; set; }
    }
}
