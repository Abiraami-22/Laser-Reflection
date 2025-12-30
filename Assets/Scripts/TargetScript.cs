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

    private bool isLit = false;
    private bool levelCompleted = false;

    private float charge = 0f;

    private SpriteRenderer sr;
    private Color baseColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;

        if (levelCompleteText != null)
            levelCompleteText.SetActive(false);
    }

    void Update()
    {
        if (isLit && !levelCompleted)
        {
            // Increase charge smoothly
            charge += Time.deltaTime / requiredTime;
            charge = Mathf.Clamp01(charge);

            // Smooth color transition (charging)
            Color chargedColor = Color.Lerp(
                baseColor,
                laserColor * glowMultiplier,
                charge
            );
            sr.color = chargedColor;

            if (charge >= 1f)
            {
                levelCompleted = true;

                if (levelCompleteText != null)
                    levelCompleteText.SetActive(true);

                StartCoroutine(PopCrystal());
            }
        }
        else if (!levelCompleted)
        {
            // Discharge smoothly if laser breaks (FIXED)
            charge -= Time.deltaTime;
            charge = Mathf.Clamp01(charge);

            sr.color = Color.Lerp(
                baseColor,
                laserColor * glowMultiplier,
                charge
            );
        }

        // Reset laser signal every frame (only if not completed)
        if (!levelCompleted)
            isLit = false;
    }

    // Called by LaserRaycast
    public void SetLit()
    {
        isLit = true;
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

        gameObject.SetActive(false);
    }
}
