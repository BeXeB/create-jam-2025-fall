using Godot;
using System;

[GlobalClass]
public partial class MobWithLevelRequirements : Resource
{
	[Export]
	public PackedScene mobPrefab;
	[Export]
	public int levelRequirement;
}
