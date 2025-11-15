using System.Numerics;

namespace MafiaGame.Interfaces;

public interface IEnemy
{
    public float MaxHealth { get; set; }
    public float Health { get; set; }
    public Vector2 Position { get; set; }
    public IWeapon Weapon { get; set; }
}