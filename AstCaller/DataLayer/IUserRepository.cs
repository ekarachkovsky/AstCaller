using System.Threading.Tasks;
using AstCaller.Models.Domain;

namespace AstCaller.DataLayer
{
    public interface IUserRepository
    {
        Task<User> GetByLogin(string login);

        Task<User> GetAsync(int id);
    }
}
