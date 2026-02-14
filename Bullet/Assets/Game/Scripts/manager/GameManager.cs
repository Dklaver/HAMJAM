using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] BulletControl bullet;
    [SerializeField] SlowdownMechanic slowdownMechanic;
    public static event Action OnLost;


    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndGame()
    {
        TriggerLose();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitApp()
    {
        Application.Quit();
    }

    private IEnumerator LoseConditionUI()
    {
        yield return new WaitForSeconds(1.5f);
        // Notify UI
        OnLost?.Invoke();
    }

    private void TriggerLose()
    {
        ScoreManager.Instance.finalScore = CalculateFinalScore();
        ScoreManager.Instance.UpdateFinalScore();

        slowdownMechanic.Speedup();
        slowdownMechanic.enabled = false;

        bullet.hasLost = true;

        // Stop movement logic
        bullet.isMovingForward = false;

        // Enable physics fall
        bullet.rb.isKinematic = false;
        bullet.rb.useGravity = true;

        // Optional: keep some forward momentum
        bullet.rb.linearVelocity = transform.forward * bullet.ForwardSpeed;

        // Stop rotating visuals
        bullet.listOfObjectsToRotate.Clear();

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(LoseConditionUI());

    }

    public float CalculateFinalScore()
    {
        float speed = bullet.forwardSpeed;
        int hit = ScoreManager.Instance.score;

        // Lose condition
        if (speed <= 5f)
            return 0f;

        if (hit == 0)
            return 0f;

        // Normalize inputs
        float speedValue = Mathf.InverseLerp(5f, 100f, speed);
        float hitValue = hit / 4f;

        // Weighted result
        float finalScore = (speedValue * 0.7f) + (hitValue * 0.3f);

        return finalScore * 10000;
    }
}
