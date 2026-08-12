// =============================================================================
// SingleFileApp1 - the File-based app technology ("basics only").
// Run it with:  dotnet run SingleFileApp1.cs
//
// It must use top-level statements: a bare `static void Main()` in a file-based
// app is compiled as an unused local function (CS7022/CS8321) and the program
// does nothing, which makes it untestable.
//
// SCENARIO "file-based-app-session" - README -> Basics -> Core actions.
// EXPECTED: the file-based app starts under the debugger, Pause/Resume/Stop work,
//   and the debug session appears like any other .NET session.
//
// SCENARIO "console-during-pause-resume" - README -> More -> Console 1, 3 and 4.
// ACTION: let the app print a few lines, Pause, wait, then Resume.
// EXPECTED: output stops while suspended and continues from the next counter value
//   after Resume - no lost, reordered or duplicated lines.
//
// SCENARIO "attach-to-local-process" - README -> More -> Attach/Detach 1.
// ACTION: start it from a terminal without the debugger, then Run | Attach to Process.
// EXPECTED: attaching suspends on the breakpoint below; detaching leaves the app running.
//
// BREAKPOINT: on 'Console.WriteLine(...)' below, ideally with condition 'i == 10'
//   (README -> More -> Breakpoints 1.1 - conditional breakpoint).
// WATCH: 'i' increases by 1 per iteration; Set Value on 'i' changes the printed number.
// =============================================================================

long i = 0;
while (true)
{
    Console.WriteLine("some example text " + i);
    Thread.Sleep(500);
    i++;
}
