using Godot;
using System;
using System.Linq;

public partial class RepuOrb : Node2D
{
	[Export]
	public float ReputationExpAmount = 1;
	private Player _target;
	public bool IsBeingPickedUp = false;
	public override void _Ready()
	{
		_target = GetTree().GetNodesInGroup("Player").FirstOrDefault() as Player;
	}

	public override void _Process(double delta)
	{
		if (!IsBeingPickedUp)
		{
			if (Position.DistanceTo(_target.Position) < _target.PickUpRadius)
			{
				IsBeingPickedUp = true;
			}
			else
			{
				return;
			}
		}
		var direction = (_target.Position - Position).Normalized();
		Position += direction * 600 * (float)delta;

		if (Position.DistanceTo(_target.Position) < 5)
		{
			PickedUp();
			QueueFree();
		}
	}

	public void PickedUp()
	{
		_target.ReputationExp += ReputationExpAmount * _target.ReputationMultiplier;
	}
}
