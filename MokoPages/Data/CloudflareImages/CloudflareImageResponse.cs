using Newtonsoft.Json;

namespace MokoPages.Data.CloudflareImages
{
    public class CloudflareImageResponse
    {
        [JsonProperty("result")]
        public CloudflareImagePage Result { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("errors")]
        public List<object> Errors { get; set; }

        [JsonProperty("messages")]
        public List<object> Messages { get; set; }

        public static async Task<CloudflareImagePage> GetCloudflareImages(string cloudflareToken, string accountId, int pageNumber)
        {
            var cfImgPage = new CloudflareImagePage
            {
                PageNumber = pageNumber
            };
            HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cloudflareToken);
            var resultStr = await httpClient.GetStringAsync($"https://api.cloudflare.com/client/v4/accounts/{accountId}/images/v1?page={pageNumber}&per_page={CloudflareImagePage.PAGE_SIZE}");
            var result = JsonConvert.DeserializeObject<CloudflareImageResponse>(resultStr);
            if (result.Success)
            {
                cfImgPage.Images = result?.Result?.Images;
                cfImgPage.PageNumber = pageNumber;
            }
            return cfImgPage;
        }
    }
}
