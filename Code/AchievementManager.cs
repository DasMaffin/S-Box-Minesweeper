public class AchievementManager
{
	private static AchievementManager _instance;
	public static AchievementManager Instance
	{
		get
		{
			if ( _instance == null )
			{
				_instance = new AchievementManager();
			}
			return _instance;
		}
	}

	private Dictionary<string, Achievement> _achievements = new Dictionary<string, Achievement>();

	private AchievementManager()
	{
		// Initialize achievements here
		AddAchievement("20_wins", "Win on 20 boards", "Win a total of 20 times on a board with 100 fields or more.", 20);
		AddAchievement("20_mines", "Win on a board with 20 or more mines!", "Generate a board that has 20 or more mines and find all of them!", 1);
		// Add more achievements as needed
	}

	private void AddAchievement(string id, string name, string description, int target)
	{
		if ( !_achievements.ContainsKey(id) )
		{
			_achievements[id] = new Achievement(id, name, description, target);
		}
	}

	public void ProgressAchievement(string id, int progressAmount = 1)
	{
		if ( _achievements.TryGetValue(id, out Achievement achievement) )
		{
			achievement.AddProgress(id, progressAmount);
		}
	}

	public void ResetAchievement(string id)
	{
		if ( _achievements.TryGetValue(id, out var achievement) )
		{
			achievement.ResetProgress();
		}
	}

	public IEnumerable<Achievement> GetAchievements()
	{
		return _achievements.Values;
	}

	public Achievement GetAchievement(string id)
	{
		return _achievements.TryGetValue(id, out var achievement) ? achievement : null;
	}
}
