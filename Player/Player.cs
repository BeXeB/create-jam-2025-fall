using Godot;
using MafiaGame.Interfaces;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Node2D
{
	[Signal]
	public delegate void DeadEventHandler();	
	[Signal]
	public delegate void HealthChangedEventHandler(float newHealth, float maxHealth);
	[Signal]
	public delegate void ReputationExpChangedEventHandler(float reputationExp, float reputationExpToNextLevel);
	[Signal]
	public delegate void ReputationUpEventHandler(float newReputationLevel);
	[Export]
	public float StartMaxHealth { get; set; } = 50;
	[Export]
	public float StartMovementSpeed { get; set; } = 200;
	[Export]
	public float StartReputationExpToNextLevel { get; set; } = 5;
	[Export]
	public float StartReputationMultiplier { get; set; } = 1;
	[Export]
	public float StartPickUpRadius { get; set; } = 100;
	[Export]
	public float StartAttackSpeed { get; set; } = 1;
	[Export]
	public WeaponNames[] WeaponNames;
	[Export]
	public Camera2D camera;
	public List<IWeapon> weapons;
	
	private float _health;
	private float _maxHealth;
	private float _movementSpeed;
	private float _reputationExp;
	private float _reputationExpToNextLevel;
	private float _reputation;
	private float _reputationMultiplier;
	private float _pickUpRadius;
	private float _attackSpeed;

	public float Health
	{
		get => _health;
		set
		{
			_health = value;
			EmitSignal(SignalName.HealthChanged, _health, MaxHealth);
		}
	}
	public float MaxHealth 
	{
		get => _maxHealth;
		set
		{
			_maxHealth = value;
			EmitSignal(SignalName.HealthChanged, Health, _maxHealth);
		}
	}
	public float MovementSpeed 
	{
		get => _movementSpeed;
		set => _movementSpeed = value;
	}
	public float ReputationExp
	{
		get => _reputationExp;
		set
		{
			_reputationExp = value;
			EmitSignal(SignalName.ReputationExpChanged, _reputationExp, ReputationExpToNextLevel);
			if (_reputationExp >= ReputationExpToNextLevel)
			{
				_reputationExp -= ReputationExpToNextLevel;
				Reputation += 1;
				ReputationExpToNextLevel *= 1.2f;
			}
		}
	}
	public float ReputationExpToNextLevel 
	{
		get => _reputationExpToNextLevel;
		set
		{
			_reputationExpToNextLevel = value;
			EmitSignal(SignalName.ReputationExpChanged, ReputationExp, _reputationExpToNextLevel);
		}
	}
	public float Reputation 
	{
		get => _reputation;
		set
		{
			_reputation = value;
			EmitSignal(SignalName.ReputationUp, _reputation);
		}
	}
	public float ReputationMultiplier 
	{
		get => _reputationMultiplier;
		set => _reputationMultiplier = value;
	}
	public float PickUpRadius 
	{
		get => _pickUpRadius;
		set => _pickUpRadius = value;
	}
	public float AttackSpeed 
	{
		get => _attackSpeed;
		set => _attackSpeed = value;
	}
	private bool _isDead = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		_isDead = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_isDead)
		{
			return;
		}

		foreach (var weapon in weapons)
		{
			weapon.TimeToAttack -= (float)delta;

			if (weapon.TimeToAttack < 0)
			{
				var mousePos = camera.GetGlobalMousePosition();
				weapon.Attack(mousePos);
				weapon.TimeToAttack = 1 / (weapon.AttacksPerSecond * AttackSpeed);
			}
		}

		var velocity = Vector2.Zero;

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}

		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}
		
		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * MovementSpeed;
		}
		
		Position += velocity * (float)delta;
	}

	public void Start()
	{
		Position = new Vector2(0,0);
		Show();
		GetNode<CollisionShape2D>("Player Area/CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		ResetStats();
		ResetWeapons();
	}

	private void ResetWeapons()
	{
		weapons = [];
		AddWeapon();
	}

	private void ResetStats()
	{
		MaxHealth = StartMaxHealth;
		Health = MaxHealth;
		MovementSpeed = StartMovementSpeed;
		Reputation = 0;
		ReputationExpToNextLevel = StartReputationExpToNextLevel;
		ReputationExp = 0;
		ReputationMultiplier = StartReputationMultiplier;
		PickUpRadius = StartPickUpRadius;
		AttackSpeed = StartAttackSpeed;
		_isDead = false;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Mobs"))
		{
			// Process melee dm
			var enemy = body.GetParent() as IEnemy;
			if (enemy.IsInTimeOut())
			{
				return;
			}
			Health -= enemy.Damage;
			enemy.SetDamageTimeout();
		} else if (body.IsInGroup("MobProjecties"))
		{
			// Process projectile dmg
			var projectile = body.GetParent() as IProjectile;
			Health -= projectile.Damage;
			body.QueueFree();
		}

		if (Health <= 0)
		{
			Hide();
			EmitSignal(SignalName.Dead);
			_isDead = true;
			GetNode<CollisionShape2D>("Player Area/CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		}
	}

	public void ApplyUpgrade(string upgradeName)
	{
		switch (upgradeName)
		{
			case "IncreaseMaxHealth":
				MaxHealth *= 1.1f;
				Health = MaxHealth;
				break;
			case "IncreaseMovementSpeed":
				MovementSpeed *= 1.1f;
				break;
			case "IncreaseAttackSpeed":
				AttackSpeed *= 1.1f;
				break;
			case "IncreaseReputationMultiplier":
				ReputationMultiplier *= 1.1f;
				break;
			case "IncreasePickupRadius":
				PickUpRadius *= 1.1f;
				break;
			case "UnlockKnuckles":
				AddWeapon("Knuckles");
				break;
			case "UnlockBaseballBat":
				AddWeapon("BaseballBat");
				break;
			case "UnlockThrowingKnive":
				AddWeapon("ThrowingKnive");
				break;
			default:
				foreach (var weapon in weapons)
				{	
					if (upgradeName.Contains(weapon.WeaponName))
					{
						upgradeName = upgradeName.Replace(weapon.WeaponName, "").Replace("Increase", "");
						weapon.LevelUp(upgradeName);
					}
				}
				break;
		}
	}

	private void AddWeapon(string weaponName = "Knuckles")
	{
		var newWeapon = WeaponNames.Where(wn => wn.name == weaponName).FirstOrDefault().weaponPrefab.Instantiate<Node2D>();
		AddChild(newWeapon);
		newWeapon.Position = Vector2.Zero;
		var knucklesWeapon = newWeapon as IWeapon;
		knucklesWeapon.TimeToAttack = 1 / (knucklesWeapon.AttacksPerSecond * AttackSpeed);
		weapons.Add(newWeapon as IWeapon);
	}
}
