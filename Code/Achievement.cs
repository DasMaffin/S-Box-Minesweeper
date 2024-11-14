using System;
using Sandbox.Services;

public class Achievement
{
	public string Id { get; private set; }
	public string Name { get; private set; }
	public string Description { get; private set; }
	public int Target { get; private set; }
	public int Progress { get; private set; }
	public bool IsUnlocked => Progress >= Target;

	public event Action<Achievement> OnUnlocked;

	public Achievement(string id, string name, string description, int target)
	{
		Id = id;
		Name = name;
		Description = description;
		Target = target;
		Progress = 0;
	}

	public void AddProgress(string id, int amount)
	{
		if ( IsUnlocked ) return;

		Progress += amount;
		if(Target != 1)
			Stats.Increment(id, amount);
		if ( Progress >= Target )
		{
			Progress = Target;
			Unlock(id);
		}
	}

	public void ResetProgress()
	{
		Progress = 0;
	}

	private void Unlock(string id)
	{
		OnUnlocked?.Invoke(this);
		Achievements.Unlock(id);
		Log.Info($"Achievement Unlocked: {Name} - {Description}");
	}
}
