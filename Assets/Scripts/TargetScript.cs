using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{
    [Header("Charge Settings")]
    public float requiredTime = 1f;
    public Color laserColor = Color.red;
    public float glowMultiplier = 1.5f;

    [Header("UI")]
    public GameObject levelCompleteText;
    public float delayAfterPop = 2f;

    // =======================
    // 🔊 AUDIO SOURCES
    // =======================

    [Header("Audio Sources")]
    public AudioSource chargeSource;
    public AudioSource breakSource;

    // =======================
    // 🔊 CHARGE SOUND SETTINGS
    // =======================

    [Header("Charge Sound Loop (Seconds)")]
    public float chargeLoopStart = 0f;
    public float chargeLoopEnd = 5f;
    public float chargeDelay = 0f;

    // =======================
    // 🔊 BREAK SOUND SETTINGS
    // =======================

    [Header("Break Sound Timing (Seconds)")]
    public float breakStartTime = 0f;
    public float breakEndTime = 1f;   // optional (for future looping)
    public float breakDelay = 0f;

    // =======================
    // INTERNAL STATE
    // =======================

    private bool isLit = false;
    private bool levelCompleted = false;

    private bool chargeLoopActive = false;
    private bool breakPlayed = false;

    private float charge = 0f;

    private SpriteRenderer sr;
    private Collider2D col;
    private Color baseColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        baseColor = sr.color;

        if (levelCompleteText != null)
            levelCompleteText.SetActive(false);
    }

    void Update()
    {
        if (isLit && !levelCompleted)
        {
            charge += Time.deltaTime / requiredTime;
            charge = Mathf.Clamp01(charge);

            sr.color = Color.Lerp(baseColor, laserColor * glowMultiplier, charge);

            StartChargeLoop();

            if (charge >= 1f)
            {
                levelCompleted = true;
                StopChargeLoop();
                StartCoroutine(PopThenShowUI());
            }
        }
        else if (!levelCompleted)
        {
            charge -= Time.deltaTime;
            charge = Mathf.Clamp01(charge);

            sr.color = Color.Lerp(baseColor, laserColor * glowMultiplier, charge);
            StopChargeLoop();
        }

        // Manual charge loop
        if (chargeLoopActive && chargeSource != null && chargeSource.isPlaying)
        {
            if (chargeSource.time >= chargeLoopEnd)
                chargeSource.time = chargeLoopStart;
        }

        if (!levelCompleted)
            isLit = false;
    }

    // =======================
    // 🔥 LASER INTERFACE
    // =======================

    public void SetLit()
    {
        if (!levelCompleted)
            isLit = true;
    }

    public bool IsPopped()
    {
        return levelCompleted;
    }

    // =======================
    // 🔊 CHARGE SOUND CONTROL
    // =======================

    public void StartChargeLoop()
    {
        if (chargeLoopActive || chargeSource == null)
            return;

        chargeLoopActive = true;
        StartCoroutine(StartChargeAfterDelay());
    }

    public void StopChargeLoop()
    {
        if (!chargeLoopActive || chargeSource == null)
            return;

        chargeSource.Stop();
        chargeLoopActive = false;
    }

    IEnumerator StartChargeAfterDelay()
    {
        if (chargeDelay > 0)
            yield return new WaitForSeconds(chargeDelay);

        chargeSource.time = chargeLoopStart;
        chargeSource.Play();
    }

    // =======================
    // 💥 BREAK SOUND CONTROL
    // =======================

    public void PlayBreakSound()
    {
        if (breakPlayed || breakSource == null)
            return;

        breakPlayed = true;
        StartCoroutine(PlayBreakAfterDelay());
    }

    IEnumerator PlayBreakAfterDelay()
    {
        if (breakDelay > 0)
            yield return new WaitForSeconds(breakDelay);

        breakSource.time = breakStartTime;
        breakSource.Play();

        // Optional stop point (future-proofing)
        if (breakEndTime > breakStartTime)
            StartCoroutine(StopBreakAtEnd());
    }

    IEnumerator StopBreakAtEnd()
    {
        float duration = breakEndTime - breakStartTime;
        yield return new WaitForSeconds(duration);

        if (breakSource.isPlaying)
            breakSource.Stop();
    }

    // =======================
    // 💥 VISUALS
    // =======================

    IEnumerator PopThenShowUI()
    {
        yield return StartCoroutine(PopCrystal());
        yield return new WaitForSeconds(delayAfterPop);

        if (levelCompleteText != null)
            levelCompleteText.SetActive(true);
    }

    IEnumerator PopCrystal()
    {
        PlayBreakSound();
        LaserRaycast laser = FindFirstObjectByType<LaserRaycast>();
        if (laser != null) { laser.StopLaserHum(); }

        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 1.3f;

        float t = 0f;
        float duration = 0.25f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float n = t / duration;

            transform.localScale = Vector3.Lerp(startScale, endScale, n);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1 - n);

            yield return null;
        }

        sr.enabled = false;
        if (col != null)
            col.enabled = false;
    }
}
