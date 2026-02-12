using UnityEngine;

public class FallingPropeller : MonoBehaviour
{
    [Header("Spin")]
    public float spinSpeed = 800f;
    public float spinSlowdownRate = 200f;

    [Header("Fall")]
    public float gravity = 8f;
    public float tumbleSpeed = 200f;

    private bool isFalling = false;
    private Vector3 fallVelocity;

    //add the visual effects of the fire coming from behind it

    void Update()
    {
        if (!isFalling)
        {
            // Spin while attached
            transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);

            // Gradually slow spin
            spinSpeed = Mathf.Max(0f, spinSpeed - spinSlowdownRate * Time.deltaTime);
        }
        else
        {
            // Apply gravity
            fallVelocity.y -= gravity * Time.deltaTime;
            transform.position += fallVelocity * Time.deltaTime;

            // Tumble while falling
            transform.Rotate(Vector3.right, tumbleSpeed * Time.deltaTime);
            transform.Rotate(Vector3.forward, tumbleSpeed * 0.5f * Time.deltaTime);
        }
    }

    public void Detach(Vector3 bulletForwardVelocity)
    {
        isFalling = true;

        transform.parent = null;

        // Keep moving forward with the bullet
        fallVelocity = bulletForwardVelocity;

        // Add a small upward + sideways separation
        fallVelocity += new Vector3(
            Random.Range(-0.5f, 0.5f),
            1.5f,
            Random.Range(-0.5f, 0.5f)
        );

        Destroy(gameObject, 5f);
    }
}
