using System.Threading.Tasks;
using AstCaller.Models.Domain;

namespace AstCaller.DataLayer
{
    public interface ICampaignAbonentRepository
    {
        Task<int> SaveAsync(CampaignAbonent entity);
    }
}
