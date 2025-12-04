using UnityEngine;
public enum RunState
{
    Uninitialized,
    Running,
    Paused,
    GameOver
}
public static class GameState
{
    public static RunState Status { get; private set; } = RunState.Running;
    public static int Coins { get; set; }

    public static void AddCoins(int coins)
    {
        Coins += coins;
    }

    public static void SubtractCoins(int coins)
    {
        Coins -= coins;
    }

    public static void Pause()
    {
        if (Status != RunState.Running)
        {
            return;
        }

        Status = RunState.Paused;
        Time.timeScale = 0;
    }

    public static void Unpause()
    {
        if (Status != RunState.Paused)
        {
            return;
        }

        Status = RunState.Running;
        Time.timeScale = 1;
    }
}
