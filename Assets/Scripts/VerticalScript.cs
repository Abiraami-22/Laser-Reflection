using UnityEngine;
using UnityEngine.EventSystems;

public class MirrorVerticalDrag : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private Vector3 offset;
    private Camera mainCamera;
    private float fixedX;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0f;

        offset = transform.position - mousePos;
        fixedX = transform.position.x;   // 🔒 lock X
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0f;

        Vector3 newPos = mousePos + offset;
        newPos.x = fixedX;   // 🔒 keep X fixed

        transform.position = newPos;
    }
}
