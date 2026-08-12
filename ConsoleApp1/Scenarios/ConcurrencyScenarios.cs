namespace ConsoleApp1.Scenarios;

/// <summary>
/// Threads, async stepping, Tasks and Parallel Stacks scenarios -
/// README -> Basics -> Threads &amp; Variables 1-2, More -> Threads &amp; Variables 1,
/// More -> Stepping, More -> Parallel Stacks and More -> Tasks.
/// </summary>
internal static class ConcurrencyScenarios
{
	public static async Task RunAsync()
	{
		Console.WriteLine("--- concurrency ---");
		ThreadsAndParallelStacks();
		await AsyncSteppingAsync();
		await TasksViewAsync();
	}

	// SCENARIO "threads-and-parallel-stacks"
	//   README -> Basics -> Threads & Variables 1-2, More -> Threads & Variables 1,
	//   README -> More -> Parallel Stacks.
	// SETUP: line breakpoint on the Console.WriteLine inside Tick, with the suspend
	//   policy set to All (breakpoint settings -> Suspend: All).
	// EXPECTED (Threads list): Worker-A, Worker-B and Worker-C are listed next to the
	//   main thread. Selecting one switches the frames and the values of 'name'/'i'.
	// EXPECTED (Stack frames): the frames are Tick -> Worker -> thread start, in that
	//   order; selecting a frame moves the editor and refreshes Variables.
	// ACTION: Freeze Worker-B, resume, and verify only A and C keep printing; unfreeze
	//   and verify B continues. Try Copy Stack, Copy All Stacks, Refresh Stack Frames,
	//   Show/Hide Threads and Navigate to Modules View / Parallel Stacks from here.
	// EXPECTED (Parallel Stacks): one branch per worker thread, all sharing the
	//   Worker/Tick frames; resume + pause refreshes the view.
	private static void ThreadsAndParallelStacks()
	{
		var workers = new List<Thread>();

		foreach (var name in new[] { "Worker-A", "Worker-B", "Worker-C" })
		{
			var thread = new Thread(() => Worker(name)) { Name = name, IsBackground = true };
			workers.Add(thread);
			thread.Start();
		}

		foreach (var thread in workers)
		{
			thread.Join();
		}
	}

	private static void Worker(string name)
	{
		for (int i = 1; i <= 5; i++)
		{
			Tick(name, i);
			Thread.Sleep(200);
		}
	}

	private static void Tick(string name, int i)
	{
		Console.WriteLine($"threads: [{name}] tick {i}");
	}

	// SCENARIO "async-stepping" - README -> More -> Stepping.
	// BREAKPOINT: on 'var raw = await LoadAsync(id);' inside BuildReportAsync.
	// STEP OVER: stepping follows the logical user-code flow across both awaits -
	//   LoadAsync, then TransformAsync, then the return statement - and never lands in
	//   compiler-generated MoveNext frames.
	// STEP INTO on the await: the debugger enters LoadAsync; Step Out returns to
	//   BuildReportAsync after the await, not to the state machine.
	// ACTION: toggle Frames -> Show Async Internals and verify the async infrastructure
	//   frames appear and disappear.
	// WATCH: 'raw' == "value-7", 'transformed' == "VALUE-7".
	// EXPECTED output: 'report#7: VALUE-7 (len 7)'.
	private static async Task AsyncSteppingAsync()
	{
		var report = await BuildReportAsync(7);
		Console.WriteLine($"async-stepping: {report}");
	}

	private static async Task<string> BuildReportAsync(int id)
	{
		var raw = await LoadAsync(id);
		var transformed = await TransformAsync(raw);
		return $"report#{id}: {transformed} (len {transformed.Length})";
	}

	private static async Task<string> LoadAsync(int id)
	{
		await Task.Delay(100);
		return $"value-{id}";
	}

	private static async Task<string> TransformAsync(string value)
	{
		await Task.Delay(100);
		return value.ToUpperInvariant();
	}

	// SCENARIO "tasks-view" - README -> More -> Tasks.
	// SETUP: line breakpoint on 'await Task.Delay(150 * n);' inside WorkAsync, suspend
	//   policy All, so several tasks are in flight when the debugger suspends.
	// EXPECTED: the Tasks window lists the tasks with their states; inspecting a
	//   task shows its details and stack; selecting a task frame navigates to WorkAsync
	//   and updates Variables ('n' differs per task).
	// ACTION: resume and pause again - the view refreshes. Switch between Table and
	//   Graph views: Table shows all tasks, Graph shows only the active ones.
	// EXPECTED output: 'tasks: 10, 20, 30, 40'.
	private static async Task TasksViewAsync()
	{
		var tasks = Enumerable.Range(1, 4).Select(WorkAsync).ToArray();
		var results = await Task.WhenAll(tasks);
		Console.WriteLine($"tasks: {string.Join(", ", results)}");
	}

	private static async Task<int> WorkAsync(int n)
	{
		await Task.Delay(150 * n);
		return n * 10;
	}
}
