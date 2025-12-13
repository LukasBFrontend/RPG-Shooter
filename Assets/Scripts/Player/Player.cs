using UnityEngine;

public class Player : Character
{
    public static Config Config { get; private set; }
    public static Movement Movement { get; private set; }
    public static Actions Actions { get; private set; }
    public static State State { get; private set; }

    void Start()
    {
        Cache();
        SetMaxHealth(Health);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Cache()
    {
        Config = Config.Instance;
        Actions = Actions.Instance;
        Movement = Movement.Instance;
        State = State.Instance;

        OnDeath = () =>
        {
            State.Respawn();
            Debug.Log("Player died");
        };
    }

}
