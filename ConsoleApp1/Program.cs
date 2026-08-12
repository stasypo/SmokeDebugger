using System.Diagnostics;
using ConsoleApp1.Scenarios;

// =============================================================================
// ConsoleApp1 - the scenario catalog for the debugger smoke test.
//
// Every scenario in this project maps to a numbered item in README.md -> Scope.
// Scenario groups run sequentially and deterministically, so the whole checklist
// can be walked top to bottom in a single debug session.
//
// To shorten a run, pass one group name as a program argument, for example:
//     dotnet run --project ConsoleApp1 -- stepping
// Group names: breakpoints, stepping, evaluation, visualizers, concurrency,
//              exceptions, native, console, crossthread
// =============================================================================

public class Program
{
	public static async Task Main(string[] args)
	{
		var only = args.Length > 0 ? args[0] : null;

		// BREAKPOINT "session-start": set a line breakpoint here to verify the core
		// session actions - README -> Basics -> Core actions.
		// ACTION: Run/Re-run, Pause, Resume and Stop from the toolbar, the Debug tool
		// window, the Run menu and the keyboard shortcuts.
		Console.WriteLine("=== ConsoleApp1 debugger scenarios ===");

		if (Selected(only, "breakpoints")) BreakpointScenarios.Run();
		if (Selected(only, "stepping")) SteppingScenarios.Run();
		if (Selected(only, "evaluation")) EvaluationScenarios.Run();
		if (Selected(only, "visualizers")) VisualizerScenarios.Run();
		if (Selected(only, "concurrency")) await ConcurrencyScenarios.RunAsync();
		if (Selected(only, "exceptions")) ExceptionScenarios.Run();
		if (Selected(only, "native")) NativeInteropScenarios.Run();
		if (Selected(only, "console")) ConsoleScenarios.Run();
		if (Selected(only, "crossthread")) CrossThreadDependency();

		Console.WriteLine("=== all scenarios finished ===");
	}

	private static bool Selected(string? only, string group) =>
		only is null || string.Equals(only, group, StringComparison.OrdinalIgnoreCase);

	// SCENARIO "cross-thread-evaluation" - README -> More -> Evaluation.
	// BREAKPOINT: on the Console.WriteLine below.
	// ACTION: expand 'a' in the Variables view and try to expand 'a.Name'.
	// EXPECTED: the property getter calls Debugger.NotifyOfCrossThreadDependency(),
	// so the debugger does not evaluate it silently - it shows a "Load value" /
	// "evaluation requires all threads to run" prompt. Clicking it evaluates the
	// property and shows a SomeClass2 instance.
	private static void CrossThreadDependency()
	{
		var a = new SomeClass();
		Console.WriteLine($"cross-thread scenario ready: {a.GetType().Name}");
	}
}

public class SomeClass2;

public class SomeClass
{
	public SomeClass2 Name
	{
		get
		{
			// Forces the debugger to ask before evaluating this property.
			Debugger.NotifyOfCrossThreadDependency();
			return new SomeClass2();
		}
	}
}
