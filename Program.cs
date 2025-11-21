using Microsoft.EntityFrameworkCore;
using FxInvestmentApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQL Server with detailed error handling
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = "Server=db19057.databaseasp.net;Database=db19057;User Id=db19057;Password=Bt9-8Q_bz6F@;Encrypt=False;MultipleActiveResultSets=True;Connection Timeout=30;";

    Console.WriteLine("🔗 Configuring SQL Server connection...");

    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 2, // Reduced retries for faster failure
            maxRetryDelay: TimeSpan.FromSeconds(2),
            errorNumbersToAdd: null);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

// Simple connection test without complex queries
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    Console.WriteLine("🔄 Testing basic database connection...");

    // Just test if we can connect, don't query tables yet
    var canConnect = dbContext.Database.CanConnect();

    if (canConnect)
    {
        Console.WriteLine("✅ Basic database connection successful!");
    }
    else
    {
        Console.WriteLine("❌ Cannot connect to database");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Connection failed: {ex.Message}");
    Console.WriteLine($"💡 Try the /api/dbtest/raw-connection endpoint for more details");
}

Console.WriteLine("🚀 FxInvestment API Started!");
Console.WriteLine("📝 Swagger: /swagger");
Console.WriteLine("🔧 Database Test: /api/dbtest/raw-connection");

app.Run();