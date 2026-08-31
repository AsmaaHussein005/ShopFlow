using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ShopFlow.Data;

public class ShopFlowDbContextFactory : IDesignTimeDbContextFactory<ShopFlowDbContext>
{
    public ShopFlowDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ShopFlowDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(localdb)\\mssqllocaldb;Database=ShopFlowDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString);

        return new ShopFlowDbContext(optionsBuilder.Options);
    }
}
