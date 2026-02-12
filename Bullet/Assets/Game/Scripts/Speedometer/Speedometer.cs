using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI SpeedText;

    private float currentSpeed;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = SpeedText.GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        BulletControl.OnSpeedChanged += UpdateSpeed;
    }

    private void OnDisable()
    {
        BulletControl.OnSpeedChanged -= UpdateSpeed;
    }

    private void UpdateSpeed(float newSpeed)
    {
        currentSpeed = newSpeed * 2.2369f;
        SpeedText.text = currentSpeed.ToString("F1") + " km/h";
    }

    private void Update()
    {
        UpdateColor();
        UpdateVFX();
    }

    private void UpdateColor()
    {
        if (currentSpeed <= 100f)
            SpeedText.color = Color.white;
        else if (currentSpeed <= 200f)
            SpeedText.color = Color.yellow;
        else
            SpeedText.color = Color.red;
    }

    private void UpdateVFX()
    {
        float scale = 1f;

        if (currentSpeed <= 100f)
        {
            scale = 1f;
        }
        else if (currentSpeed <= 200f)
        {
            // Slow smooth pulse
            float pulse = (Mathf.Sin(Time.time * 3f) + 1f) * 0.5f;
            scale = 1f + pulse * 0.04f;
        }
        else
        {
            // Fast aggressive pulse
            float pulse = (Mathf.Sin(Time.time * 8f) + 1f) * 0.5f;
            scale = 1f + pulse * 0.04f;
        }

        rectTransform.localScale = Vector3.one * scale;
    }
}
