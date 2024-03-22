using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    private int gold;

    public static GameManager Instance
    {
        get
        {
            if (instance is null)
            {
                return null;
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance is null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
}