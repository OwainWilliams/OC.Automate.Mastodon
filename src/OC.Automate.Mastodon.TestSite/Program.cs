try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services
        .AddUmbraco(builder.Environment, builder.Configuration)
        .AddBackOffice()
        .AddWebsite()
        .AddComposers()
        .Build();

    var app = builder.Build();
    app.UseUmbracoCore();
    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Fatal error: {ex}");
    throw;
}
