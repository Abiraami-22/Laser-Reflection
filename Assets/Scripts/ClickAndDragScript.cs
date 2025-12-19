using UnityEngine;
using UnityEngine.EventSystems;

public class DraggingExample : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private Vector3 offset;
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0;

        offset = transform.position - mousePos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0;

        transform.position = mousePos + offset;
    }
}