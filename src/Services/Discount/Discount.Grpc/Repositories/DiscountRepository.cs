﻿using Dapper;
using Discount.Grpc.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Threading.Tasks;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(@$"
SELECT * 
FROM Coupon 
WHERE ProductName = @ProductName"
                , new {
                    ProductName = productName
                });

            return coupon ?? new Coupon();
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync($@"
INSERT INTO Coupon (
    ProductName 
    ,Description
    ,Amount
) 
VALUES (
    @ProductName
    ,@Description
    ,@Amount
)"
                , new
                {
                    ProductName = coupon.ProductName,
                    Description = coupon.Description,
                    Amount = coupon.Amount
                });

            return affected > 0;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync($@"
UPDATE Coupon
SET 
    ProductName = @ProductName
    ,Description = @Description
    ,Amount = @Amount
WHERE 1=1
    AND Id = @Id"
                , new
                {
                    ProductName = coupon.ProductName,
                    Description = coupon.Description,
                    Amount = coupon.Amount,
                    Id = coupon.Id
                });

            return affected > 0;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync(@$"
DELETE FROM Coupon 
WHERE ProductName = @ProductName"
                , new
                {
                    ProductName = productName
                });

            return affected > 0;
        }
    }
}
