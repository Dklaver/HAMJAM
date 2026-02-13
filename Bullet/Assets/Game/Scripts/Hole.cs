using UnityEngine;

public class Hole : MonoBehaviour
{
    // Duration of scaling animation
    public float rewardValue = 15;
    public float scaleDuration = 0.07f;
    [SerializeField] SlowdownMechanic slowdownMechanic;

    private bool isScaling = false;
    private float scaleTimer = 0f;
    private Vector3 initialScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (isScaling)
        {
            if (slowdownMechanic.isSlowingDown)
                scaleTimer += Time.deltaTime * (1f / slowdownMechanic.slowValue);

            else
                scaleTimer += Time.deltaTime;

            float t = scaleTimer / scaleDuration;

            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);

            if (scaleTimer >= scaleDuration)
            {
                transform.localScale = Vector3.zero;
                isScaling = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isScaling)
            {
                isScaling = true;
                scaleTimer = 0f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!isScaling)
            {
                isScaling = true;
                scaleTimer = 0f;
            }
        }
    }
}
