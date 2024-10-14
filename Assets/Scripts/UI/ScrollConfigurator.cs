using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[AddComponentMenu("UI/Scroll Configurator", 0)]
//[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class ScrollConfigurator : MonoBehaviour
{
    public Scrollbar scrollBar;
    public GameObject scrollView;
    public GameObject scrollContainer;

    public class ScrollUpdateEvent : UnityEvent { }
    [SerializeField]
    private ScrollUpdateEvent m_scrollUpdateEvent = new ScrollUpdateEvent();
    public ScrollUpdateEvent scrollUpdateEvent { get { return m_scrollUpdateEvent; } set { m_scrollUpdateEvent = value; } }

    private void Start()
    {
        scrollBar.onValueChanged.AddListener((float a) => { SetScrollPosition(); });
    }
    private float filepanelSize => scrollView.GetComponent<RectTransform>().sizeDelta.y;//691.74f;
    private float tabsHeight => scrollContainer.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
    public void SetScrollPosition()
    {
        int itemCount = scrollContainer.transform.childCount;
        float itemTotalSize = (float)tabsHeight * (itemCount + 1);
        if (itemTotalSize <= filepanelSize)
        {
            scrollBar.value = 0;
            scrollBar.size = 1;
        }
        else
        {
            float multipliyer2 = scrollContainer.transform.childCount - 11f;
            float multipliyer = filepanelSize / itemTotalSize;
            scrollBar.size = multipliyer;
            scrollContainer.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, 14f + ((float)tabsHeight * scrollBar.value * (multipliyer + multipliyer2)), 0);
        }
        scrollUpdateEvent?.Invoke();
    }
    public void ResetScrollPosition()
    {
        scrollBar.value = 0;
        scrollBar.size = 1;
    }
}
