using Microsoft.Data.SqlClient;

namespace BugTracker.Helpers
{
    public class DbHelper
    {
        // METHODS
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(@"Server=(localdb)\mssqllocaldb;Database=BugTrackerDevDB;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
