//using Google.Cloud.BigQuery.V2;
//using Newtonsoft.Json;

//namespace MokoPages
//{
//    public class CloudflareImage
//    {
//        public long PageNumber { get; set; }
//        public long PagePosition { get; set; }


//        [JsonProperty("id")]
//        public string Id { get; set; }

//        [JsonProperty("filename")]
//        public string Filename { get; set; }

//        [JsonProperty("uploaded")]
//        public DateTime Uploaded { get; set; }

//        [JsonProperty("requireSignedURLs")]
//        public bool RequireSignedURLs { get; set; }

//        [JsonProperty("variants")]
//        public List<string> Variants { get; set; }
//    }

//    public class CloudflareImageResult
//    {
//        [JsonProperty("images")]
//        public List<CloudflareImage> Images { get; set; }
//    }

//    public class StoreCloudflareImageResult
//    {
//        public Dictionary<long, string>? ErrorMessageMap { get; set; }

//        public long? AffectedRows { get; private set; }

//        public void IncrementAffectedRows(long? affectedRows)
//        {
//            if (affectedRows.HasValue)
//            {
//                if (!AffectedRows.HasValue)
//                {
//                    AffectedRows = affectedRows.Value;
//                }
//                else
//                {
//                    AffectedRows += affectedRows.Value;
//                }
//            }
//        }
//    }
//    public class CloudflareImageResponse
//    {
//        [JsonProperty("result")]
//        public CloudflareImageResult Result { get; set; }

//        [JsonProperty("result_info")]
//        public object ResultInfo { get; set; }

//        [JsonProperty("success")]
//        public bool Success { get; set; }

//        [JsonProperty("errors")]
//        public List<object> Errors { get; set; }

//        [JsonProperty("messages")]
//        public List<object> Messages { get; set; }

//        [JsonIgnore]
//        public StoreCloudflareImageResult StoreResult { get; set; }

//        [JsonIgnore]
//        public int PerPage { get { return BigQueryCloudflarePageResponse.FIXED_PAGE_SIZE; } }

//        public static async Task<CloudflareImageResponse> GetCloudflareImageResponsePageAsync(BigQueryCloudflarePageResponse pageResponse, string cloudflareToken, string accountId)
//        {
//            HttpClient httpClient = new();
//            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cloudflareToken);
//            var resultStr = await httpClient.GetStringAsync($"https://api.cloudflare.com/client/v4/accounts/{accountId}/images/v1?page={pageResponse.NextPage}&per_page={BigQueryCloudflarePageResponse.FIXED_PAGE_SIZE}");
//            var result = JsonConvert.DeserializeObject<CloudflareImageResponse>(resultStr);
//            return result;
//        }

//        public async Task StoreCFImages(BigQueryClient bigQueryClient, BigQueryCloudflarePageResponse pageResponse)
//        {
//            //StoreResult = new StoreCloudflareImageResult();
//            var query = "INSERT INTO `mokodb.mkodb.cfimgsetsby50` (pageNumber, pagePosition, id, filename, uploaded, requiredSignedURLs, variants) WITH vars AS (SELECT @id as testId) SELECT @pageNumber, @pagePosition, testId, @filename, @uploaded, @requiredSignedURLs, @variants FROM `mokodb.mkodb.cfimgsetsby50`, vars WHERE NOT(testId IN (SELECT id FROM `mokodb.mkodb.cfimgsetsby50`)) LIMIT 1";
//            if (Result?.Images?.Count > 0)
//            {
//                for (long i = pageResponse.NextPagePosition; i < Result.Images.Count; i++)
//                {
//                    try
//                    {
//                        var image = Result.Images[(int)i];
//                        bigQueryClient.ExecuteQuery(query, new BigQueryParameter[]
//                        {
//                            new BigQueryParameter("pageNumber",BigQueryDbType.Int64, pageResponse.NextPage),
//                            new BigQueryParameter("pagePosition",BigQueryDbType.Int64, i+1),
//                            new BigQueryParameter("id",BigQueryDbType.String, image.Id),
//                            new BigQueryParameter("filename",BigQueryDbType.String, image.Filename),
//                            new BigQueryParameter("uploaded", BigQueryDbType.DateTime, image.Uploaded),
//                            new BigQueryParameter("requiredSignedURLs", BigQueryDbType.Bool, image.RequireSignedURLs),
//                            new BigQueryParameter("variants", BigQueryDbType.String, string.Join(',', image.Variants)),
//                         });

//                        //BigQueryResults ? results = await bigQueryClient.ExecuteQueryAsync(query, parameters);
//                        //if (results != null)
//                        //{
//                        //    StoreResult.IncrementAffectedRows(results?.NumDmlAffectedRows);
//                        //}

