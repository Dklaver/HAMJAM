using UnityEngine;
using UnityEngine.InputSystem; // Required for new Input System

public class BulletControl : MonoBehaviour
{
    public Transform bulletReference;
    public LayerMask planeLayer;
    public float moveSpeed = 10f;

    private Camera mainCamera;
    private Vector3 currentTargetWorld;

    void Start()
    {
        mainCamera = Camera.main;
        currentTargetWorld = transform.position;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (Mouse.current == null) return; // Safe check

        // Read mouse position from new Input System
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, planeLayer))
        {
            Vector3 localHit = bulletReference.InverseTransformPoint(hit.point);
            Vector3 newLocalTarget = new Vector3(localHit.x, localHit.y, bulletReference.InverseTransformPoint(transform.position).z);
            currentTargetWorld = bulletReference.TransformPoint(newLocalTarget);
        }

        transform.position = Vector3.Lerp(transform.position, currentTargetWorld, Time.deltaTime * moveSpeed);
    }
}
