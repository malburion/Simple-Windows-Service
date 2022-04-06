using SimpleService;
using Serilog;
using SimpleService.TaskQueue;

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .UseWindowsService()
    .ConfigureServices((context, services) =>
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("G:\\logs\\app.txt")
            .CreateLogger();
        services.AddSingleton<FileWatcher>();
        services.AddHostedService<QueueHostedService>();
        services.AddSingleton<IBackgroundTaskQueue>(_ => {
            if (!int.TryParse(context.Configuration["QueueCapacity"], out var queueCapacity))
            {
                queueCapacity = 100;
            }

            return new DefaultBackgroundTaskQueue(queueCapacity);
        });
    })
    .Build();

FileWatcher fileWatcher = host.Services.GetRequiredService<FileWatcher>();
fileWatcher.WatchFolder("G:\\data");

await host.RunAsync();