//                        var duplicateCheckQuery = "select * FROM `mokodb.mkodb.cfimgsetsby50` WHERE id IN (SELECT id FROM `mokodb.mkodb.cfimgsetsby50` GROUP BY id HAVING COUNT(*) > 1)";
//                        var duplicateImages = await GetBQCFImages(bigQueryClient, duplicateCheckQuery);
//                        if (duplicateImages.Result.Images.Count > 0)
//                        {
//                            foreach (var duplicate in duplicateImages.Result.Images)
//                            {
//                                var removeQuery = "delete from `mokodb.mkodb.cfimgsetsby50` WHERE id = @id";
//                                var removeParameters = new BigQueryParameter[]
//                                {
//                                    new BigQueryParameter("id",BigQueryDbType.String, image.Id),
//                                };
//                                var result = await bigQueryClient.ExecuteQueryAsync(removeQuery, removeParameters);
//                                bigQueryClient.ExecuteQuery(query, new BigQueryParameter[]
//                        {
//                            new BigQueryParameter("pageNumber",BigQueryDbType.Int64, duplicate.PageNumber),
//                            new BigQueryParameter("pagePosition",BigQueryDbType.Int64, duplicate.PagePosition),
//                            new BigQueryParameter("id",BigQueryDbType.String, duplicate.Id),
//                            new BigQueryParameter("filename",BigQueryDbType.String, duplicate.Filename),
//                            new BigQueryParameter("uploaded", BigQueryDbType.DateTime, duplicate.Uploaded),
//                            new BigQueryParameter("requiredSignedURLs", BigQueryDbType.Bool, duplicate.RequireSignedURLs),
//                            new BigQueryParameter("variants", BigQueryDbType.String, string.Join(',', duplicate.Variants)),
//                        });
//                            }
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        if (StoreResult.ErrorMessageMap == null)
//                        {
//                            StoreResult.ErrorMessageMap = new Dictionary<long, string>();
//                        }
//                        StoreResult.ErrorMessageMap.Add(i, ex.Message);
//                    }
//                }
//            }
//            //return StoreResult;
//        }

//        public static async Task<BigQueryCloudflarePageResponse> GetNextPageNumber(BigQueryClient bigQueryClient)
//        {
//            var query = @"select maxPage, (select max(pagePosition) FROM `mokodb.mkodb.cfimgsetsby50` where pageNumber = maxPage) as maxPagePosition FROM (SELECT max(pageNumber) as maxPage FROM `mokodb.mkodb.cfimgsetsby50`)";
//            var pageQueryResult = await bigQueryClient.ExecuteQueryAsync(query, null);
//            var rawRow = pageQueryResult.First().RawRow;
//            var pageStr = rawRow.F[0].V;
//            var positionStr = rawRow.F[1].V;
//            var page = Convert.ToInt64(pageStr);
//            var position = Convert.ToInt64(positionStr);
//            return new BigQueryCloudflarePageResponse(page, position);
//        }

//        public static async Task<CloudflareImageResponse> GetBQCFImages(BigQueryClient bigQueryClient, string replaceQuery = "")
//        {
//            var query = "select * from `mokodb.mkodb.cfimgsetsby50` order by pageNumber, pagePosition";
//            if (!string.IsNullOrEmpty(replaceQuery))
//            {
//                query = replaceQuery;
//            }
//            var imageQueryResult = await bigQueryClient.ExecuteQueryAsync(query, null);
//            var result = new CloudflareImageResponse
//            {
//                Result = new CloudflareImageResult
//                {
//                    Images = new List<CloudflareImage> { }
//                }
//            };
//            if (imageQueryResult != null)
//            {
//                foreach (var row in imageQueryResult)
//                {
//                    result.Result.Images.Add(new CloudflareImage
//                    {
//                        PageNumber = Convert.ToInt64(row.RawRow.F[0].V),
//                        PagePosition = Convert.ToInt64(row.RawRow.F[1].V),
//                        Id = row.RawRow.F[2].V.ToString(),
//                        Filename = row.RawRow.F[3].V.ToString(),
//                        Uploaded = Convert.ToDateTime(row.RawRow.F[4].V.ToString()),
//                        RequireSignedURLs = Convert.ToBoolean(row.RawRow.F[5].V.ToString()),
//                        Variants = row.RawRow.F[6].V.ToString().Split(',').ToList()
//                    });
//                }
//            }

//            return result;
//        }
//    }

//    public class BigQueryCloudflarePageResponse
//    {
//        public static readonly int FIXED_PAGE_SIZE = 50;
//        public long PageNumber { get; set; }
//        public long LastPagePosition { get; set; }
//        public long NextPage { get { return LastPagePosition == FIXED_PAGE_SIZE ? PageNumber + 1 : PageNumber; } }
//        public long NextPagePosition { get { return LastPagePosition == FIXED_PAGE_SIZE ? 0 : LastPagePosition; } }

//        public BigQueryCloudflarePageResponse(long pageNumber, long lastPagePosition)
//        {
//            PageNumber = pageNumber;
//            LastPagePosition = lastPagePosition;
//        }

//    }
//}
