using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonClickSound : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioSource audioSource;

    private void Reset()
    {
        audioSource = FindFirstObjectByType<AudioSource>();
    }

    private void Awake()
    {
        if (audioSource == null)
            audioSource = FindFirstObjectByType<AudioSource>();

        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound);
    }

}
