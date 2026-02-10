using UnityEngine;

public class Heatmap : MonoBehaviour
{
    [SerializeField] private int critPoints = 2;
    [SerializeField] private Material material;
    [SerializeField] private float minIntensity = 1f;
    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private Transform targetObject; // The object with the heatmap shader

    void Start()
    {
        if (targetObject == null)
            targetObject = transform;
        
        SetCritPoints();
    }

    private void SetCritPoints()
    {
        float[] hits = new float[32 * 4]; // Now storing x, y, z, intensity

        // Get the bounds of the target object
        Renderer renderer = targetObject.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("Target object needs a Renderer component!");
            return;
        }

        Bounds bounds = renderer.bounds;

        for (int i = 0; i < critPoints; i++)
        {
            // Generate random world position within the object's bounds
            Vector3 randomPos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            float intensity = Random.Range(minIntensity, maxIntensity);

            hits[i * 4]     = randomPos.x;
            hits[i * 4 + 1] = randomPos.y;
            hits[i * 4 + 2] = randomPos.z;
            hits[i * 4 + 3] = intensity;
        }

        material.SetInt("_HitCount", critPoints);
        material.SetFloatArray("_Hits", hits);
    }
}