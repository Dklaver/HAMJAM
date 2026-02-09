using UnityEngine;

public class Heatmap : MonoBehaviour
{
    [SerializeField] private int critPoints = 2;
    [SerializeField] private Material material;
    [SerializeField] private float minIntensity = 1f;
    [SerializeField] private float maxIntensity = 1f;

    void Start()
    {
        SetCritPoints();
    }

    private void SetCritPoints()
    {
        float[] hits = new float[32 * 3];

        for (int i = 0; i < critPoints; i++)
        {
            float u = Random.value; // 0–1
            float v = Random.value; // 0–1
            float intensity = Random.Range(minIntensity, maxIntensity);

            hits[i * 3]     = u;
            hits[i * 3 + 1] = v;
            hits[i * 3 + 2] = intensity;
        }

        material.SetInt("_HitCount", critPoints);
        material.SetFloatArray("_Hits", hits);
    }
}
