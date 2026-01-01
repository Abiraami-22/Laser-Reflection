using UnityEngine;

public class LaserRaycast : MonoBehaviour
{
    private LineRenderer line;

    [Header("Laser Settings")]
    public float maxDistance = 100f;
    public int maxBounces = 10;

    [Header("VFX Objects")]
    public GameObject startVFXObject;
    public GameObject endVFXObject;

    private ParticleSystem startVFX;
    private ParticleSystem[] endVFXs;

    private bool endVFXPlaying = false;
    private bool setupValid = true;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        if (line == null)
        {
            Debug.LogError("[LaserRaycast] LineRenderer missing!");
            setupValid = false;
            return;
        }

        // ---------- START VFX ----------
        if (startVFXObject != null)
        {
            startVFX = startVFXObject.GetComponentInChildren<ParticleSystem>();
            if (startVFX != null)
            {
                startVFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                startVFX.Play();
            }
            else
            {
                Debug.LogWarning("[LaserRaycast] StartVFXObject has no ParticleSystem.");
            }
        }

        // ---------- END VFX ----------
        if (endVFXObject == null)
        {
            Debug.LogError("[LaserRaycast] EndVFXObject not assigned!");
            setupValid = false;
            return;
        }

        endVFXs = endVFXObject.GetComponentsInChildren<ParticleSystem>();

        if (endVFXs.Length == 0)
        {
            Debug.LogError("[LaserRaycast] EndVFXObject has NO ParticleSystems!");
            setupValid = false;
            return;
        }

        // Stop everything at start
        foreach (var ps in endVFXs)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        endVFXObject.SetActive(false);
    }

    void Update()
    {
        if (!setupValid)
            return;

        ShootLaser();
    }

    void ShootLaser()
    {
        line.positionCount = 1;
        line.SetPosition(0, transform.position);

        Vector2 direction = transform.right;

        bool shouldPlayEndVFX =
            CastLaser(transform.position, direction, maxBounces);

        if (!shouldPlayEndVFX)
            StopEndVFX();
    }

    bool CastLaser(Vector2 startPos, Vector2 direction, int bouncesLeft)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, maxDistance);

        if (hit.collider != null)
        {
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, hit.point);

            // Move EndVFXObject to hit point
            endVFXObject.transform.position = hit.point;

            float angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg;
            endVFXObject.transform.rotation = Quaternion.Euler(0, 0, angle);

            // WALL
            if (hit.collider.CompareTag("Wall"))
            {
                PlayEndVFX();
                return true;
            }

            // MIRROR
            if (hit.collider.CompareTag("Mirror") && bouncesLeft > 0)
            {
                Vector2 reflectDir = Vector2.Reflect(direction, hit.normal);
                Vector2 newStartPos = hit.point + reflectDir * 0.01f;
                return CastLaser(newStartPos, reflectDir, bouncesLeft - 1);
            }

            // TARGET
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
}
