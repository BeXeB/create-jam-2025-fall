namespace MafiaGame.Interfaces;

public interface IEnemy
{
    // Temp untill enemies actually have weapons
    public float Damage { get; set; }
    public float ExperienceMultiplier { get; set; }
    public float MaxHealth { get; set; }
    public float Health { get; set; }
    public float MovementSpeed { get; set; }
    public IWeapon Weapon { get; set; }
    public void SetDamageTimeout();
    public bool IsInTimeOut();
    public void Die();
}