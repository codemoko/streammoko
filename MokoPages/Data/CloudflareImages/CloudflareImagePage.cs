using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Net;

namespace MokoPages.Data.CloudflareImages
{
    public class CloudflareImagePage : ICosmosContainerRoot<CloudflareImagePage>
    {
        public static readonly string PARTITION_KEY = "/partitionKey";
        public static readonly int PAGE_SIZE = 100;
        private static readonly string CONTAINER = "CFImages";


        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey { get; set; } = "CFIMG100";

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty(PropertyName = "pageNumber")]
        public int PageNumber { get; set; }

        [JsonProperty(PropertyName = "images")]
        public CloudflareImage[] Images { get; set; }

        [JsonIgnore]
        public Container DbContainer { get; set; }

        public static async Task<CloudflareImagePage[]> GetAllAsync()
        {
            List<CloudflareImagePage> pageList = new();

            var sqlQueryText = "SELECT * FROM CFImages";
            QueryDefinition queryDefinition = new(sqlQueryText);

            var result = new CloudflareImagePage();
            await result.TryInitContainer();

            FeedIterator<CloudflareImagePage> queryResultSetIterator = result.DbContainer.GetItemQueryIterator<CloudflareImagePage>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                var response = await queryResultSetIterator.ReadNextAsync();
                foreach (var x in response)
                {
                    pageList.Add(x);
                }
            }
            return pageList.ToArray();
        }

        public static async Task<CloudflareImagePage> GetByPageNumber(int pageNumber)
        {
            var result = new CloudflareImagePage
            {
                PageNumber = pageNumber
            };
            await result.TryInitContainer();
            return await result.GetAsync();
        }

        public async Task DeleteSelfAsync()
        {
            await TryInitContainer();
            await DbContainer.DeleteItemAsync<CloudflareImagePage>(Id, new PartitionKey(PartitionKey));
        }

        public async Task DeleteByPageNumber(int page)
        {
            var duplicatePage = await GetByPageNumber(page);
            await duplicatePage.DeleteSelfAsync();
            return;
        }

        public async Task<bool> ExistsAsync()
        {
            await TryInitContainer();
            var existsRecord = await GetAsync();
            return existsRecord != null;
        }

        public async Task<CloudflareImagePage> GetAsync()
        {
            await TryInitContainer();
            CloudflareImagePage cfImgPage = null;
            try
            {
                IQueryable<CloudflareImagePage> queryablePages = DbContainer.GetItemLinqQueryable<CloudflareImagePage>(true);
                var cfImgsPage = queryablePages.Where(x => x.PageNumber == PageNumber)
                    .AsEnumerable()
                    .LastOrDefault();
                if (cfImgsPage != null)
                {
                    Id = cfImgsPage.Id;
                    cfImgPage = cfImgsPage;
                }
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                //Console.WriteLine(ex.ToString());
            }

            return cfImgPage;
        }

        public async Task InsertOrUpsertSelfAsync()
        {
            await TryInitContainer();
            var exists = await ExistsAsync();
            if (!exists)
            {
                await DbContainer.CreateItemAsync(this, new PartitionKey(PartitionKey));
            }
            else
            {
                await DbContainer.UpsertItemAsync(this, new PartitionKey(PartitionKey));
            }
            return;
        }

        public async Task TryInitContainer()
        {
            if (DbContainer == null)
            {
                var cosmosClient = new CosmosClient(DataConstants.ENDPOINT_URI, DataConstants.PRIMARY_KEY, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
                Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(DataConstants.COSMOS_DB_ID);
                DbContainer = await database.CreateContainerIfNotExistsAsync(CONTAINER, PARTITION_KEY);
            }
        }

        public static async Task SyncCloudflareImages()
        {
            var pageNumber = 1;
            var done = false;
            while (!done)
            {
                var cfImgPage = await CloudflareImageResponse.GetCloudflareImages(DataConstants.CLOUDFLARE_TOKEN, DataConstants.CLOUDFLARE_ACCOUNT_ID, pageNumber);
                await cfImgPage.InsertOrUpsertSelfAsync();
                if (PAGE_SIZE != cfImgPage.Images?.Length)
                {
                    done = true;
                }
                else
                {
                    pageNumber++;
                }
            }
        }
    }
}
