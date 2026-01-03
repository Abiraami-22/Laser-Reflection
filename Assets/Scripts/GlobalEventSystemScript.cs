using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalEventSystem : MonoBehaviour
{
    private static GlobalEventSystem instance;

    void Awake()
    {
        // Find ALL EventSystems in the game
        EventSystem[] systems = FindObjectsByType<EventSystem>(
            FindObjectsSortMode.None
        );

        // Keep the first one, destroy the rest
        for (int i = 1; i < systems.Length; i++)
        {
            Destroy(systems[i].gameObject);
        }

        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
