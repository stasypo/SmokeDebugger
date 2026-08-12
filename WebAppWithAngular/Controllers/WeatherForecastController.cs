using Microsoft.AspNetCore.Mvc;

namespace WebAppWithAngular.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
	private static readonly string[] Summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

	private readonly ILogger<WeatherForecastController> _logger;

	public WeatherForecastController(ILogger<WeatherForecastController> logger)
	{
		_logger = logger;
	}

	// SCENARIO "aspnet-and-angular-request" - README -> Basics -> Breakpoints 1.1,
	//   README -> Basics -> Stepping 1 and 3, README -> More -> Evaluation 5 and 6.
	// SETUP: run WebAppWithAngular under the debugger and open /fetch-data in the browser -
	//   the Angular page calls this action.
	// BREAKPOINT: on 'return Enumerable.Range(1, 5)...' below.
	// EXPECTED: the debugger suspends on the server side while the browser request is in
	//   flight; the Frames list shows the ASP.NET Core request pipeline as the caller.
	// EVALUATE: 'Summaries.Length' -> 10; open the LINQ visualizer on the
	//   Enumerable.Range(...).Select(...) chain and the collection visualizer on the result.
	// ALSO: this is the C#-plus-TypeScript scenario - keep a TS breakpoint in
	//   ClientApp/src/app/fetch-data/fetch-data.component.ts and verify both are hit in the
	//   same session.
	[HttpGet]
	public IEnumerable<WeatherForecast> Get()
	{
		Console.WriteLine("Hello!");
		Console.WriteLine("Hello!");
		Console.WriteLine("Hello!");
		Console.WriteLine("Hello!");
		Console.WriteLine("Hello!");
		return Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index))
				, TemperatureC = Random.Shared.Next(-20, 55)
				, Summary = Summaries[Random.Shared.Next(Summaries.Length)]
			})
			.ToArray();
	}
}