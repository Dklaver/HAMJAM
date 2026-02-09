using UnityEngine;
using UnityEngine.InputSystem;

public class PistolAnimation : MonoBehaviour
{
    public GameObject pistolSlid; // Assign the GameObject that has the Animator
    public GameObject pistolClip; // Assign the GameObject that has the Animator

    private Animator pistolSlidAnimator;
    private Animator pistolClipAnimator;

    void Start()
    {
        if (pistolSlid != null)
        {
            pistolSlidAnimator = pistolSlid.GetComponent<Animator>();
            pistolClipAnimator = pistolClip.GetComponent<Animator>();
        }

    }

    void Update()
    {
        PistolSlid();
        PistolBullets();
    }


    void PistolSlid()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            pistolSlidAnimator.SetTrigger("Empty");
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            pistolSlidAnimator.SetTrigger("Reload");
        }
    }

    void PistolBullets()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            pistolClipAnimator.SetTrigger("Empty");
        }

        if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            pistolClipAnimator.SetTrigger("Reload");
        }
    }
}