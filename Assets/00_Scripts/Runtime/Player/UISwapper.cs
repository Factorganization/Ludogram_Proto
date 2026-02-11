using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UISwapper : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Sprite[] crosshairs;
    private Image crosshairImage;

    private void Start()
    {
        crosshairImage = canvas.GetComponentInChildren<Image>();
        SetUILayer().Forget();
    }

    private async UniTask SetUILayer()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        Debug.Log("change layer");
        canvas.gameObject.layer = 5;
        crosshairImage.gameObject.layer = 5;
    }

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

    public void SwitchCrosshairVisibility(bool state)
    {
        crosshairImage.enabled = state;
    }
}
