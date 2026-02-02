using MortierFu.Shared;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private Transform cameraRoot;

    [Header("Split-screen")]
    [Tooltip("1-based index: 1 for first player, 2 for second, ...")]
    [SerializeField] private int playerIndex = 1;
    [Tooltip("Total number of players / cameras")]
    [SerializeField] private int totalPlayers = 2;
    [Tooltip("Layer mask that this camera should render (e.g. P1 or P2)")]
    [SerializeField] private LayerMask playerLayer;

    // Public Getters
    public Camera PlayerMainCamera => playerCamera;
    public CinemachineCamera CinemachineCamera => cinemachineCamera;
    public Transform CameraRoot => cameraRoot;

    private PlayerInput playerInput;

    private void Awake()
    {
        if (playerCamera == null) Logs.LogError("[Player]: Player Main Camera is not assigned!", this);
        if (cinemachineCamera == null) Logs.LogError("[Player]: Cinemachine Brain is not assigned!", this);
        if (cameraRoot == null) Logs.LogError("[Player]: Camera Root is not assigned!", this);

        // Try to get the PlayerInput associated with this player 
        playerInput = GetComponent<PlayerInput>() ?? GetComponentInParent<PlayerInput>();

        if (playerInput != null)
        {
            // PlayerInput.playerIndex is zero-based
            playerIndex = playerInput.playerIndex + 1;
        }

        // Determine total players at runtime by counting active PlayerInput components
        var inputs = FindObjectsOfType<PlayerInput>();
        totalPlayers = Mathf.Clamp(Mathf.Max(1, inputs.Length), 1, 4);

        ConfigureSplitScreen(); // apply at runtime
    }

    // Apply changes in editor when values change
    private void OnValidate()
    {
        // keep sane values
        playerIndex = Mathf.Clamp(playerIndex, 1, Mathf.Max(1, totalPlayers));
        totalPlayers = Mathf.Clamp(totalPlayers, 1, 4);
        if (playerCamera != null)
            ConfigureSplitScreen();
    }

    // Call this to configure viewport and culling for this player's camera
    public void ConfigureSplitScreen()
    {
        if (playerCamera == null) return;

        // Set culling mask to only the assigned player layer (includes that layer, excludes others)
        playerCamera.cullingMask = playerLayer;

        // Simple handling for 1, 2, 3-4 players:
        Rect rect;
        if (totalPlayers == 1)
        {
            rect = new Rect(0f, 0f, 1f, 1f);
        }
        else if (totalPlayers == 2)
        {
            // side-by-side horizontal split
            float width = 0.5f;
            float x = (playerIndex == 1) ? 0f : width;
            rect = new Rect(x, 0f, width, 1f);
        }
        else // 3 or 4 -> 2x2 grid
        {
            int columns = 2;
            int rows = 2;
            int index = Mathf.Clamp(playerIndex - 1, 0, totalPlayers - 1);
            int col = index % columns;
            int row = index / columns;
            float w = 1f / columns;
            float h = 1f / rows;
            // Unity's viewport y=0 is bottom, so invert row
            rect = new Rect(col * w, 1f - (row + 1) * h, w, h);
        }

        playerCamera.rect = rect;
    }
}
