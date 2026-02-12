using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI SpeedText;
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
        float speed = newSpeed * 2.2369f;
        SpeedText.text = speed.ToString("F1") + " km/h";
    }

    private void UpdateColor()
    {

    }

    private void UpdateVFX()
    {

    }
}
