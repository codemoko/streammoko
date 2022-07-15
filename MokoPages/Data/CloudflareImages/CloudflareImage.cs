using Newtonsoft.Json;

namespace MokoPages.Data.CloudflareImages
{
    public class CloudflareImage
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "filename")]
        public string Filename { get; set; }

        [JsonProperty(PropertyName = "uploaded")]
        public DateTime Uploaded { get; set; }

        [JsonProperty(PropertyName = "requireSignedURLs")]
        public bool RequireSignedURLs { get; set; }

        [JsonProperty(PropertyName = "variants")]
        public string[] Variants { get; set; }
    }
}
