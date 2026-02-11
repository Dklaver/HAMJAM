using NUnit.Framework.Internal;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int targetLayer;

    private void Awake()
    {
        targetLayer = LayerMask.NameToLayer("Target");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != targetLayer)
            return;

        HeatmapHitDetector detector = collision.collider.GetComponent<HeatmapHitDetector>();
        ParticleImpact particleSpawner = collision.collider.GetComponent<ParticleImpact>();

        if (detector != null && particleSpawner != null)
        {
            Vector3 hitPoint = collision.contacts[0].point;
            Vector3 hitNormal   = collision.contacts[0].normal;
            detector.EvaluateHit(hitPoint);
            particleSpawner.SpawnParticle(hitPoint, hitNormal);
        }

        Destroy(gameObject);
    }
}
