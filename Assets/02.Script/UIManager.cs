using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // static instance
    private static UIManager instance;

    // shooting guns UI
    [SerializeField] private Image crosshair;
    
    // cursor UI
    private bool cursorInputForLook = true;
    private bool isMovingAllowed = true;

    // HUD - Need to Convert Text to Image-based Later
    [SerializeField] private TMP_Text hpTextTemp;
    [SerializeField] private TMP_Text maxHpTextTemp;
    [SerializeField] private TMP_Text spTextTemp;
    [SerializeField] private TMP_Text maxSpTextTemp;
    
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


    public void SetActiveCrosshair(bool state)
    {
        crosshair.enabled = state;
    }

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