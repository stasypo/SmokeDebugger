namespace ConsoleApp1.Scenarios;

/// <summary>
/// Stepping scenarios - README -> Basics -> Stepping.
/// Async stepping lives in <see cref="ConcurrencyScenarios"/>.
/// </summary>
internal static class SteppingScenarios
{
	public static void Run()
	{
		Console.WriteLine("--- stepping ---");
		StepOverAndForceStepOver();
		StepIntoAndStepOut();
		SmartStepInto();
		RunToCursorAndSkipToCursor();
		SteppingTime();
		StepIntoExternalCode();
	}

	// SCENARIO "step-over-and-force-step-over" - README -> Basics -> Stepping 1 and 2.
	// SETUP: line breakpoint on 'var doubled = Double(value);' AND a second line
	//   breakpoint inside Double (marked below).
	// STEP OVER on 'var doubled = ...': the breakpoint inside Double is honoured, so
	//   execution stops there instead of on the next line.
	// FORCE STEP OVER on the same line (restart first): the breakpoint inside Double is
	//   ignored and execution lands on the Console.WriteLine below with doubled == 42.
	private static void StepOverAndForceStepOver()
	{
		int value = 21;
		var doubled = Double(value);
		Console.WriteLine($"step-over: doubled = {doubled}");
	}

	private static int Double(int x)
	{
		// Keep the "Force Step Over" breakpoint on the next line.
		return x * 2;
	}

	// SCENARIO "step-into-and-step-out" - README -> Basics -> Stepping 3 and 5.
	// BREAKPOINT: on 'var result = Outer(3);'.
	// STEP INTO: lands in Outer, then in Inner. The Frames list grows by one entry per
	//   step and the editor follows the current frame.
	// STEP OUT from Inner: execution resumes until Inner returns and suspends in Outer;
	//   a second Step Out suspends back here.
	// WATCH: 'result' == 36 (Inner(3) == 12, Outer returns 12 * 3).
	private static void StepIntoAndStepOut()
	{
		var result = Outer(3);
		Console.WriteLine($"step-into: result = {result}");
	}

	private static int Outer(int x) => Inner(x) * x;

	private static int Inner(int x) => x * 4;

	// SCENARIO "smart-step-into" - README -> Basics -> Stepping 4.
	// BREAKPOINT: on 'var report = Format(Describe(7), Describe(8));'.
	// SMART STEP INTO: the debugger must offer three targets - Format, and both
	//   Describe calls. Pick the second Describe.
	// EXPECTED: execution enters Describe with n == 8; the final report is
	//   "7:odd | 8:even".
	private static void SmartStepInto()
	{
		var report = Format(Describe(7), Describe(8));
		Console.WriteLine($"smart-step-into: {report}");
	}

	private static string Format(string left, string right) => $"{left} | {right}";

	private static string Describe(int n) => n % 2 == 0 ? $"{n}:even" : $"{n}:odd";

	// SCENARIO "run-to-cursor-and-skip-to-cursor" - README -> Basics -> Stepping 6 and 7.
	// BREAKPOINT: on 'int total = 0;'.
	// RUN TO CURSOR: place the caret on the Console.WriteLine below and run to cursor.
	//   Execution suspends there with total == 45 (the loop really ran).
	// SKIP TO CURSOR: restart, stop on 'int total = 0;', Step Over once so total == 0,
	//   then place the caret on the same Console.WriteLine and Skip to Cursor.
	//   The loop body is never executed, so total is still 0 and the app prints 0.
	private static void RunToCursorAndSkipToCursor()
	{
		int total = 0;
		for (int i = 0; i < 10; i++)
		{
			total += i;
		}

		Console.WriteLine($"run-to-cursor: total = {total}");
	}

	// SCENARIO "stepping-time" - README -> Basics -> Stepping 8.
	// BREAKPOINT: on 'var checksum = SlowCall();'.
	// STEP OVER: SlowCall blocks for ~750 ms, so the stepping time indicator has
	//   something measurable to report.
	// EXPECTED: the stepping time is displayed after the step and is refreshed by the
	//   next (fast) step.
	private static void SteppingTime()
	{
		var checksum = SlowCall();
		Console.WriteLine($"stepping-time: checksum = {checksum}");
	}

	private static int SlowCall()
	{
		Thread.Sleep(750);
		return 42;
	}

	// SCENARIO "step-into-external-code" - README -> Basics -> Stepping 9.
	// SETUP: Settings | Build, Execution, Deployment | Debugger - enable external
	//   source debugging and make sure stepping into external code is NOT suppressed
	//   ("Do not step into external code" off / "Enable external source debug" on).
	// BREAKPOINT: on 'var joined = string.Join(", ", parts);'.
	// STEP INTO: the debugger opens the decompiled or source-linked String.Join and
	//   allows stepping through it; Step Out returns here.
	// EXPECTED: 'joined' == "alpha, beta, gamma".
	private static void StepIntoExternalCode()
	{
		var parts = new[] { "alpha", "beta", "gamma" };
		var joined = string.Join(", ", parts);
		Console.WriteLine($"step-into-external-code: {joined}");
	}
}