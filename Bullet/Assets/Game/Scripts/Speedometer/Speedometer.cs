using UnityEngine;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI SpeedText;
    [SerializeField] private ParticleSystem speedLinesUnder100km;
    [SerializeField] private ParticleSystem speedLines100km;
    [SerializeField] private ParticleSystem speedLines200km;
    [SerializeField] private GameObject finalScoreMenu;

    private float currentSpeed;
    private RectTransform rectTransform;
    private bool speedLines100Playing = false;
    private bool speedLines200Playing = false;
    private bool speedLinesUnder100Playing = false;

    private bool hasLost = false;

    private void Awake()
    {
        rectTransform = SpeedText.GetComponent<RectTransform>();
        if (finalScoreMenu != null)
        {
            finalScoreMenu.SetActive(false);
        }
    }
    private void OnEnable()
    {
        BulletControl.OnSpeedChanged += UpdateSpeed;
        GameManager.OnLost += ShowFinalScore;
    }

    private void OnDisable()
    {
        BulletControl.OnSpeedChanged -= UpdateSpeed;
        GameManager.OnLost -= ShowFinalScore;
    }
    private void ShowFinalScore()
    {
        StopSpeedLinesUnder100();
        StopSpeedLines100();
        StopSpeedLines200();

        finalScoreMenu.SetActive(true);
        hasLost = true;

    }

    private void UpdateSpeed(float newSpeed)
    {
        currentSpeed = newSpeed * 10f;
        SpeedText.text = currentSpeed.ToString("F1") + " km/h";
    }

    private void Update()
    {
        if (hasLost) return;
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
            PlaySpeedLinesUnder100();
            StopSpeedLines100();
            StopSpeedLines200();
        }
        else if (currentSpeed <= 200f)
        {
            // Slow smooth pulse
            float pulse = (Mathf.Sin(Time.time * 3f) + 1f) * 0.5f;
            scale = 1f + pulse * 0.04f;
            PlaySpeedLines100();
            StopSpeedLines200();
            StopSpeedLinesUnder100();
        }
        else
        {
            // Fast aggressive pulse
            float pulse = (Mathf.Sin(Time.time * 8f) + 1f) * 0.5f;
            scale = 1f + pulse * 0.04f;
            StopSpeedLines100();
            PlaySpeedLines200();
            StopSpeedLinesUnder100();
        }

        rectTransform.localScale = Vector3.one * scale;
    }
    private void PlaySpeedLinesUnder100()
    {
        if (!speedLinesUnder100Playing)
        {
            speedLinesUnder100km.Play();
            speedLinesUnder100Playing = true;
        }
    }

    private void StopSpeedLinesUnder100()
    {
        if (speedLinesUnder100Playing)
        {
            speedLinesUnder100km.Stop();
            speedLinesUnder100Playing = false;
        }
    }
    private void PlaySpeedLines100()
    {
        if (!speedLines100Playing)
        {
            speedLines100km.Play();
            speedLines100Playing = true;
        }
    }

    private void StopSpeedLines100()
    {
        if (speedLines100Playing)
        {
            speedLines100km.Stop();
            speedLines100Playing = false;
        }
    }

    private void PlaySpeedLines200()
    {
        if (!speedLines200Playing)
        {
            speedLines200km.Play();
            speedLines200Playing = true;
        }
    }

    private void StopSpeedLines200()
    {
        if (speedLines200Playing)
        {
            speedLines200km.Stop();
            speedLines200Playing = false;
        }
    }

}
