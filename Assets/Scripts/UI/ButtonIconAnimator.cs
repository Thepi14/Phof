using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonIconAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform Icon => transform.Find("Icon").GetComponent<RectTransform>();

    private const float DEFAULT_POS_OFFSET = 2;
    private const float DEFAULT_PRESSED_POS_OFFSET = 0;

    public void Start()
    {
        Icon.localPosition = new Vector3(0, DEFAULT_POS_OFFSET);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Icon.localPosition = new Vector3(0, DEFAULT_PRESSED_POS_OFFSET);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Icon.localPosition = new Vector3(0, DEFAULT_POS_OFFSET);
    }
}
