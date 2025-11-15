using Godot;

namespace MafiaGame.Interfaces;

public interface IProjectile
{
    public float Damage { get; }
    public float Speed { get; }
    public float Lifetime { get; }
    public void Launch(Vector2 direction, float damage);
}