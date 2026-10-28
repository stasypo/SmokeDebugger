using System.Diagnostics;
using BlazorAppMultitarget.Components;
using BlazorAppMultitarget.Services;
using JetBrains.Annotations;

// =============================================================================
// BlazorAppMultitarget - the ASP.NET Core host for the debugger smoke test.
//
// This project covers the "whole checklist" technology (ASP.NET Core) and the
// Multitarget framework requirement: it builds for net10.0 AND net9.0, so the
// run configuration lets you pick the target framework.
//
// SCENARIO "multitarget-session" - README -> Basics -> Core actions.
// SETUP: in the run configuration select the net10.0 target framework.
// ACTION: start a debug session, hit the breakpoint below, then Stop, switch the
//   run configuration to net9.0 and re-run.
// EXPECTED: both target frameworks start under the debugger and the same
//   breakpoints are hit in both sessions.
// =============================================================================

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

// Shared across all circuits so every browser tab plays the same /multiplayer game.
builder.Services.AddSingleton<MultiplayerGame>();

// SCENARIO "startup-console-output" - README -> More -> Console 1 and 3.
// ACTION: verify the numbers 0..4 appear once each, in order, in the debug console.
for (int i = 0; i < 5; i++)
{
	Console.WriteLine(i);
}

// SCENARIO "startup-user-exception" - README -> Basics -> Breakpoints 2.
// SETUP: CLR Exception Breakpoint for System.InvalidOperationException,
//   Break on "Thrown", "User code only" on.
// EXCEPTION: the debugger must suspend on the throw below, before the catch runs.
// EXPECTED: the frame points at this line in Program.cs and the message is
//   "smoke-test startup exception"; resuming prints the exception to the console.
try
{
	throw new InvalidOperationException("smoke-test startup exception");
}
catch (Exception e)
{
	Console.WriteLine(e);
}

var app = builder.Build();

// SCENARIO "jit-debugger" - README -> [Before release only] JIT-debugger.
// ACTION: uncomment one of the lines below and start the app WITHOUT the debugger.
// EXPECTED (Launch): the configured JIT debugger attaches here.
// EXPECTED (Break): the attached debugger suspends exactly on this line.
// Debugger.Launch();
// Debugger.Break();

// BREAKPOINT "aspnet-pipeline": stop here to verify that a breakpoint in the
// startup path of an ASP.NET Core app is hit, and use it as the anchor for the
// Modules / Memory / Debug Output views - README -> More -> Modules, Memory,
// Debug Output.
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

// SCENARIO "inline-breakpoint-aspnet" - README -> Basics -> Breakpoints 1.2.
// BREAKPOINT: three statements share the line below. Use the inline breakpoint
//   to put a breakpoint on 'app.UseAntiforgery()' only.
// EXPECTED: execution suspends on that statement, not at the beginning of the line.
app.UseStatusCodePagesWithReExecute("/not-found"); app.UseHttpsRedirection(); app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();

// SCENARIO "global-watch" - README -> More -> Threads & Variables 3.5.
// SETUP: uncomment the JetBrains.Annotations PackageReference in
//   BlazorAppMultitarget.csproj, then uncomment the Animal class below and the
//   'var animal = ...' line above app.Run(). The same attribute is prepared on
//   Components/Pages/Weather.razor -> WeatherForecast.
// EXPECTED: "Test Global Watch: Animal Name" appears in the Watches view on every
//   suspension where an Animal instance is reachable, without adding it manually.
// var animal = new Animal { Name = "Dog" };
// class Animal
// {
// 	[DebuggerGlobalWatch(Name = "Test Global Watch: Animal Name")]
// 	public string Name { get; set; }
// }