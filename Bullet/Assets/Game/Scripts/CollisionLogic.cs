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

        TriggerHitVolume();
    }

    void SlowDown()
    {
        bullet.forwardSpeed -= slowDownAmount;

        TriggerHitVolume();
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

    // ----------------------------
    // VOLUME CONTROL
    // ----------------------------
    void TriggerHitVolume()
    {
        if (hitVolume == null)
            return;

        // instantly go to full effect
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
