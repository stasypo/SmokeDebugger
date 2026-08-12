using System.Runtime.InteropServices;

namespace ConsoleApp1.Scenarios;

/// <summary>
/// Mixed mode (managed + native) scenario - README -> Basics -> Evaluation -> Mixed mode debug.
/// </summary>
internal static class NativeInteropScenarios
{
	// Deliberately declared with DllImport (not LibraryImport) so that Step Into goes
	// straight from managed code into the native frame instead of into a generated
	// marshalling stub.
	[DllImport("kernel32.dll")]
	private static extern ulong GetTickCount64();

	[DllImport("kernel32.dll")]
	private static extern uint GetCurrentProcessId();

	// SCENARIO "mixed-mode-debug" - README -> Basics -> Evaluation -> Mixed mode debug.
	// SETUP: Windows x64 only. Edit the ConsoleApp1 run configuration and turn on native
	//   code debugging (mixed mode), then start a debug session.
	// BREAKPOINT: on 'var ticks = GetTickCount64();'.
	// STEP INTO: the debugger enters the native kernel32!GetTickCount64 frame.
	// EXPECTED: the Frames list contains both managed and native frames, stepping works
	//   in the native frame (Disassembly view).
	// NOTE: on non-Windows hosts this scenario is skipped - kernel32.dll does not exist.
	public static void Run()
	{
		Console.WriteLine("--- native interop ---");

		if (!OperatingSystem.IsWindows())
		{
			Console.WriteLine("mixed-mode-debug: skipped (Windows only)");
			return;
		}

		var ticks = GetTickCount64();
		var pid = GetCurrentProcessId();
		Console.WriteLine($"mixed-mode-debug: pid {pid}, uptime {ticks} ms");
	}
}
