namespace ConsoleApp1.Scenarios;

/// <summary>
/// Variables, watches and evaluation scenarios - README -> Basics -> Threads &amp; Variables,
/// README -> Basics -> Evaluation and README -> More -> Evaluation.
/// Visualizers live in <see cref="VisualizerScenarios"/>.
/// </summary>
internal static class EvaluationScenarios
{
	public static void Run()
	{
		Console.WriteLine("--- evaluation ---");
		VariablesAndWatches();
		ImmediateWindowAndSyntheticVariable();
		GenericFrameInfo();
		PredictiveBooleans();
		PredictiveLoop();
	}

	// SCENARIO "variables-and-watches" - README -> Basics -> Threads & Variables 3-5,
	//   README -> More -> Threads & Variables 3-6, README -> Basics -> Evaluation.
	// BREAKPOINT: on 'Console.WriteLine(Summarize(order));' below.
	// HOVER: hover over 'order' in the editor - the tooltip shows the object and can be
	//   expanded to its fields.
	// WATCH: add the regular watch 'order.Total * 2' -> 250.00; add an inlay watch for
	//   'order.Customer' -> "Contoso". Pause the watch, step, and verify it stops
	//   re-evaluating until it is resumed.
	// SET VALUE: set 'order.IsPaid' to true, then resume - the app prints "PAID"
	//   instead of "UNPAID", which proves the running app used the new value.
	// PIN TO TOP: pin 'Customer' inside 'order' and verify it stays at the top of the
	//   expanded node.
	// MARK OBJECT: mark 'order' (for example as 'theOrder') and evaluate
	//   'theOrder_DebugLabel.Total' in the Immediate window.
	// COPY NAME/VALUE + INSPECT: use the Variables context menu on 'order.Items'.
	private static void VariablesAndWatches()
	{
		var order = new Order
		{
			Id = 1001,
			Customer = "Contoso",
			Total = 125.00m,
			IsPaid = false,
			Items = { "keyboard", "mouse", "monitor" }
		};

		Console.WriteLine(Summarize(order));
	}

	private static string Summarize(Order order) =>
		$"variables-and-watches: order {order.Id} for {order.Customer}, " +
		$"total {order.Total}, {order.Items.Count} items, {(order.IsPaid ? "PAID" : "UNPAID")}";

	// SCENARIO "immediate-window-and-synthetic-variable"
	//   README -> More -> Evaluation 1-3.
	// BREAKPOINT: on the Console.WriteLine below.
	// EVALUATE (Immediate window and Evaluation dialog):
	//   'basket.Count'            -> 4
	//   'basket[2].ToUpper()'     -> "CHERRY"
	//   'basket.Sum(x => x.Length)' -> 21
	//   'basket.NoSuchMember'     -> a clear evaluation error, not a crash.
	// SYNTHETIC VARIABLE: in the Immediate window run 'var doubled = basket.Count * 2;'
	//   and verify 'doubled' (== 8) shows up in Variables as a synthetic variable and
	//   survives the next step.
	private static void ImmediateWindowAndSyntheticVariable()
	{
		var basket = new List<string> { "apple", "banana", "cherry", "date" };
		Console.WriteLine($"immediate-window: {basket.Count} items");
	}

	// SCENARIO "generic-frame-info"
	//   README -> More -> Threads & Variables 2.4.3-2.4.5 (frame presentation options).
	// BREAKPOINT: inside Label<T>.
	// ACTION: in the Frames view toggle Show Method Return Value, Method Parameters,
	//   Method Generic Parameters and Raw Frame Presentation.
	// EXPECTED: the frame is presented as 'Label<System.Int32>(value: 5)' with the
	//   options on, collapses to 'Label' with them off, and the method return value
	//   ("Int32:5") appears in Variables after stepping out.
	private static void GenericFrameInfo()
	{
		Console.WriteLine($"generic-frame-info: {Label(5)}");
	}

	private static string Label<T>(T value) => $"{typeof(T).Name}:{value}";

	// SCENARIO "predictive-boolean" - README -> More -> Evaluation 7.
	// BREAKPOINT: on 'if (hasAccess && !isExpired)'.
	// EXPECTED: before stepping, the debugger already shows the predicted result of the
	//   condition (true) and which branch will be taken. Set 'isExpired' to true and
	//   verify the prediction flips to false / the else branch.
	// WATCH: 'canDelete' == false (isAdmin is false only after the if-branch runs).
	private static void PredictiveBooleans()
	{
		bool hasAccess = true;
		bool isExpired = false;
		bool isAdmin = false;

		if (hasAccess && !isExpired)
		{
			Console.WriteLine("predictive-boolean: access granted");
		}
		else
		{
			Console.WriteLine("predictive-boolean: access denied");
		}

		bool canDelete = isAdmin && hasAccess && !isExpired;
		Console.WriteLine($"predictive-boolean: canDelete = {canDelete}");
	}

	// SCENARIO "predictive-loop" - README -> More -> Evaluation 8.
	// BREAKPOINT: on the 'foreach' line.
	// EXPECTED: the debugger shows the predicted per-iteration values / execution flow
	//   for the loop. 3, 6 and 9 satisfy the condition, so 'matched' is 3 after the loop.
	private static void PredictiveLoop()
	{
		var numbers = Enumerable.Range(1, 10).ToList();
		int matched = 0;

		foreach (var n in numbers)
		{
			if (n % 3 == 0)
			{
				matched++;
			}
		}

		Console.WriteLine($"predictive-loop: matched = {matched}");
	}

	/// <summary>Object with mixed member kinds, used by the Variables view scenarios.</summary>
	private sealed class Order
	{
		public int Id { get; init; }
		public string Customer { get; init; } = "";
		public decimal Total { get; init; }
		public bool IsPaid { get; set; }
		public List<string> Items { get; } = new();
	}
}
