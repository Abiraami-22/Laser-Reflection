using UnityEngine;
using System.Collections;

public class LaserRaycast : MonoBehaviour
{
    private LineRenderer line;

    [Header("Laser Settings")]
    public float maxDistance = 100f;
    public int maxBounces = 10;
    public float laserStartOffset = 0.05f;

    [Header("VFX Objects")]
    public GameObject startVFXObject;
    public GameObject endVFXObject;

    [Header("Laser SFX")]
    public AudioSource laserHumSource;
    public float humLoopStart = 0.5f;   // seconds
    public float humLoopEnd = 9.5f;     // seconds
    public float humStartDelay = 0f;

    private ParticleSystem startVFX;
    private ParticleSystem[] endVFXs;

    private bool endVFXPlaying = false;
    private bool setupValid = true;
    private bool humPlaying = false;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        if (line == null)
        {
            setupValid = false;
            return;
        }

        // START VFX
        if (startVFXObject != null)
        {
            startVFX = startVFXObject.GetComponentInChildren<ParticleSystem>();
            if (startVFX != null)
                startVFX.Play();
        }

        // END VFX
        if (endVFXObject == null)
        {
            setupValid = false;
            return;
        }

        endVFXs = endVFXObject.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in endVFXs)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        endVFXObject.SetActive(false);
    }

    void Update()
    {
        if (!setupValid)
            return;

        ShootLaser();
        HandleLaserHumLoop();
    }

    void ShootLaser()
    {
        Vector2 laserStart =
            (Vector2)transform.position + (Vector2)transform.right * laserStartOffset;

        line.positionCount = 1;
        line.SetPosition(0, laserStart);

        Vector2 direction = transform.right;

        bool hitSomething =
            CastLaser(laserStart, direction, maxBounces);

        if (!hitSomething)
            StopEndVFX();
    }

    bool CastLaser(Vector2 startPos, Vector2 direction, int bouncesLeft)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, maxDistance);

        if (hit.collider != null)
        {
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, hit.point);

            endVFXObject.transform.position = hit.point;

            float angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg;
            endVFXObject.transform.rotation = Quaternion.Euler(0, 0, angle);

            if (hit.collider.CompareTag("Wall"))
            {
                PlayEndVFX();
                return true;
            }

            if (hit.collider.CompareTag("Mirror") && bouncesLeft > 0)
            {
                Vector2 reflectDir = Vector2.Reflect(direction, hit.normal);
                Vector2 newStartPos = hit.point + reflectDir * 0.05f;
                return CastLaser(newStartPos, reflectDir, bouncesLeft - 1);
            }

            if (hit.collider.CompareTag("Target"))
            {
                Target target = hit.collider.GetComponent<Target>();
                if (target != null && !target.IsPopped())
                {
                    target.SetLit();
                    PlayEndVFX();
                    return true;
                }
            }
        }
        else
        {
            line.positionCount++;
            line.SetPosition(
                line.positionCount - 1,
                startPos + direction * maxDistance
            );
        }

        return false;
    }

    void PlayEndVFX()
    {
        if (endVFXPlaying)
            return;

        endVFXObject.SetActive(true);
        foreach (var ps in endVFXs)
            ps.Play();

        endVFXPlaying = true;
    }

    void StopEndVFX()
    {
        if (!endVFXPlaying)
            return;

        foreach (var ps in endVFXs)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        endVFXObject.SetActive(false);
        endVFXPlaying = false;
    }

    // 🔊 LASER HUM LOOP WITH CUSTOM LOOP POINTS
    void HandleLaserHumLoop()
    {
        if (laserHumSource == null)
            return;

        if (!humPlaying)
        {
            StartCoroutine(StartHumWithDelay());
            humPlaying = true;
        }

        if (laserHumSource.isPlaying &&
            laserHumSource.time >= humLoopEnd)
        {
            laserHumSource.time = humLoopStart;
        }
    }

    IEnumerator StartHumWithDelay()
    {
        if (humStartDelay > 0)
            yield return new WaitForSeconds(humStartDelay);

        laserHumSource.time = humLoopStart;
        laserHumSource.Play();
    }

    public void StopLaserHum()
    {
        if (laserHumSource != null)
            laserHumSource.Stop();

        humPlaying = false;
    }
}
