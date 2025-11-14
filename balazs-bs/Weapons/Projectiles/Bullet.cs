using MafiaGame.Interfaces;

namespace MafiaGame.Weapons.Projectiles;

public class Bullet : IProjectile
{
    public float Damage { get; }
    public float Speed { get; }
}