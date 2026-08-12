namespace ConsoleApp1.Scenarios;

/// <summary>
/// Visualizer scenarios - README -> More -> Evaluation 4-6.
/// </summary>
internal static class VisualizerScenarios
{
	public static void Run()
	{
		Console.WriteLine("--- visualizers ---");
		StringVisualizers();
		CollectionVisualizer();
		LinqVisualizer();
	}

	// SCENARIO "string-visualizer" - README -> More -> Evaluation 4.
	// BREAKPOINT: on the Console.WriteLine below.
	// ACTION: click the magnifier next to 'html', 'xml' and 'json' in the Variables
	//   view and open the HTML, XML and JSON viewers.
	// EXPECTED: each visualizer shows the complete (not truncated) value with the
	//   matching highlighting; the plain-text tab shows the same characters.
	private static void StringVisualizers()
	{
		var html = "<html><body><h1>Debugger</h1><p>Smoke&nbsp;test</p>" +
		           "<ul><li>one</li><li>two</li></ul></body></html>";

		var xml = "<order id=\"1001\"><customer>Contoso</customer>" +
		          "<items><item sku=\"kbd\">keyboard</item><item sku=\"mou\">mouse</item></items>" +
		          "</order>";

		var json = "{\"id\":1001,\"customer\":\"Contoso\",\"total\":125.00," +
		           "\"items\":[\"keyboard\",\"mouse\",\"monitor\"],\"paid\":false}";

		Console.WriteLine($"string-visualizer: {html.Length}/{xml.Length}/{json.Length} chars");
	}

	// SCENARIO "collection-visualizer" - README -> More -> Evaluation 5.
	// BREAKPOINT: on the Console.WriteLine below.
	// ACTION: open the collection visualizer on 'people' (50 rows).
	// EXPECTED: the DataGrip-like table lists 50 rows with Name/Age/City columns;
	//   sorting, filtering, paging and the export controls work. The first row is
	//   ("Person 01", 21, "Munich").
	private static void CollectionVisualizer()
	{
		var cities = new[] { "Berlin", "Munich", "Prague", "Amsterdam", "Lisbon" };
		var people = Enumerable.Range(1, 50)
			.Select(i => new Person($"Person {i:D2}", 20 + i % 40, cities[i % cities.Length]))
			.ToList();

		Console.WriteLine($"collection-visualizer: {people.Count} people");
	}

	// SCENARIO "linq-visualizer" - README -> More -> Evaluation 6.
	// BREAKPOINT: on 'var result = query.ToList();'.
	// ACTION: open the LINQ visualizer on 'query' and walk through the Where / OrderBy /
	//   Select steps.
	// EXPECTED: the visualizer shows the intermediate result of every operator and its
	//   final output matches 'result': ["Ada", "Alan", "Grace", "Margaret"] (4 items,
	//   Linus is filtered out because he is 28).
	private static void LinqVisualizer()
	{
		var people = new List<Person>
		{
			new("Ada", 36, "Berlin"),
			new("Alan", 41, "Munich"),
			new("Grace", 45, "Prague"),
			new("Linus", 28, "Amsterdam"),
			new("Margaret", 33, "Lisbon")
		};

		var query = people
			.Where(p => p.Age > 30)
			.OrderBy(p => p.Name)
			.Select(p => p.Name);

		var result = query.ToList();
		Console.WriteLine($"linq-visualizer: {string.Join(", ", result)}");
	}

	private sealed record Person(string Name, int Age, string City);
}
