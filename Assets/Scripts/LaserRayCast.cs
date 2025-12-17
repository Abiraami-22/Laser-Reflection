using UnityEngine;
using UnityEngine.InputSystem;

public class LaserRaycast : MonoBehaviour
{
    void Update()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.z = 0f;

        Vector2 direction = (mousePos - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);

        if (hit.collider != null)
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, direction * 20f, Color.red);
        }
    }
}

