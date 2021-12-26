namespace SixKeysOfTangrin;

public static class Program
{
    static void Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();
        using IServiceScope serviceScope = host.Services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;

        IGame game = provider.GetRequiredService<IGame>();

        game.Start();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
                services.AddScoped<IGame, Game>()
                        .AddScoped<IInputDevice, ConsoleInputDevice>()
                        .AddScoped<IOutputDevice, ConsoleOutputDevice>()
                        .AddScoped<IRandomGenerator, StandardRandomGenerator>()
                        .AddScoped<IMap, TangrinMap>()
                        .AddScoped<IPlayer, Player>()
                        .AddScoped<IInventory, ThreeSlotInventory>()
                        .AddScoped<ISuspense, Suspense>());
}
