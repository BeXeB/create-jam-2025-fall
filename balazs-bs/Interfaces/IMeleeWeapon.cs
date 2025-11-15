namespace MafiaGame.Interfaces;

public interface IMeleeWeapon : IWeapon
{
    public float Range { get; }
    public float Damage { get; }
}