using UnityEngine;
using UnityEngine.InputSystem;

public class SlowdownMechanic : MonoBehaviour
{
    [SerializeField] private float slowDownValue = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
            Slowdown();

        if (Keyboard.current.qKey.wasReleasedThisFrame)
            Speedup();
    }

    void Slowdown()
    {
        Time.timeScale = slowDownValue;
    }

    void Speedup()
    {
        Time.timeScale = 1;
    }
}
