using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; set; }
    protected abstract void OnAwake();

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        OnAwake();

        Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}
