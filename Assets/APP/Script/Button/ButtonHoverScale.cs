using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverScale : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float speed = 10f;

    private Vector3 defaultScale;
    private Vector3 targetScale;

    void Start()
    {
        defaultScale = transform.localScale;
        targetScale = defaultScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * speed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = defaultScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = defaultScale;
    }
}
