using UnityEngine;

public class Bootstraper : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
