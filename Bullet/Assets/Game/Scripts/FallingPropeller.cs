using UnityEngine;

public class FallingPropeller : MonoBehaviour
{
    [Header("Spin")]
    public float spinSpeed = 800f;
    public float spinSlowdownRate = 200f;

    [Header("Fall")]
    public float gravity = 9f;
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

    public void Detach()
    {
        isFalling = true;

        // Detach from bullet
        transform.parent = null;

        // Give slight initial push
        fallVelocity = new Vector3(
            Random.Range(-1f, 1f),
            2f,
            Random.Range(-1f, 1f)
        );

        Destroy(gameObject, 5f); // cleanup
    }
}
