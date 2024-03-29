using WebApp.Components;
using Games.Hubs;
using Games;
using Games.Base;
using WebApp.Utilities.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR.Client;
using Serilog;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();
builder.Services.AddResponseCompression(opts =>
{
	opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
});

// Add the "User" service to the DI container
builder.Services.AddScoped(sp =>
{
	return new UserService(sp.GetRequiredService<ProtectedSessionStorage>());
});

// Add necessary HubConnections to the scope of the 
builder.Services.AddKeyedScoped("tictactoehub", (sp, obj) =>
{
	var navManager = sp.GetRequiredService<NavigationManager>();
	return new HubConnectionBuilder()
		.WithUrl(navManager.ToAbsoluteUri("/tictactoehub"))
		.WithAutomaticReconnect()
		.Build();
});

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console(
	outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
	.CreateLogger();

builder.Host.UseSerilog();

Log.Information("Starting web application!");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseResponseCompression();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();
app.MapHub<TicTacToeHub>("/tictactoehub");

app.Run();
