namespace IdentityService.KycTest.TestContainers;

public class BankTestInfrastructure : IAsyncLifetime
{
    // public PostgreSqlContainer Postgres { get; } = new PostgreSqlBuilder()
    //     .WithImage("postgres:13")
    //     .WithDatabase("ServiceA_test")
    //     .WithUsername("postgres")
    //     .WithPassword("postgres")
    //     .Build();
    //
    // public KafkaContainer Kafka { get; } = new KafkaBuilder()
    //     .WithImage("confluentinc/cp-kafka:7.4.0")
    //     .Build();
    //
    // public RedisContainer Redis { get; } = new RedisBuilder()
    //     .WithImage("redis:7")
    //     .WithPortBinding(6379, true)  // expose random free port
    //     .Build();

    public async Task InitializeAsync()
    {
        // await Postgres.StartAsync();
        // var builder = new NpgsqlConnectionStringBuilder(Postgres.GetConnectionString())
        // {
        //     Database = "ServiceA_test"
        // };
        // var connectionString = builder.ToString();
        // await InitializePostgresExtensions(connectionString);
        // await ApplyMigrations(connectionString);
        // await PrintTables(connectionString);
        //
        // await Kafka.StartAsync();
        // await Redis.StartAsync();
    }

    public async Task DisposeAsync()
    {
        // await Postgres.DisposeAsync();
        // await Kafka.DisposeAsync();
        // await Redis.DisposeAsync();
    }
    
    
    private async Task InitializePostgresExtensions(string connectionString)
    {
        // await using var conn = new NpgsqlConnection(connectionString);
        // await conn.OpenAsync();
        // await using var cmd = new NpgsqlCommand("CREATE EXTENSION IF NOT EXISTS hstore;", conn);
        // await cmd.ExecuteNonQueryAsync();
    }
    
    private async Task ApplyMigrations(string connectionString)
    {
        // var builder = new DbContextOptionsBuilder<AppDbContext>();
        // builder.UseNpgsql(connectionString, npgsql =>
        // {
        //     npgsql.MigrationsAssembly(typeof(DesignTimeDbContextFactory).Assembly);
        // });
        //
        // await using var context = new AppDbContext(builder.Options);
        //
        // Console.WriteLine("Applying migrations...");
        // Console.WriteLine($"DB: {connectionString}");
        //
        // await context.Database.MigrateAsync();
        //
        // var tables = context.Model.GetEntityTypes()
        //     .Select(t => t.GetTableName())
        //     .Distinct()
        //     .ToList();
        //
        // Console.WriteLine("EF Table Names:");
        // tables.ForEach(Console.WriteLine);
    }

    private async Task PrintTables(string connectionString)
    {
        // await using var conn = new NpgsqlConnection(connectionString);
        // await conn.OpenAsync();
        //
        // await using var cmd = new NpgsqlCommand(
        //     "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';",
        //     conn);
        //
        // await using var reader = await cmd.ExecuteReaderAsync();
        //
        // while (await reader.ReadAsync())
        // {
        //     Console.WriteLine(reader.GetString(0));
        // }
    }
}