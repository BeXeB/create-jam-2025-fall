using System.Linq;
using Godot;
using MafiaGame.Interfaces;

public partial class BaseEnemy : Node2D, IEnemy
{
	[Export]
	private PackedScene weaponPrefab;
	[Export]
	private float _maxHealth = 10;
	[Export]
	private float _movementSpeed = 170;
	[Export]
	private float _damage = 5;
	private float _health;
	private IWeapon _weapon;
	[Export]
	private float _experienceMultiplier = 1;
	public float Damage { get => _damage; set => _damage = value; }
	public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }
	public float Health { get => _health; set => _health = value; }
	public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }
	public IWeapon Weapon { get => _weapon; set => _weapon = value; }
	public float ExperienceMultiplier { get => _experienceMultiplier; set => _experienceMultiplier = value; }

	protected Player target;
	private float _dmgTimeOut = 0;

	public override void _Ready()
	{
		Health = MaxHealth;
		// var weaponObj = weaponPrefab.Instantiate<Node2D>();
		// weaponObj.Position = Position;
		// AddChild(weaponObj);
		// var weapon = weaponObj as IWeapon;
		// weapon.TimeToAttack = 1 / weapon.AttacksPerSecond;
		// Weapon = weapon;
		target = GetTree().GetNodesInGroup("Player").FirstOrDefault() as Player;
	}

	public override void _Process(double delta)
	{
		// move towards player
		var dir = target.Position - Position;
		Position += dir.Normalized() * (float)delta * MovementSpeed;
		_dmgTimeOut -= (float)delta;
		//later add if in range attack with weapon
	}

	public virtual void Die()
	{
		//Spawn reputation orb
		var repuOrbPrefab = GD.Load<PackedScene>("res://Enemies/RepuOrb.tscn");
		var repuOrb = repuOrbPrefab.Instantiate<Node2D>();
		repuOrb.Position = Position;
		var repuOrbScript = repuOrb as RepuOrb;
		repuOrbScript.ReputationExpAmount *= ExperienceMultiplier;
		GetTree().CurrentScene.CallDeferred("add_child", repuOrb);
		QueueFree();
	}

	public void SetDamageTimeout()
	{
		_dmgTimeOut = .5f;
	}

	public bool IsInTimeOut() => _dmgTimeOut > 0;
}
