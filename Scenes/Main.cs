using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Main : Node2D
{

	[Export]
	public MobWithLevelRequirements[] mobsWithLevelRequirements;

	[Export]
	public Player player;

	public float baseSpawnRate = 1; //x per second
	public float spawnRateMultiplierPerLevel = 0.1f;
	private float timeToSpawn = 1;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timeToSpawn -= (float)delta * baseSpawnRate * (1 + player.level * spawnRateMultiplierPerLevel);

		if (timeToSpawn < 0)
		{
			timeToSpawn = 1;

			// Spawn enemy
			var rand = new Random();
			var enemyPrefab = mobsWithLevelRequirements
								.Where(mlvl => mlvl.levelRequirement <= player.level)
								.OrderBy(x => rand.Next())
								.FirstOrDefault().mobPrefab;
			var mob = enemyPrefab.Instantiate<Node2D>();
			mob.Position = new Vector2((GD.Randf()-.5f)*1000,(GD.Randf()-.5f)*1000);
			AddChild(mob);
		}
	}
}
