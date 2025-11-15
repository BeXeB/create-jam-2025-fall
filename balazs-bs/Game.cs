using MafiaGame.Interfaces;

namespace MafiaGame;

public class Game
{
    public Player Player;
    public List<IEnemy> Enemies;
    public float ReputationNeededForNextLevel;
    public int CurrentLevel;
}