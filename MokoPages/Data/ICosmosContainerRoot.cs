using Microsoft.Azure.Cosmos;

namespace MokoPages.Data
{
    public interface ICosmosContainerRoot<T>
    {
        Container DbContainer { get; set; }
        string PartitionKey { get; set; }
        Task<T> GetAsync();
        Task<bool> ExistsAsync();
        Task DeleteSelfAsync();
        Task InsertOrUpsertSelfAsync();
        Task TryInitContainer();
    }
}
