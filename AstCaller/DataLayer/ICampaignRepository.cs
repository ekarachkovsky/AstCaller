using System.Collections.Generic;
using System.Threading.Tasks;
using AstCaller.Models.Domain;

namespace AstCaller.DataLayer
{
    public interface ICampaignRepository
    {
        Task<Campaign> GetAsync(int id);

        Task<int> TotalCount();

        Task<IEnumerable<T>> GetRange<T>(int rangeBegin, int pageSize, string viewName);

        Task<int> SaveAsync(Campaign model);

        Task DeleteAsync(int id);
    }
}
