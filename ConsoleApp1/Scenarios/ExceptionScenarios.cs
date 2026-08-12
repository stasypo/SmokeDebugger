namespace ConsoleApp1.Scenarios;

/// <summary>
/// Exception breakpoint scenarios - README -> Basics -> Breakpoints 2-3
/// and README -> [Before release only] JIT-debugger.
/// </summary>
internal static class ExceptionScenarios
{
	public static void Run()
	{
		Console.WriteLine("--- exceptions ---");
		UserCodeException();
		ExternalCodeException();
	}

	// SCENARIO "user-code-exception" - README -> Basics -> Breakpoints 2.
	// SETUP: Breakpoints dialog -> CLR Exception Breakpoints -> add
	//   System.InvalidOperationException, Break on "Thrown", "User code only" on.
	// EXCEPTION: the debugger must suspend on the throw inside ThrowUserException and
	//   the current frame must point at that user-code line (not at the catch).
	// EXPECTED: the exception popup shows the message "smoke-test user exception";
	//   resuming lets the catch block run and the app prints the caught message.
	private static void UserCodeException()
	{
		try
		{
			ThrowUserException();
		}
		catch (InvalidOperationException e)
		{
			Console.WriteLine($"user-code-exception: caught '{e.Message}'");
		}
	}

	private static void ThrowUserException() =>
		throw new InvalidOperationException("smoke-test user exception");

	// SCENARIO "external-code-exception" - README -> Basics -> Breakpoints 3.
	// SETUP: CLR Exception Breakpoint for System.FormatException with "User code only"
	//   OFF - the throw happens inside System.Private.CoreLib, not in this project.
	// EXCEPTION: the debugger suspends in external code.
	// EXPECTED: the Frames list shows the library frames (Number.ThrowOverflowOr
	//   FormatException / int.Parse) with this method as the caller; enabling
	//   Frames -> Frames from Libraries makes those frames visible.
	private static void ExternalCodeException()
	{
		try
		{
			var parsed = int.Parse("not-a-number");
			Console.WriteLine($"external-code-exception: unreachable {parsed}");
		}
		catch (FormatException e)
		{
			Console.WriteLine($"external-code-exception: caught '{e.Message}'");
		}
	}
}
