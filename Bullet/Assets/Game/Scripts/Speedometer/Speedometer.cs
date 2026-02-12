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
        SpeedText.text = newSpeed.ToString("F1");
    }

    private void UpdateColor()
    {

    }

    private void UpdateVFX()
    {

    }
}
