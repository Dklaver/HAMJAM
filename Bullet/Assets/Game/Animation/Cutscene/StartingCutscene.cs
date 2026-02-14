using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartingCutscene : MonoBehaviour
{
    [SerializeField]
    List<GameObject> thingToShow = new List<GameObject>();

    [SerializeField]
    TextMeshProUGUI timeToGo;

    [Header("Objects")]
    public GameObject pistolSlid;
    public GameObject fakeCamera;
    public GameObject fakeBullet;

    [Header("Effects")]
    public ParticleSystem particles;

    private Animator pistolSlidAnimator;
    private Animator fakeCameraAnimator;
    private Animator fakeBulletAnimator;

    void Start()
    {
        pistolSlidAnimator = pistolSlid.GetComponent<Animator>();
        fakeCameraAnimator = fakeCamera.GetComponent<Animator>();
        fakeBulletAnimator = fakeBullet.GetComponent<Animator>();
        timeToGo.text = "";

        foreach (GameObject go in thingToShow)
        {
            go.SetActive(false);
        }

        StartCoroutine(TimeLine());
    }

    // ----------------------------
    // CUTSCENE FLOW
    // ----------------------------
    IEnumerator TimeLine()
    {
        // Wait before firing
        yield return new WaitForSeconds(1f);
        Fire(); // EVERYTHING happens together here

        yield return new WaitForSeconds(0.8f);
        pistolSlidAnimator.SetTrigger("Empty");
        pistolSlidAnimator.SetTrigger("Reload");
        // Wait before camera move
        yield return new WaitForSeconds(1f);

        MoveFakeCamera();

        // Finish cutscene

        for (int i = 4; i > 0; i--)
        {
            if (i < 4)
                timeToGo.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        Time.timeScale = 0.1f;

        // Smoothly return to normal speed
        StartCoroutine(RestoreTimeScale());
        StartGame();
    }

    // ----------------------------
    // FIRE EVENT (SYNCED)
    // ----------------------------
    void Fire()
    {
        // Restart particles cleanly
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particles.Play();

        // Start animations in same frame

        fakeBulletAnimator.SetTrigger("Shoot");
    }

    // ----------------------------
    // CAMERA MOVE
    // ----------------------------
    void MoveFakeCamera()
    {
        fakeCameraAnimator.SetTrigger("Move");
    }

    // ----------------------------
    // END CUTSCENE
    // ----------------------------
    void StartGame()
    {
        foreach (GameObject go in thingToShow)
        {
            go.SetActive(true);
        }

        fakeCamera.SetActive(false);
        fakeBullet.SetActive(false);
        timeToGo.gameObject.SetActive(false);
    }


    IEnumerator RestoreTimeScale()
    {
        float duration = 1f;
        float timer = 0f;

        float startScale = 0.1f;
        float endScale = 1f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            Time.timeScale =
                Mathf.Lerp(startScale, endScale, timer / duration);

            Debug.Log(Time.timeScale);

            yield return null;
        }

        Time.timeScale = 1f;
    }
}
