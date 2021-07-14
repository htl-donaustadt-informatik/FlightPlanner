using System.Data.Common;

namespace FlightPlanner.DataLayer
{
    public interface IOwnConnectionFactory
    {
        DbConnection CreateConnection();
    }
}