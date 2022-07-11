using Newtonsoft.Json;

namespace WorkerTest.Objects
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Image
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("uploaded")]
        public DateTime Uploaded { get; set; }

        [JsonProperty("requireSignedURLs")]
        public bool RequireSignedURLs { get; set; }

        [JsonProperty("variants")]
        public List<string> Variants { get; set; }
    }

    public class Result
    {
        [JsonProperty("images")]
        public List<Image> Images { get; set; }
    }

    public class CloudflareImageResponse
    {
        [JsonProperty("result")]
        public Result Result { get; set; }

        [JsonProperty("result_info")]
        public object ResultInfo { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("errors")]
        public List<object> Errors { get; set; }

        [JsonProperty("messages")]
        public List<object> Messages { get; set; }
    }

}
