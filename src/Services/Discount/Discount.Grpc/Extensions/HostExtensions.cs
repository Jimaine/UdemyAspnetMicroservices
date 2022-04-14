using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Threading;

namespace Discount.Grpc.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();

                try
                {
                    string connectionString = configuration.GetValue<string>("DatabaseSettings:ConnectionString");

                    logger.LogInformation("Migrating postgresql database.");

                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();

                        using (var command = new NpgsqlCommand { Connection = connection})
                        {
                            command.CommandText = GetDeleteIfExistsCommand();
                            command.ExecuteNonQuery();

                            command.CommandText = GetCreateCommand();
                            command.ExecuteNonQuery();

                            command.CommandText = GetSeedCommandOne();
                            command.ExecuteNonQuery();

                            command.CommandText = GetSeedCommandTwo();
                            command.ExecuteNonQuery();

                            logger.LogInformation("Migrated postgresql database.");
                        }
                    }
                }
                catch (NpgsqlException npgsqlException)
                {
                    logger.LogError(npgsqlException, "An error occured while migrating the postgresql database");

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }
            }

            return host;
        }

        private static string GetDeleteIfExistsCommand()
        {
            return @"
DROP TABLE IF EXISTS Coupon
;";
        }

        private static string GetCreateCommand()
        {
            return @"
CREATE TABLE Coupon(
	ID SERIAL PRIMARY KEY NOT NULL,
	ProductName VARCHAR(24) NOT NULL,
	Description TEXT,
	Amount INT
);";
        }

        private static string GetSeedCommandOne()
        {
            return @"
INSERT INTO Coupon (
    ProductName
    ,Description
    ,Amount
) 
VALUES (
    'IPhone X'
    ,'IPhone Discount'
    ,150
);";
        }

        private static string GetSeedCommandTwo()
        {
            return @"
INSERT INTO Coupon (
    ProductName
    ,Description
    ,Amount
) 
VALUES (
    'Samsung 10'
    ,'Samsung Discount'
    ,100
);";
        }
    }
}
