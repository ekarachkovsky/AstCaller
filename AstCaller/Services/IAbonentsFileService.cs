using System.Threading.Tasks;

namespace AstCaller.Services
{
    public interface IAbonentsFileService
    {
        Task<int> ProcessFileAsync(string fileLocation, int campaignId);
    }
}
