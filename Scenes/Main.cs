using Godot;
using MafiaGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Main : Node2D
{

	[Export]
	public MobWithLevelRequirements[] mobsWithLevelRequirements;
	[Export]
	public MobWithLevelRequirements[] bossMobsWithLevelRequirements;

	[Export]
	public Player player;
	[Export]
	public float spawnRadius = 1100;
	[Export]
	public float baseSpawnRate = 1; //x per second
	[Export]
	public float spawnRateMultiplierPerLevel = 0.1f;
	[Export]
	public float baseEnemyXpMuiltiplierPerLevel = 0.1f;
	[Export]
	public float enemyBaseHealthMultiplierPerLevel = 0.025f;
	private float _timeToSpawn = 1;
	private bool _isGameOver = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_isGameOver)
		{
			return;
		}

		_timeToSpawn -= (float)delta * baseSpawnRate * (1 + player.Reputation * spawnRateMultiplierPerLevel);

		if (_timeToSpawn < 0)
		{
			_timeToSpawn = 1;

			// Spawn enemy
			var rand = new Random();
			var enemyPrefab = mobsWithLevelRequirements
								.Where(mlvl => mlvl.levelRequirement <= player.Reputation)
								.OrderBy(x => rand.Next())
								.FirstOrDefault().mobPrefab;
			var mob = enemyPrefab.Instantiate<Node2D>();
			var angleRad = GD.Randf() * Mathf.Pi * 2;
			var spawnPosition = new Vector2(Mathf.Cos(angleRad)*spawnRadius,Mathf.Sin(angleRad)*spawnRadius);
			mob.Position = spawnPosition + player.Position;
			var enemy = mob as IEnemy;
			enemy.ExperienceMultiplier *= 1 + (player.Reputation * baseEnemyXpMuiltiplierPerLevel); 
			enemy.MaxHealth *= 1 + (player.Reputation * enemyBaseHealthMultiplierPerLevel);
			enemy.Health = enemy.MaxHealth;
			AddChild(mob);
		}
	}

	private void OnRepuUp(float newRepu)
    {
		if (newRepu <= 0)
		{
			return;
		}
		foreach (var boss in bossMobsWithLevelRequirements)
		{
			if (newRepu % boss.levelRequirement != 0)
            {
				continue;
            }

			//Spawn boss
			var mob = boss.mobPrefab.Instantiate<Node2D>();
			var angleRad = GD.Randf() * Mathf.Pi * 2;
			var spawnPosition = new Vector2(Mathf.Cos(angleRad)*spawnRadius,Mathf.Sin(angleRad)*spawnRadius);
			mob.Position = spawnPosition + player.Position;
			var enemy = mob as IEnemy;
			enemy.ExperienceMultiplier = 1 + (player.Reputation * baseEnemyXpMuiltiplierPerLevel); 
			enemy.MaxHealth *= 1 + (player.Reputation * enemyBaseHealthMultiplierPerLevel);
			enemy.Health = enemy.MaxHealth;
			AddChild(mob);
		}
        // Pause the game, while the player selects an upgrade
		GetTree().Paused = true;
		var uiScene = GetNode<UiScene>("HUD");
		//Generate upgrade options here and pass to UI
		List<(string displayText, string upgradeName)> upgradeOptions = [
			("Increase Max Health by 10%", "IncreaseMaxHealth"),
			("Increase Movement Speed by 10%", "IncreaseMovementSpeed"),
			("Increase Attack Speed by 10%", "IncreaseAttackSpeed"),
			("Increase Reputation Gain by 10%", "IncreaseReputationMultiplier"),
			("Increase Pickup Radius by 10%", "IncreasePickupRadius"),
			("Increase Damage Dealt by 10%", "IncreaseDamageDealt")
        ];
		foreach (var weapon in player.weapons)
		{
			// upgradeOptions.Add(($"Increase {weapon.WeaponName} Damage by 10%", $"Increase{weapon.WeaponName}Damage"));
			if (weapon is MafiaGame.Interfaces.IMeleeWeapon)
			{
				upgradeOptions.Add(($"Increase {weapon.WeaponName} Range by 10%", $"Increase{weapon.WeaponName}Range"));
			}
			else if (weapon is MafiaGame.Interfaces.IRangedWeapon)
			{
				upgradeOptions.Add(($"Increase {weapon.WeaponName} Projectile Count by 1", $"Increase{weapon.WeaponName}ProjectileCount"));
			}
		}
		foreach (var item in player.WeaponNames)
		{
			if (player.weapons.Any(w => w.WeaponName == item.name))
			{
				continue;
			}
			upgradeOptions.Add(($"Unlock {item.name} Weapon", $"Unlock{item.name}"));
		}
		var rand = new Random();
		var selectedUpgrades = upgradeOptions
								.OrderBy(x => rand.Next())
								.Take(3)
								.ToArray();

		uiScene.ShowRepuUp(selectedUpgrades);
    }

	private void OnRepuUpComplete(string upgradeName)
    {
		player.ApplyUpgrade(upgradeName);
        // Unpause the game
		GetTree().Paused = false;
		var uiScene = GetNode<UiScene>("HUD");
		uiScene.HideRepuUp();
    }

	private void OnPlayerDeath()
	{
		GameOver();
	}

	public void GameOver()
	{
		_isGameOver = true;
		GetNode<UiScene>("HUD").ShowGameOver();
	}

	public void NewGame()
	{
		GetTree().CallGroup("Mobs", Node.MethodName.QueueFree);
		GetTree().CallGroup("RepuOrb", Node.MethodName.QueueFree);
		_isGameOver = false;
		player.Start();
	}
}
