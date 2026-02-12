using Unity.Mathematics;
using UnityEngine;

public class ParticleImpact : MonoBehaviour
{
    [SerializeField]private GameObject particleToSpawn;
    private Heatmap heatmap;

    private void Awake()
    {
        heatmap = GetComponent<Heatmap>();

        if (heatmap == null)
            Debug.LogError("HeatmapHitDetector requires Heatmap component.");
    }

    public void SpawnParticle(Vector3 hitPosition, Vector3 hitNormal)
    {
        if (heatmap.HeatPoints == null)
        {
            Debug.Log("no heatpoints foudns");
            return;
        }
        if (particleToSpawn == null)
        {
            return;
        }
        Quaternion rotation = Quaternion.LookRotation(hitNormal);
        Instantiate(particleToSpawn, hitPosition, rotation);
    }
}
