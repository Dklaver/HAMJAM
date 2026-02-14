using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class CollisionLogic : MonoBehaviour
{
    [SerializeField] private float slowDownAmount = 10f;

    [SerializeField] private BulletControl bullet;
    [SerializeField] private SlowdownMechanic slowdownMechanic;

    private Hole latestRewardHitObject;

    [Header("Hit Volume")]
    [SerializeField] private Volume hitVolume;
    [SerializeField] private float volumeFadeDuration = 1f;

    private Coroutine volumeFadeRoutine;

    void SpeedUp()
    {
        bullet.forwardSpeed += latestRewardHitObject.rewardValueSpeed;
        slowdownMechanic.stamina += latestRewardHitObject.rewardValueEnergy;
    }

    void SlowDown()
    {
        bullet.forwardSpeed -= slowDownAmount;

        TriggerSlowdownVolume();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hole"))
        {
            latestRewardHitObject = other.GetComponent<Hole>();
            SpeedUp();
            Debug.Log("SPEED UP");
        }

        if (other.CompareTag("Obstacle"))
        {
            SlowDown();
            Debug.Log("Hit an obstacle");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            SlowDown();
            Debug.Log("Hit an obstacle");
        }
    }

    // ----------------------------
    // ONLY FOR SLOWDOWN EFFECT
    // ----------------------------
    void TriggerSlowdownVolume()
    {
        if (hitVolume == null)
            return;

        // instantly enable full effect
        hitVolume.weight = 1f;

        // restart fade if already running
        if (volumeFadeRoutine != null)
            StopCoroutine(volumeFadeRoutine);

        volumeFadeRoutine = StartCoroutine(FadeVolume());
    }

    IEnumerator FadeVolume()
    {
        float timer = 0f;

        while (timer < volumeFadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            hitVolume.weight =
                Mathf.Lerp(1f, 0f, timer / volumeFadeDuration);

            yield return null;
        }

        hitVolume.weight = 0f;
    }
}
