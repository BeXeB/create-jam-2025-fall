using MafiaGame.Interfaces;

namespace MafiaGame.Weapons.MeleeWeapons;

public class Knuckles : IMeleeWeapon
{
    public float AttacksPerSecond { get; }
    public IWeapon LevelUp()
    {
        return new SpikedKnuckles();
    }

    public float Range { get; }
    public float Damage { get; }
}