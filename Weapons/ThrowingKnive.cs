using Godot;
using MafiaGame.Interfaces;
using System;

public partial class ThrowingKnive : Node2D, IRangedWeapon
{
	[Export]
	private float _attacksPerSecond = 1f;
	[Export]
	private string _weaponName = "ThrowingKnive";
	[Export]
	private float _damage = 15f;
	[Export]
	private float _spread = 90f;
	[Export]
	private int _projectileCount = 1;
	private float _timeToAttack = 0f;

	public float Spread => _spread;
	public int ProjectileCount => _projectileCount;

	public IProjectile Projectile => throw new NotImplementedException();

	public float AttacksPerSecond => _attacksPerSecond;

	public float TimeToAttack { get => _timeToAttack; set => _timeToAttack = value; }

	public string WeaponName => _weaponName;

	public void LevelUp(string upgradeName)
	{
		switch (upgradeName)
		{
			case "Damage":
				_damage *= 1.1f;
				break;
			case "ProjectileCount":
				_projectileCount += 1;
				break;
			default:
				GD.PrintErr("Unknown upgrade: " + upgradeName);
				break;
		}
	}

	public void Attack(Vector2 mousePosition)
	{
		//Throw a projectile towards the mouse position
		// For each extra projectile add 2 to the side in a W shape, so at 2 projectiles we fire 3 (total) at -15, 0, +15 degrees, at 3 projectiles we fire 5 (total) at -30, -15, 0, +15, +30 degrees, etc.
		var direction = (mousePosition - GlobalPosition).Normalized();
		if (ProjectileCount == 1)
		{
			var projectileInstance = GD.Load<PackedScene>("res://Weapons/Projectiles/ThrowingKniveProjectile.tscn").Instantiate<IProjectile>();
			(projectileInstance as Node2D).GlobalPosition = GlobalPosition;
			GetTree().CurrentScene.AddChild(projectileInstance as Node2D);
			projectileInstance.Launch(direction, _damage * GetParent<Player>().DamageBonus);	
			return;
		}
		var angleStep = Spread / (ProjectileCount - 1);
		var startAngle = -Spread / 2;

		for (int i = 0; i < ProjectileCount; i++)
		{
			var angle = startAngle + i * angleStep;
			var rotatedDirection = direction.Rotated(Mathf.DegToRad(angle));	
			var projectileInstance = GD.Load<PackedScene>("res://Weapons/Projectiles/ThrowingKniveProjectile.tscn").Instantiate<IProjectile>();
			(projectileInstance as Node2D).GlobalPosition = GlobalPosition;
			GetTree().CurrentScene.AddChild(projectileInstance as Node2D);
			projectileInstance.Launch(rotatedDirection, _damage * GetParent<Player>().DamageBonus);	
		}
	}
}
