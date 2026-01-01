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

    private bool isLit = false;
    private bool levelCompleted = false;

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

            sr.color = Color.Lerp(
                baseColor,
                laserColor * glowMultiplier,
                charge
            );

            if (charge >= 1f)
            {
                levelCompleted = true;
                StartCoroutine(PopThenShowUI());
            }
        }
        else if (!levelCompleted)
        {
            charge -= Time.deltaTime;
            charge = Mathf.Clamp01(charge);

            sr.color = Color.Lerp(
                baseColor,
                laserColor * glowMultiplier,
                charge
            );
        }

        // Reset laser signal every frame (unless completed)
        if (!levelCompleted)
            isLit = false;
    }

    // 🔥 Called by LaserRaycast
    public void SetLit()
    {
        if (!levelCompleted)
            isLit = true;
    }

    // ⭐ NEW: Used by LaserRaycast to stop EndVFX permanently
    public bool IsPopped()
    {
        return levelCompleted;
    }

    IEnumerator PopThenShowUI()
    {
        yield return StartCoroutine(PopCrystal());

        yield return new WaitForSeconds(delayAfterPop);

        if (levelCompleteText != null)
            levelCompleteText.SetActive(true);
    }

    IEnumerator PopCrystal()
    {
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

        // 🔥 Hide crystal visually but keep object alive
        sr.enabled = false;
        if (col != null)
            col.enabled = false;
    }
}
