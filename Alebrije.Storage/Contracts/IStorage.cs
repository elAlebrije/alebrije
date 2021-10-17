using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Alebrije.Storage.Contracts
{
    public interface IStorage
    {
        Task<string> Upload(IFormFile file, AssetType asset);
    }
}