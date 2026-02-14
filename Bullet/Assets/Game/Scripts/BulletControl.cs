using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class BulletControl : MonoBehaviour
{
    public Transform bulletReference;
    public Transform bulletBox;

    public bool isMovingForward = true;

    public float moveSpeed = 10f;
    [SerializeField] public float forwardSpeed = 1f;
    public float ForwardSpeed
    {
        get => forwardSpeed;
        set
        {
            if (Mathf.Approximately(forwardSpeed, value))
                return;

            forwardSpeed = value;
            OnSpeedChanged?.Invoke(forwardSpeed);
        }
    }
    public float rotationSpeed = 90f; // degrees per second
    public List<GameObject> listOfObjectsToRotate = new List<GameObject>();

    public float cameraXOffset = 0f;
    public float cameraYOffset = 0f;
    public float cameraZOffset = 0f;

    public bool freeCameraXMovement = false;
    public bool freeCameraYMovement = false;

    public float planeZOffset = 0f;

    [UnityEngine.Range(0f, 1f)]
    public float xMin = 0.1f;

    [UnityEngine.Range(0f, 1f)]
    public float xMax = 0.9f;

    [UnityEngine.Range(0f, 1f)]
    public float yMin = 0.1f;

    [UnityEngine.Range(0f, 1f)]
    public float yMax = 0.9f;

    private Camera mainCamera;
    private Vector3 currentTargetWorld;
    private Vector3 cameraOriginalPosition;

    private Vector3 boxOriginalPosition;
    public static event Action<float> OnSpeedChanged;

    public bool blockPositiveX;
    public bool blockNegativeX;
    public bool blockPositiveY;
    public bool blockNegativeY;

    [SerializeField] public Rigidbody rb;
    [SerializeField] private float loseSpeedKMH = 50f;

    public bool hasLost = false;

    private void OnEnable()
    {
        SoundManager.Instance.PlayLoop(Sound.Wind);
    }

    private void OnDisable()
    {
        SoundManager.Instance.StopLoop(Sound.Wind);
    }

    void Start()
    {
        mainCamera = Camera.main;
        currentTargetWorld = transform.position;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        cameraOriginalPosition = mainCamera.transform.position;
        boxOriginalPosition = bulletBox.transform.position;

        StartCoroutine(RemovePropellerAfterTime());
    }

    private void CheckLoseCondition()
    {
        float speedKMH = ForwardSpeed * 10f;

        if (hasLost)
            return;

        if (speedKMH < loseSpeedKMH)
        {
            GameManager.Instance.EndGame();
        }
    }


    private IEnumerator RemovePropellerAfterTime()
    {
        yield return new WaitForSeconds(2f);
        RemovePropeller();
    }

    private void RemovePropeller()
    {
        FallingPropeller prop = GetComponentInChildren<FallingPropeller>();
        if (prop != null)
        {
            GameObject propObject = prop.gameObject;

            if (listOfObjectsToRotate.Contains(propObject))
            {
                listOfObjectsToRotate.Remove(propObject);
            }

            Vector3 forwardVelocity = transform.forward * ForwardSpeed;
            prop.Detach(forwardVelocity);

        }
    }

    void Update()
    {
        if (hasLost)
            return;

        if (Mouse.current == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow);

        // Convert screen to viewport space (0 to 1)
        Vector3 viewportPos = mainCamera.ScreenToViewportPoint(mousePosition);

        // Clamp viewport independently
        float clampedX = Mathf.Clamp(viewportPos.x, xMin, xMax);
        float clampedY = Mathf.Clamp(viewportPos.y, yMin, yMax);

        // Convert back to world at fixed distance
        float distanceFromCamera = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);

        Vector3 worldTarget = mainCamera.ViewportToWorldPoint(
            new Vector3(clampedX, clampedY, distanceFromCamera)
        );

        currentTargetWorld = new Vector3(
            worldTarget.x,
            worldTarget.y,
            transform.position.z
        );

        Vector3 target = Vector3.Lerp(
            transform.position,
            currentTargetWorld,
            Time.deltaTime * moveSpeed
        );

        Vector3 delta = target - transform.position;

        // Block based on direction
        if (delta.x > 0 && blockPositiveX)
            delta.x = 0;

        if (delta.x < 0 && blockNegativeX)
            delta.x = 0;

        if (delta.y > 0 && blockPositiveY)
            delta.y = 0;

        if (delta.y < 0 && blockNegativeY)
            delta.y = 0;

        transform.position += delta;


        if (isMovingForward)
            ForwardSpeed -= Time.deltaTime;

        MoveForward(ForwardSpeed);
        CheckLoseCondition();

        //CheckLock();
    }

    private void LateUpdate()
    {
        RotateObjects();
    }

    public void MoveForward(float speed)
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        bulletBox.position = new Vector3(boxOriginalPosition.x, boxOriginalPosition.y, transform.position.z);

        if (freeCameraXMovement && freeCameraYMovement)
        {
            mainCamera.gameObject.transform.position = new(transform.position.x - cameraXOffset, transform.position.y - cameraYOffset, transform.position.z - cameraZOffset);
        }

        else if (freeCameraXMovement)
        {
            mainCamera.gameObject.transform.position = new(transform.position.x - cameraXOffset, cameraOriginalPosition.y - cameraYOffset, transform.position.z - cameraZOffset);
        }
        else if (freeCameraYMovement)
        {
            mainCamera.gameObject.transform.position = new(cameraOriginalPosition.x - cameraXOffset, transform.position.y - cameraYOffset, transform.position.z - cameraZOffset);
        }
        else
            mainCamera.gameObject.transform.position = new(cameraOriginalPosition.x - cameraXOffset, cameraOriginalPosition.y - cameraYOffset, transform.position.z - cameraZOffset);
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
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = contact.normal;

            // If normal points left, we hit right wall
            if (normal.x > 0.5f)
                blockNegativeX = true;

            if (normal.x < -0.5f)
                blockPositiveX = true;

            if (normal.y > 0.5f)
                blockNegativeY = true;

            if (normal.y < -0.5f)
                blockPositiveY = true;
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        blockPositiveX = false;
        blockNegativeX = false;
        blockPositiveY = false;
        blockNegativeY = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HITT: " + other.gameObject.name);
    }

}
