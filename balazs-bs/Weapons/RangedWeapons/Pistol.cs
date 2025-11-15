using MafiaGame.Interfaces;
using MafiaGame.Weapons.Projectiles;

namespace MafiaGame.Weapons.RangedWeapons;

public class Pistol : IRangedWeapon
{
    public float AttacksPerSecond { get; }
    public IWeapon LevelUp()
    {
        throw new NotImplementedException();
    }

    public float Range { get; }
    public float Spread { get; }
    public IProjectile Projectile { get; } = new Bullet();
}