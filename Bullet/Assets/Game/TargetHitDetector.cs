using UnityEngine;

public class TargetColorDetector : MonoBehaviour
{
    public Texture2D targetTexture;
    public MeshCollider targetCollider;

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.thisCollider == targetCollider)
            {
                Ray ray = new Ray(contact.point - contact.normal * 0.01f, contact.normal);
                if (targetCollider.Raycast(ray, out RaycastHit hit, 1f))
                {
                    Vector2 uv = hit.textureCoord;
                    Color color = targetTexture.GetPixelBilinear(uv.x, uv.y);
                    string region = GetColorRegionByHSV(color);

                    Debug.Log("Hit Color: " + color);
                    Debug.Log("Hit Region: " + region);
                }
            }
        }
    }

    string GetColorRegionByHSV(Color color)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);

        if (v < 0.2f) return "Black";  // very dark
        if (v > 0.8f && s < 0.2f) return "White"; // bright and low saturation

        if (s < 0.2f) return "Black"; // low saturation = gray

        if (h >= 0f && h < 0.1f) return "Red";
        if (h >= 0.1f && h < 0.25f) return "Yellow";
        if (h >= 0.25f && h < 0.4f) return "Green";
        if (h >= 0.4f && h <= 0.8f) return "Blue";

        return "Unknown";
    }
}
