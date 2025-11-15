namespace MafiaGame.Interfaces;

public interface IRangedWeapon : IWeapon
{
    public float Range { get; }
    public float Spread { get; }
    public IProjectile Projectile { get; }
}