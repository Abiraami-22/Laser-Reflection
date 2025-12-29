using UnityEngine;
using UnityEngine.InputSystem;

public class MirrorRotate : MonoBehaviour
{
    public float scrollRotationSpeed = 200f;
    private bool isMouseOver = false;

    void Update()
    {
        DetectHover();

        if (!isMouseOver) return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0f)
        {
            transform.Rotate(0f, 0f, -scroll * scrollRotationSpeed * Time.deltaTime);
        }
    }

    void DetectHover()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        isMouseOver = hit.collider != null && hit.collider.gameObject == gameObject;
    }
}
