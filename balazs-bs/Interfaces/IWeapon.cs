namespace MafiaGame.Interfaces;

public interface IWeapon
{
    public float AttacksPerSecond { get; }
    public IWeapon LevelUp();
}