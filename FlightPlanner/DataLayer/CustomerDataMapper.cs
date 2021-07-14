using FlightPlanner.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightPlanner
{
    class CustomerDataMapper
    {
        // own implementation
        public IOwnConnectionFactory OwnConnectionFactory { get; set; }
        
        // better use the already existing ADO.NET implementation than the own implementation
        public DbProviderFactory DbProviderFactory { get; set; }

        public string ConnectionString { get; set; }

        public CustomerDataMapper(IOwnConnectionFactory ownConnectionFactory, string connectionString)
        {
            this.OwnConnectionFactory = ownConnectionFactory;
            this.ConnectionString = connectionString;
        }

        public CustomerDataMapper(DbProviderFactory dbProviderFactory, string connectionString)
        {
            this.DbProviderFactory = dbProviderFactory;
            this.ConnectionString = connectionString;
        }


        public string TestOwnConnectionFactory()
        {
            using (DbConnection databaseConnection = this.OwnConnectionFactory.CreateConnection())
            {
                // erzeuge zuerst ein zur Sql Server Connection passendes Command, weise Select Befehl zu
                IDbCommand command = databaseConnection.CreateCommand();

                // Aus Performance Gründen alle Datensätze auf einmal lesen, nicht die Methode Read() dieser Klasse verwenden.
                command.CommandText = "select * from Customer";

                databaseConnection.Open();

                // erzeuge den DataReader (der das zeilenweise Auslesen erlaubt) aus dem Command
                string testResult = command.ExecuteScalar().ToString();
                
                return testResult;
            }
        }

        public string TestAdoNetProviderFactory()
        {
            using (DbConnection databaseConnection = this.DbProviderFactory.CreateConnection())
            {
                databaseConnection.ConnectionString = this.ConnectionString;
                // erzeuge zuerst ein zur Sql Server Connection passendes Command, weise Select Befehl zu
                IDbCommand command = databaseConnection.CreateCommand();

                // Aus Performance Gründen alle Datensätze auf einmal lesen, nicht die Methode Read() dieser Klasse verwenden.
                command.CommandText = "select * from Customer";

                databaseConnection.Open();

                // erzeuge den DataReader (der das zeilenweise Auslesen erlaubt) aus dem Command
                string testResult = command.ExecuteScalar().ToString();

                return testResult;
            }
        }

        public int UpdateLastName(int id, string newName)
        {
            using (DbConnection databaseConnection = this.DbProviderFactory.CreateConnection())
            {
                databaseConnection.ConnectionString = this.ConnectionString;
                IDbCommand updateCustomerCommand = databaseConnection.CreateCommand();
                updateCustomerCommand.CommandText =
                    $"update Customer set LastName = '{newName}' where Customer.Id = {id};";

                // Console.WriteLine NICHT an dieser Stelle in einem professionellen Programm verwenden, 
                // Methode soll auch bei GUI Anwendungen funktionieren
                Console.WriteLine(updateCustomerCommand.CommandText);

                databaseConnection.Open();

                int rowCount = updateCustomerCommand.ExecuteNonQuery();
                return rowCount;
            }
        }
    }
}
