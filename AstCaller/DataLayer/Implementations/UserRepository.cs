using System.Threading.Tasks;
using AstCaller.Models.Domain;
using Dapper;

namespace AstCaller.DataLayer.Implementations
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ISqlConnectionProvider connectionProvider) : base(connectionProvider, "users")
        {
        }

        public async Task<User> GetByLogin(string login)
        {
            using (var db = _connectionProvider.GetConnection())
            {
                return await db.QuerySingleOrDefaultAsync<User>("select * from users where upper(login)=@login", new { login });
            }
        }
    }
}
