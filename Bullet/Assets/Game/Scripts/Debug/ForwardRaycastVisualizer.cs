using UnityEngine;

[ExecuteAlways] // Runs in edit mode too
public class ForwardRaycastVisualizer : MonoBehaviour
{
    [Header("Ray Settings")]
    public float rayLength = 5f;
    public float rayWidth = 0.5f;
    public LayerMask hitLayers = Physics.DefaultRaycastLayers;

    [Header("Debug")]
    public Color rayColor = Color.green;
    public Color hitColor = Color.red;
    public bool performRaycast = true;

    private RaycastHit hitInfo;

    void Update()
    {
        // Only do physics checks during play mode (optional)
        if (!Application.isPlaying || !performRaycast)
            return;

        Physics.BoxCast(
            transform.position,
            new Vector3(rayWidth * 0.5f, rayWidth * 0.5f, rayWidth * 0.5f),
            transform.forward,
            out hitInfo,
            transform.rotation,
            rayLength,
            hitLayers
        );
    }

    void OnDrawGizmos()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        Vector3 endPoint = origin + direction * rayLength;

        // Draw center line
        Gizmos.color = rayColor;
        Gizmos.DrawLine(origin, endPoint);

        // Draw box volume (width)
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(
            origin + direction * (rayLength * 0.5f),
            transform.rotation,
            new Vector3(rayWidth, rayWidth, rayLength)
        );

        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = oldMatrix;

        // Draw hit point if detected
        if (performRaycast && Physics.BoxCast(
            origin,
            new Vector3(rayWidth * 0.5f, rayWidth * 0.5f, rayWidth * 0.5f),
            direction,
            out RaycastHit hit,
            transform.rotation,
            rayLength,
            hitLayers))
        {
            Gizmos.color = hitColor;
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
    }
}
