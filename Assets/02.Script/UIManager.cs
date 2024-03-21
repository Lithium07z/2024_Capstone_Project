using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance is null)
                instance = null;
            return instance;
        }
    }

    void Awake()
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

        SetCursorForUI(false);
    }


    [SerializeField] private Image crosshair;


    public void SetActiveCrosshair(bool state)
    {
        crosshair.enabled = state;
    }

    private bool cursorInputForLook = true;
    private bool isMovingAllowed = true;

    public bool GetCursorInput()
    {
        return cursorInputForLook;
    }

    public bool GetIsMovingAllowed()
    {
        return isMovingAllowed;
    }

    public void SetCursorForUI(bool state)
    {
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
        cursorInputForLook = !state;
        isMovingAllowed = !state;
    }
}