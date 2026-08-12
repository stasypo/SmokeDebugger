namespace ConsoleApp1.Scenarios;

/// <summary>
/// Console output scenarios - README -> More -> Console.
/// </summary>
internal static class ConsoleScenarios
{
	// SCENARIO "console-output" - README -> More -> Console 1-4.
	// SETUP: no breakpoints needed for the first pass.
	// ACTION 1 (console-out): the even lines go to standard output. Verify
	//   "console-out: line N" appears exactly once for N = 2, 4, 6, 8, 10.
	// ACTION 2 (console-err): the odd lines go to the error stream. Verify
	//   "console-err: line N" appears once for N = 1, 3, 5, 7, 9, in the same console
	//   and highlighted differently from stdout.
	//   Together the ten lines must be strictly ordered 1..10, with no gaps or repeats.
	// ACTION 3 (Console.WriteLine): the text written here is exactly what shows up in
	//   the application console tab.
	// ACTION 4 (pause/resume): the loop runs for ~3 seconds; Pause in the middle,
	//   verify output stops, then Resume and verify the sequence continues from the
	//   next number without gaps or repeats.
	public static void Run()
	{
		Console.WriteLine("--- console ---");

		for (int line = 1; line <= 10; line++)
		{
			if (line % 2 == 0)
			{
				Console.Out.WriteLine($"console-out: line {line}");
			}
			else
			{
				Console.Error.WriteLine($"console-err: line {line}");
			}

			Thread.Sleep(300);
		}
	}
}
