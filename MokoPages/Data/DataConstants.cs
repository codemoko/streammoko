namespace MokoPages.Data
{
    public static class DataConstants
    {
        public static readonly string COSMOS_DB_ID = "StreamMokoDb";
        public static readonly string ENDPOINT_URI = Environment.GetEnvironmentVariable("EndpointUri");
        public static readonly string PRIMARY_KEY = Environment.GetEnvironmentVariable("PrimaryKey");
        public static readonly string CLOUDFLARE_TOKEN = Environment.GetEnvironmentVariable("CloudflareToken");
        public static readonly string CLOUDFLARE_ACCOUNT_ID = Environment.GetEnvironmentVariable("CloudflareAccountId");
    }
}
