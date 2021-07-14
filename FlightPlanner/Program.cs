using FlightPlanner.DataLayer;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace FlightPlanner
{
    class Program
    {

        static void Main(string[] args)
        {
            // Programm verwendet ADO.NET API Connected Layer, Alternativen: ADO.NET Disconnected Layer, ADO.NET Entity Framework
            try
            {
                int rowCount = -2;

                // Die Angabe der Verbindung zur Datenbank erfolgt immer via Connnections mit einem Connectionstring
                // dieser ist manchmal aufwendig, DB-Herstellerdoku oder www.connectionstrings.com helfen
                // https://stackoverflow.com/questions/1229691/what-is-the-difference-between-integrated-security-true-and-integrated-securit

                // Connection string for connecting to SQL Server Local Db, for other database servers the connection
                // string must be modified.
                // Inital Catalog -> name of database
                // Integrated Security=SSPI -> use Windows Authentication (wie im Connection Dialog von Visual Studio)
                // Integrated Security=false -> use SQL Server Authentication, you must have an SQL Server database user account

                // TODO: it is best practice to specify the connection string in app.config/web.config
                // string connectionString = @"Data Source=delphin;Initial Catalog=FlightPlanner;Integrated Security=SSPI";
                // string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDb;Initial Catalog=FlightPlanner;Integrated Security=false;uid=Reinhard;password=reinhard";
                // string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDb;Initial Catalog=FlightPlanner;Integrated Security=SSPI";

                string connectionString = ConfigurationManager.ConnectionStrings["FlightPlannerConnection"].ConnectionString;
                string providerName = ConfigurationManager.ConnectionStrings["FlightPlannerConnection"].ProviderName;
  
                // IMPORTANT: if MySQL should be used the MySQL Connector for .NET (short name: Connector/Net) must be installed.
                // This program  uses the MySQL factory which must be registered in app.config to be useable.

                // https://stackoverflow.com/questions/9840303/abstract-factory-and-factory-pattern-in-dbproviderfactory-ado-net
                // Let's decide to use a SQL Server. Improvement: instead of hard-coding this here, we could let another method
                // decide what factory to use based on the data provider name from the app.config file.
                //IOwnConnectionFactory ownConnectionFactory = new OwnSqlServerConnectionFactory(connectionString);
                //CustomerDataMapper customerDataMapperOwn = new CustomerDataMapper(ownConnectionFactory, connectionString);
                //string test = customerDataMapperOwn.TestOwnConnectionFactory();
                //Console.WriteLine("$Testing ownConnectionFactory. Query returned: {test}");

                // Let's use the already implemented solution of ADO.NET instead of out own solution.
                DbProviderFactory adoNetFactory = DbProviderFactories.GetFactory(providerName);
                CustomerDataMapper customerDataMapper = new CustomerDataMapper(adoNetFactory, connectionString);
                string test1 = customerDataMapper.TestAdoNetProviderFactory();
                Console.WriteLine($"Testing ADO.NET's data provider factory. Query returned: {test1}");

                // The script to execute must not contain GO!
                // Recreate the database each time the program is run so that we always work with the same data for testing
                TestHelper.InitializeDatabase(connectionString);

                // CRUD - Create, Read, Update, Delete
                FlightDataMapper flightDataMapper = new FlightDataMapper(connectionString);

                Console.WriteLine("select * from Flight:");
                List<Flight> flights = flightDataMapper.ReadFlights();

                foreach (Flight flight in flights)
                {
                    Console.WriteLine(flight.ToString());
                }

                Flight testFlight = new Flight
                {
                    Id = 1001,
                    Departure = "Vienna",
                    Destination = "Budapest",
                    DepartureDate = DateTime.Now,
                    Duration = 40,
                    PlaneId = 21
                };

                // flightDataMapper.Create(testFlight);

                testFlight.Duration = 450;
                rowCount = flightDataMapper.Update(testFlight);

                // rowCount = flightDataMapper.Delete(testFlight);

                FlightPlannerDataModel flightPlannerDataModel = new FlightPlannerDataModel(connectionString);
                flightPlannerDataModel.DeleteFlight(204);

                BookingDataMapper bookingDataMapper = new BookingDataMapper(connectionString);
                // Stored procedure
                // bookingDataMapper.TestStoredProcedure();
                // bookingDataMapper.Create(new Booking(209, 1005, 3, 2, 11199m));

                Console.WriteLine();
                Console.WriteLine("--- Update your name ---");
                Console.WriteLine("Enter your new name: ");
                // Elon Musk's son's name: X Æ A-Xii
                // A nice name but this one is better to hack (SQL Injection) the database: 
                // X Æ A-Xii' where Customer.Id = 1003; update Booking set Booking.Price = 0 where Booking.CustomerId = 1003; --
                string newName = Console.ReadLine();
               
                // changes more than 2 data records (rows)
                rowCount = customerDataMapper.UpdateLastName(1003, newName);
                if (rowCount < 1)
                {
                    Console.WriteLine($"{nameof(customerDataMapper.UpdateLastName)}: No rows were updated!");
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Press enter to stop the program.");
            Console.ReadLine();
        }
    }
}
