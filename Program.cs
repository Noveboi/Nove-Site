using LearningBlazor.Components;
using LearningBlazor.Hubs;
using LearningBlazor.Utilities;
using LearningBlazor.Utilities.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();
builder.Services.AddResponseCompression(opts =>
{
	opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
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

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

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
