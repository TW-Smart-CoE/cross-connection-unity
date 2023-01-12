using UnityEngine;

public class SceneSingleton<T> : MonoBehaviour where T : SceneSingleton<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        Instance = (T) this;
    }
}
