using UnityEngine;
using UnityEngine.EventSystems;

public class MirrorHorizontalDrag : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private Vector3 offset;
    private Camera mainCamera;
    private float fixedY;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0f;

        offset = transform.position - mousePos;
        fixedY = transform.position.y;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0f;

        Vector3 newPos = mousePos + offset;
        newPos.y = fixedY;

        transform.position = newPos;
    }
}
