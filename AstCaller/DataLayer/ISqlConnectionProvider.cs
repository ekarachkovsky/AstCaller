using System.Data;

namespace AstCaller.DataLayer
{
    public interface ISqlConnectionProvider
    {
        IDbConnection GetConnection();
    }
}
