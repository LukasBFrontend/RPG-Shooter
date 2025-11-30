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
    public static RunState Status { get { return status; } }
    static RunState status = RunState.Running;
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
        if (status != RunState.Running)
        {
            return;
        }

        status = RunState.Paused;
        Time.timeScale = 0;
    }

    public static void Unpause()
    {
        if (status != RunState.Paused)
        {
            return;
        }

        status = RunState.Running;
        Time.timeScale = 1;
    }
}
