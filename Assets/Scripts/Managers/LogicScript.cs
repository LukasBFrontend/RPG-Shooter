using UnityEngine;

public class LogicScript : Singleton<LogicScript>
{
    [SerializeField] private int coins = 0;
    public int Coins
    {
        get { return coins; }
    }

    public void AddCoins(int coins)
    {
        this.coins += coins;
    }

    public void SubtractCoins(int coins)
    {
        this.coins -= coins;
    }
}
