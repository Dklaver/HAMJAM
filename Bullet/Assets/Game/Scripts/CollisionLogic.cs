using UnityEngine;

public class CollisionLogic : MonoBehaviour
{
    [SerializeField]
    private float slowDownAmount = 10f;

    [SerializeField]
    private BulletControl bullet;

    private Hole latestRewardHitObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpeedUp()
    {
        bullet.forwardSpeed += latestRewardHitObject.rewardValue;
    }

    void SlowDown()
    {
        bullet.forwardSpeed -= slowDownAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hole"))
        {
            SpeedUp();
            latestRewardHitObject = other.gameObject.GetComponent<Hole>();
            Debug.Log("SPEED UP");
        }

        if (other.gameObject.CompareTag("Obstacle"))
        {
            SlowDown();
            Debug.Log("Hit an obstacle");
        }
    }
}
