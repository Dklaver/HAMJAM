using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private SlowdownMechanic slowdownMechanic;

    [SerializeField] private string text1 = "";
    [SerializeField] private string text2 = "";


    int i = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0f)
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Time.timeScale = 1f;
                slowdownMechanic.enabled = true;
                tutorialText.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Tutorial")
        {
            i++;
            slowdownMechanic.enabled = false;
            Time.timeScale = 0f;
            tutorialText.gameObject.SetActive(true);
            tutorialText.text = text1 + "\n\n Press 'Right Mouse Button' to continue.";


            if (i > 0)
            {
                tutorialText.text = text2 + "\n\n Press 'Right Mouse Button' to continue.";
                i = 0;
            }
        }
    }
}
