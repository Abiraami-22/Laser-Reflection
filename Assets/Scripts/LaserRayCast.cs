using UnityEngine;

public class LaserRaycast : MonoBehaviour
{
    private LineRenderer line;
    public float maxDistance = 100f;
    public int maxBounces = 2;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        line.positionCount = 1;
        line.SetPosition(0, transform.position);

        Vector2 direction = Vector2.right;

        CastLaser(transform.position, direction, maxBounces);
    }

    void CastLaser(Vector2 startPos, Vector2 direction, int bouncesLeft)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, maxDistance);

        if (hit.collider != null)
        {
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, hit.point);

            if (hit.collider.CompareTag("Wall"))
                return;

            if (hit.collider.CompareTag("Mirror") && bouncesLeft > 0)
            {
                Vector2 reflectDir = Vector2.Reflect(direction, hit.normal);
                CastLaser(hit.point, reflectDir, bouncesLeft - 1);
            }

            // 🔥 CONNECT LASER → TARGET (FINAL & CORRECT WAY)
            if (hit.collider.CompareTag("Target"))
            {
                Target target = hit.collider.GetComponent<Target>();
                if (target != null)
                {
                    target.SetLit();   // ⭐ THIS IS THE IMPORTANT CHANGE
                }
            }
        }
        else
        {
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, startPos + direction * maxDistance);
        }
    }
}
