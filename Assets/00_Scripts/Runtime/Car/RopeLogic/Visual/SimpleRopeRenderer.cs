using UnityEngine;

public class SimpleRopeRenderer : MonoBehaviour
{
    [Header("Rope Points")]
    [SerializeField] private Transform startPoint;
    [SerializeField] public Transform endPoint;
    
    [Header("Visual Settings")]
    [SerializeField] private int curveResolution = 20;
    [SerializeField] private float ropeWidth = 0.1f;
    [SerializeField] private Material ropeMaterial;
    
    [Header("Catenary Settings (Rope Sag)")]
    [SerializeField] private float sagAmount = 2f; // How much the rope sags
    [SerializeField] private float swaySpeed = 1f;
    [SerializeField] private float swayAmount = 0.2f;
    
    private LineRenderer lineRenderer;
    private float swayOffset;

    void Start()
    {
        SetupLineRenderer();
        swayOffset = Random.Range(0f, 100f);
        
        // set gameobject layer to default
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    void SetupLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        
        lineRenderer.positionCount = curveResolution;
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;
        lineRenderer.material = ropeMaterial != null ? ropeMaterial : new Material(Shader.Find("Sprites/Default"));
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.numCornerVertices = 5;
        lineRenderer.numCapVertices = 5;
    }

    void Update()
    {
        if (startPoint == null || endPoint == null)
        {
            lineRenderer.enabled = false;
            return;
        }
        
        lineRenderer.enabled = true;
        DrawRopeCurve();
    }

    void DrawRopeCurve()
    {
        Vector3 start = startPoint.position;
        Vector3 end = endPoint.position;
        
        float distance = Vector3.Distance(start, end);
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
        
        // Add some sway for realism
        float sway = Mathf.Sin(Time.time * swaySpeed + swayOffset) * swayAmount;

        for (int i = 0; i < curveResolution; i++)
        {
            float t = i / (float)(curveResolution - 1);
            
            // Linear interpolation between points
            Vector3 position = Vector3.Lerp(start, end, t);
            
            // Add catenary curve (sag in the middle)
            float sagCurve = Mathf.Sin(t * Mathf.PI) * sagAmount * (distance / 10f);
            position += Vector3.down * sagCurve;
            
            // Add slight sway perpendicular to rope
            float swayCurve = Mathf.Sin(t * Mathf.PI) * sway;
            position += perpendicular * swayCurve;
            
            lineRenderer.SetPosition(i, position);
        }
    }

    // Draw gizmos for easy setup
    void OnDrawGizmosSelected()
    {
        if (startPoint != null && endPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPoint.position, endPoint.position);
            Gizmos.DrawWireSphere(startPoint.position, 0.2f);
            Gizmos.DrawWireSphere(endPoint.position, 0.2f);
        }
    }
}