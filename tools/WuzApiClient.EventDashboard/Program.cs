using WuzApiClient.EventDashboard.Components;
using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Services;
using WuzApiClient.RabbitMq.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor Server services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure dashboard options
builder.Services.Configure<DashboardOptions>(
    builder.Configuration.GetSection("Dashboard"));

// Add event stream service (singleton to preserve state across circuits)
builder.Services.AddSingleton<IEventStreamService, EventStreamService>();

// Add WuzApiClient.RabbitMq event consumer with automatic handler registration
builder.Services.AddWuzEvents(builder.Configuration, b => b
    .AddHandlersFromAssembly(ServiceLifetime.Singleton, typeof(Program).Assembly)
);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
