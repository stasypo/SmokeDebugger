namespace BlazorAppMultitarget.Services;

// Shared state for the /multiplayer page. Registered as a singleton, so every
// browser tab (every Blazor circuit) sees the same players and scores.
public sealed class MultiplayerGame
{
	private readonly object sync = new();
	private readonly Dictionary<Guid, Player> players = new();

	// Raised after any change. Subscribers run on the thread of the circuit that made
	// the change, so components must marshal back with InvokeAsync.
	public event Action? StateChanged;

	public IReadOnlyList<Player> Players
	{
		get
		{
			lock (sync)
			{
				return players.Values
					.OrderByDescending(p => p.Score)
					.ThenBy(p => p.Name)
					.Select(p => p with { })
					.ToList();
			}
		}
	}

	public Guid Join(string name)
	{
		var id = Guid.NewGuid();
		lock (sync)
		{
			players[id] = new Player(id, name, 0);
		}
		StateChanged?.Invoke();
		return id;
	}

	public void Leave(Guid id)
	{
		bool removed;
		lock (sync)
		{
			removed = players.Remove(id);
		}
		if (removed)
			StateChanged?.Invoke();
	}

	public void Score(Guid id)
	{
		lock (sync)
		{
			if (!players.TryGetValue(id, out var player))
				return;
			players[id] = player with { Score = player.Score + 1 };
		}
		StateChanged?.Invoke();
	}

	public void Reset()
	{
		lock (sync)
		{
			foreach (var id in players.Keys.ToList())
				players[id] = players[id] with { Score = 0 };
		}
		StateChanged?.Invoke();
	}
}

public sealed record Player(Guid Id, string Name, int Score);
