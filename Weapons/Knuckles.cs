using Godot;
using MafiaGame.Interfaces;
using System;

public partial class Knuckles : Node2D, IMeleeWeapon
{
	[Export]
	private float _damage = 5;
	[Export]
	private float _attacksPerSecond = 1;
	private float _timeToAttack = 0;
	private float _rangeMultiplier = 1.0f;
	public float Damage => _damage;

	public float AttacksPerSecond => _attacksPerSecond;

	public float TimeToAttack { get => _timeToAttack; set => _timeToAttack = value; }

	public string WeaponName => "Knuckles";

	public void Attack(Vector2 mousePosition)
	{
		var dir = (mousePosition - GlobalPosition).Normalized();
		//Check for enemies in a box in front of the player
		var spaceState = GetWorld2D().DirectSpaceState;
		var boxSize = new Vector2(100*_rangeMultiplier, 60*_rangeMultiplier);
		var boxTransform = new Transform2D(0, GlobalPosition + dir * (boxSize.X/2));

		// Use an IntersectShape query with a RectangleShape2D instead of the nonexistent IntersectBox.
		var rectShape = new RectangleShape2D();
		rectShape.Size = boxSize;

		var query = new PhysicsShapeQueryParameters2D();
		query.Shape = rectShape;
		query.Transform = boxTransform;
		query.CollideWithBodies = true;
		query.CollideWithAreas = false;

		// Draw a temporary outline of the hit box for debugging
		var angle = (mousePosition - GlobalPosition).Angle();
		var debugTransform = new Transform2D(angle, GlobalPosition + dir * (boxSize.X/2));

		// rectangle corners relative to center
		var localCorners = new Vector2[]
		{
			new Vector2(-boxSize.X / 2, -boxSize.Y / 2),
			new Vector2(boxSize.X / 2, -boxSize.Y / 2),
			new Vector2(boxSize.X / 2, boxSize.Y / 2),
			new Vector2(-boxSize.X / 2, boxSize.Y / 2)
		};

		// Create a Line2D to show the outline
		var outline = new Line2D
		{
			Width = 2,
			DefaultColor = new Color(1f, 0f, 0f, 0.8f), // semi-transparent red
			Closed = true
		};

		// Place the Line2D so its local origin is at the box center in this node's local space
		outline.Position = debugTransform.Origin - GlobalPosition;

		// Use only the rotation (basis) to transform the corner offsets
		var rotationOnly = new Transform2D(angle, Vector2.Zero);
		foreach (var lc in localCorners)
			outline.AddPoint(rotationOnly.BasisXform(lc));

		AddChild(outline);

		// Remove the outline after a short time
		var t = new Timer
		{
			WaitTime = 0.2f,
			OneShot = true,
			Autostart = true
		};
		t.Timeout += () =>
		{
			outline.QueueFree();
			t.QueueFree();
		};
		AddChild(t);

		var result = spaceState.IntersectShape(query, 32);
		foreach (var hit in result)
		{
			if (hit.ContainsKey("collider"))
			{
				var colliderObj = (Node2D)hit["collider"];

				if (colliderObj.GetParent() is IEnemy enemy)
				{
					enemy.Health -= Damage;
					if (enemy.Health <= 0)
					{
						enemy.Die();
					}
				}
			}
		}
	}

	public void LevelUp(string upgradeName)
	{
		switch (upgradeName)
		{
			case "Damage":
				_damage *= 1.1f;
				break;
			case "Range":
				_rangeMultiplier *= 1.1f;
				break;
			default:
				GD.PrintErr($"Unknown upgrade name: {upgradeName}");
				break;
		}
	}
}
