using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null) 
                instance = FindObjectOfType<UIManager>();
            return instance;
        }
    }


    [SerializeField] private Image crosshair;

    
    public void SetActiveCrosshair(bool state)
    {
        crosshair.enabled = state;
    }
}
