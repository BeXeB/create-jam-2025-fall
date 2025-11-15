using MafiaGame.Interfaces;

namespace MafiaGame.Weapons.MeleeWeapons;

public class SpikedKnuckles : IMeleeWeapon
{
    public float AttacksPerSecond { get; }
    public IWeapon LevelUp()
    {
        throw new NotImplementedException();
    }

    public float Range { get; }
    public float Damage { get; }
}