namespace ConsoleApp1.Scenarios;

/// <summary>
/// Breakpoint scenarios - README -> Basics -> Breakpoints and More -> Breakpoints.
/// </summary>
internal static class BreakpointScenarios
{
	public static void Run()
	{
		Console.WriteLine("--- breakpoints ---");
		LineAndInlineBreakpoints();
		ConditionalBreakpoint();
		MethodBreakpoint();
		DataBreakpoint();
	}

	// SCENARIO "line-and-inline-breakpoint" - README -> Basics -> Breakpoints 1.
	// BREAKPOINT (line): on 'var sum = ...' - execution must stop at the start of the line.
	// BREAKPOINT (inline): the same line holds three calls. Use the inline breakpoint
	//   popup in the gutter and put a breakpoint on 'Square(b)' only.
	// EXPECTED: execution stops on that single expression, and the other two calls on
	//   the line do not suspend. 'sum' is 38 once the line completes (4 + 9 + 25).
	// ALSO USED FOR: Remove one breakpoint / Remove all / Restore / Mute
	//   (README -> Basics -> Breakpoints 4, More -> Breakpoints 2-5).
	private static void LineAndInlineBreakpoints()
	{
		int a = 2, b = 3;
		var sum = Square(a); sum = sum + Square(b) + Square(a + b);
		Console.WriteLine($"line-and-inline-breakpoint: sum = {sum}");
	}

	private static int Square(int x) => x * x;

	// SCENARIO "conditional-breakpoint" - README -> More -> Breakpoints 1.1.
	// BREAKPOINT: on the Console.WriteLine inside the loop, with condition 'i == 7'.
	// EXPECTED: the debugger suspends exactly once and 'i' is 7 in the Variables view.
	//   Removing the condition makes the same breakpoint suspend 10 times.
	private static void ConditionalBreakpoint()
	{
		for (int i = 0; i < 10; i++)
		{
			Console.WriteLine($"conditional-breakpoint: iteration {i}");
		}
	}

	// SCENARIO "method-breakpoint" - README -> More -> Breakpoints 1.2.
	// BREAKPOINT: ass a method breakpoint on MethodBreakpoint() method.
	// EXPECTED: execution suspends on method entry.
	private static void MethodBreakpoint()
	{
		for (int i = 1; i <= 3; i++)
		{
			Console.WriteLine(Visit($"call-{i}"));
		}
	}

	private static string Visit(string label)
	{
		return $"method-breakpoint: visited {label}";
	}

	// SCENARIO "data-breakpoint" - README -> More -> Breakpoints 1.3 [Before release only].
	// LIMITATIONS: Windows x64 only, watched field must be smaller than 8 bytes
	//   (Tick.Value is an int = 4 bytes), at most 4 data breakpoints at a time.
	// SETUP: put a line breakpoint on 'var tick = new Tick();', run to it, step over
	//   once, then right-click 'tick.Value' in Variables -> Set Data Breakpoint.
	//   Remove/mute the original line breakpoint before resuming.
	// EXPECTED: execution suspends inside Tick.Bump() on every write to Value, five
	//   times in total, and the final value printed is 5.
	private static void DataBreakpoint()
	{
		var tick = new Tick();
		for (int i = 0; i < 5; i++)
		{
			tick.Bump();
		}

		Console.WriteLine($"data-breakpoint: tick.Value = {tick.Value}");
	}

	private sealed class Tick
	{
		// The field watched by the data breakpoint.
		public int Value;

		public void Bump() => Value++;
	}
}
