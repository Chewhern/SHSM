using ASodium;
using SHSM_ServerApp;

var builder = WebApplication.CreateBuilder(args);

// Register background service
builder.Services.AddHostedService<RepetitiveRemoveUnusedSHSMHelper>();

// Add services to the container.

builder.Services.AddControllers();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(System.Net.IPAddress.Parse("0.0.0.0"), 1001);
}
);

var app = builder.Build();

// Run code when the app has fully started
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStarted.Register(() =>
{
    SodiumInit.Init();
});

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
