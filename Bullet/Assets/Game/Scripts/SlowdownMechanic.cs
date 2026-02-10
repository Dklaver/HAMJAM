using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SlowdownMechanic : MonoBehaviour
{
    [SerializeField] private float slowValue = 0.2f;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float staminaDrain = 3f;
    [SerializeField] private float staminaRecharge = 1f;

    [SerializeField] private Image staminaUI;
    [SerializeField] private Volume slowdownVolume;
    [SerializeField] private float volumeLerpSpeed = 5f; // how fast to lerp weight

    private bool isSlowingDown = false;
    private float targetWeight = 0f;

    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
            Slowdown();

        if (Keyboard.current.qKey.wasReleasedThisFrame)
            Speedup();

        StaminaUpdate();
        LerpVolumeWeight();
    }

    void Slowdown()
    {
        if (stamina > 0)
        {
            Time.timeScale = slowValue;
            isSlowingDown = true;
            targetWeight = 1f;
        }
        else
            Speedup();
    }

    void Speedup()
    {
        if (!isSlowingDown) return;

        Time.timeScale = 1f;
        isSlowingDown = false;
        targetWeight = 0f;
    }

    void LerpVolumeWeight()
    {
        if (slowdownVolume != null)
        {
            slowdownVolume.weight = Mathf.Lerp(slowdownVolume.weight, targetWeight, Time.unscaledDeltaTime * volumeLerpSpeed);
        }
    }

    void StaminaUpdate()
    {
        if (isSlowingDown)
        {
            stamina -= staminaDrain * Time.deltaTime * (1f / slowValue);

            if (stamina <= 0)
                Speedup();
        }
        else
        {
            stamina += staminaRecharge * Time.deltaTime;
        }

        stamina = Mathf.Clamp(stamina, 0, 100);
        staminaUI.fillAmount = stamina / 100f;
    }
}
