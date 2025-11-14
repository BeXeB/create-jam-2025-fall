using System.Net.Http.Headers;
using System.Numerics;
using MafiaGame.Interfaces;

namespace MafiaGame;

public class Player
{
    public float Health { get; set;  }
    public float MaxHealth { get; set; }
    public float MovementSpeed { get; set; }
    public float Reputation { get; set; }
    public float ReputationMultiplier { get; set; }
    public float PickUpRadius { get; set; }
    public float AttackSpeed {get; set;}
    
    public List<IWeapon> Weapons { get; set; }
    public int EquippedWeaponIndex { get; set; }
    
    Vector2 Position { get; set; }

    public void UpgradeMaxHealth(float health)
    {
        MaxHealth += health;
    }

    public void UpgradeSpeed(float speed)
    {
        MovementSpeed += speed;
    }

    public void UpgradeAttackSpeed(float speed)
    {
        AttackSpeed *= speed;    
    }

    public void UpgradePickUpRadius(float radius)
    {
        PickUpRadius += radius;
    }

}