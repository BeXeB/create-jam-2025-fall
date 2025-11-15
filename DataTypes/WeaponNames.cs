using Godot;

[GlobalClass]
public partial class WeaponNames : Resource
{
	[Export]
	public PackedScene weaponPrefab;
	[Export]
	public string name;
}
