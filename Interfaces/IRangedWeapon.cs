namespace MafiaGame.Interfaces;

public interface IRangedWeapon : IWeapon
{
    public float Spread { get; }
    public int ProjectileCount { get; }
    public IProjectile Projectile { get; }
}