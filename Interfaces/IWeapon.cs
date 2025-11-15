using Godot;

namespace MafiaGame.Interfaces;

public interface IWeapon
{
    public float AttacksPerSecond { get; }
    public float TimeToAttack { get; set; }
    public string WeaponName { get; }
    public void LevelUp(string upgradeName);
    public void Attack(Vector2 mousePosition);
}