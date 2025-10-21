using System.Data;

namespace Company.App.Application.Interface.Persistence.Context
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
