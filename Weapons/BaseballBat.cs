using Godot;
using MafiaGame.Interfaces;
using System;

public partial class BaseballBat : Node2D, IMeleeWeapon
{
	[Export]
	private float _attacksPerSecond = .5f;
	[Export]
	private string _weaponName = "BaseballBat";
	[Export]
	private float _damage = 10;
	private float _timeToAttack = 0;
	private float _rangeMultiplier = 1f;
	public float AttacksPerSecond { get => _attacksPerSecond; }
	public float TimeToAttack { get => _timeToAttack; set => _timeToAttack = value; }
	public string WeaponName { get => _weaponName; }
	public float Damage { get => _damage; }

	public void Attack(Vector2 mousePosition)
	{
		//Check for enemies in a semi-circle in front of the player
		var spaceState = GetWorld2D().DirectSpaceState;
		var dir = (mousePosition - GlobalPosition).Normalized();
		var radius = 100 * _rangeMultiplier;
		var angle = Mathf.Pi;

		// Cast a circular sector to find enemies
		var query = new PhysicsShapeQueryParameters2D();
		var circleShape = new CircleShape2D();
		circleShape.Radius = radius;
		query.Shape = circleShape;
		query.Transform = new Transform2D(0, GlobalPosition);
		query.CollideWithBodies = true;
		query.CollideWithAreas = false;

		//Draw the outline of the semi-circle for debugging
		var outline = new Line2D
		{
			Width = 2,
			DefaultColor = new Color(0f, 1f, 0f, 0.8f), // semi-transparent green
			Closed = true
		};
		var segments = 3;
		for (int i = 0; i <= segments; i++)
		{
			var dirAngle = dir.Length() > 0.001f ? dir.Angle() : 0f;
			var segmentAngle = dirAngle - angle / 2 + (angle * i / segments);
			var point = GlobalPosition + new Vector2(Mathf.Cos(segmentAngle), Mathf.Sin(segmentAngle)) * radius;
			outline.AddPoint(ToLocal(point));
		}
		AddChild(outline);
		// Remove the outline after a short delay
		var timer = new Timer();
		timer.WaitTime = 0.1f;
		timer.OneShot = true;
		timer.Autostart = true;
		timer.Timeout += () =>
		{
			outline.QueueFree();
			timer.QueueFree();
		};
		AddChild(timer);

		var results = spaceState.IntersectShape(query);
		foreach (var hit in results)
		{
			if (hit.ContainsKey("collider"))
			{
				var collider = (Node2D)hit["collider"];
				
				// Check if within the angle
				var toCollider = (collider.GlobalPosition - GlobalPosition).Normalized();
				var dot = dir.Dot(toCollider);
				var hitAngle = Mathf.Acos(dot);
				if (hitAngle > angle / 2)
				{
					continue; // Outside of the semi-circle
				}

				if (collider.GetParent() is IEnemy enemy)
				{
					enemy.Health -= Damage * GetParent<Player>().DamageBonus;
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
