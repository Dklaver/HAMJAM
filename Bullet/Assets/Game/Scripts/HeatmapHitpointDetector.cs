using UnityEngine;

public enum HeatmapColors
{
    Red,
    Orange,
    Yellow,
    Green,
    Nothing
}
public class HeatmapHitDetector : MonoBehaviour
{
    private Heatmap heatmap;
    [SerializeField]
    private HeatmapColorValue[] colorValues;

    private void Awake()
    {
        heatmap = GetComponent<Heatmap>();

        if (heatmap == null)
            Debug.LogError("HeatmapHitDetector requires Heatmap component.");
    }

    public void EvaluateHit(Vector3 hitPosition)
    {
        if (heatmap.HeatPoints == null)
        {
            Debug.Log("no heatpoints foudns");
            return;
        }
            

        float totalWeight = 0f;
        float radius = heatmap.HeatRadius;

        foreach (var point in heatmap.HeatPoints)
        {
            Vector3 heatPoint = new Vector3(point.x, point.y, point.z);
            float intensity = point.w;

            float d = Vector3.Distance(hitPosition, heatPoint) / radius;
            d = Mathf.Clamp01(1f - d);

            totalWeight += d * d * intensity;
        }

        ScoreManager scoreManager = ScoreManager.Instance;
        HeatmapColors colorHit = GetColor(totalWeight);
        Debug.Log("Color hit: " + colorHit);
        int damage = GetValueForColor(colorHit);

        SoundManager.Instance.PlaySound(Sound.HitTarget);
        scoreManager.UpdateScore(damage);
        GameManager.Instance.EndGame();
    }

    private HeatmapColors GetColor(float heat)
    {
        if (heat < 0.05f) return HeatmapColors.Nothing;
        if (heat < 0.25f) return HeatmapColors.Green;
        if (heat < 0.50f) return HeatmapColors.Yellow;
        if (heat < 0.75f) return HeatmapColors.Orange;
        return HeatmapColors.Red;
    }

    private int GetValueForColor(HeatmapColors color)
{
    foreach (var entry in colorValues)
    {
        if (entry.color == color)
            return entry.value;
    }

    return 0;
}
}

[System.Serializable]
public struct HeatmapColorValue
{
    public HeatmapColors color;
    public int value;
}

