using System.Numerics;
using MafiaGame.Interfaces;

namespace MafiaGame.Enemies;

public class MafiaBoss : IEnemy
{
    private float _maxHealth;
    private float _health;
    private Vector2 _position;
    private IWeapon _weapon;

    float IEnemy.MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }

    float IEnemy.Health
    {
        get => _health;
        set => _health = value;
    }

    Vector2 IEnemy.Position
    {
        get => _position;
        set => _position = value;
    }

    IWeapon IEnemy.Weapon
    {
        get => _weapon;
        set => _weapon = value;
    }
}