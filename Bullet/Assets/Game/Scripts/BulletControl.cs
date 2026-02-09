using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletControl : MonoBehaviour
{
    public Transform bulletReference;
    public LayerMask planeLayer;
    public float moveSpeed = 10f;
    public float forwardSpeed = 1f;
    public float rotationSpeed = 90f; // degrees per second
    public float cameraDistance = 5f; // degrees per second

    public List<GameObject> listOfObjectsToRotate = new List<GameObject>();

    private Camera mainCamera;
    private Vector3 currentTargetWorld;
    private Vector3 cameraOriginalPosition;

    void Start()
    {
        mainCamera = Camera.main;
        currentTargetWorld = transform.position;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cameraOriginalPosition = mainCamera.transform.position;
    }

    void Update()
    {
        if (Mouse.current == null) return;

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

        MoveForward(forwardSpeed);
    }

    private void LateUpdate()
    {
        RotateObjects();
    }

    public void MoveForward(float speed)
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        mainCamera.gameObject.transform.position = new (cameraOriginalPosition.x, cameraOriginalPosition.y, transform.position.z - cameraDistance);
    }

    public void RotateObjects()
    {
        foreach (GameObject go in listOfObjectsToRotate)
        {
            go.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HITT: " + other.gameObject.name);
    }
}
