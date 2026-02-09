using UnityEngine;
using UnityEngine.UI;

public class UISwapper : MonoBehaviour
{
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Sprite[] crosshairs;
    
    public enum InteractionUI
    {
        NONE,
        DRIVE,
        REPAIR,
        DESTROY,
        ELSE
    }

    public void SwapCrosshair(InteractionUI crosshair)
    {
        switch (crosshair)
        {
            case InteractionUI.NONE:
                crosshairImage.sprite = crosshairs[0];
                break;
            case InteractionUI.DRIVE:
                crosshairImage.sprite = crosshairs[1];
                break;
            case InteractionUI.REPAIR:
                crosshairImage.sprite = crosshairs[2];
                break;
            case InteractionUI.DESTROY:
                crosshairImage.sprite = crosshairs[3];
                break;
            case InteractionUI.ELSE:
                crosshairImage.sprite = crosshairs[4];
                break;
        }
    }
}
