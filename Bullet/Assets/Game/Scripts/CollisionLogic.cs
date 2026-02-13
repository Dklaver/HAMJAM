using UnityEngine;

public class CollisionLogic : MonoBehaviour
{
    [SerializeField]
    private float slowDownAmount = 10f;

    [SerializeField]
    private BulletControl bullet;

    [SerializeField]
    private SlowdownMechanic slowdownMechanic;

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
        bullet.forwardSpeed += latestRewardHitObject.rewardValueSpeed;
        slowdownMechanic.stamina += latestRewardHitObject.rewardValueEnergy;
    }

    void SlowDown()
    {
        bullet.forwardSpeed -= slowDownAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hole"))
        {
            latestRewardHitObject = other.gameObject.GetComponent<Hole>();
            SpeedUp();
            Debug.Log("SPEED UP");
        }

        if (other.gameObject.CompareTag("Obstacle"))
        {
            SlowDown();
            Debug.Log("Hit an obstacle");
        }
    }
}
