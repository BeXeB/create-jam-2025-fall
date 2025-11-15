using Godot;
using System;

public partial class BossEnemy : BaseEnemy
{
	public override void Die()
	{
		// Custom death behavior for BossEnemy
		// Collect All
		// Increase Player Reputation
		// Healt
		target.ReputationExp += target.ReputationExpToNextLevel;
		target.Health = target.MaxHealth;
		// Scan all exp orbs and collect them
		var expOrbs = GetTree().GetNodesInGroup("RepuOrb");
		foreach (var orb in expOrbs)
		{
			var repuOrb = orb as RepuOrb;
			if (repuOrb != null)
			{
				repuOrb.IsBeingPickedUp = true;
			}
		}
		base.Die(); // Call the base class Die method to handle common death logic
	}
}
