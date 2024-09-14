using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ObjectUtils;

public class CanvasGameManager : MonoBehaviour
{
    public RectTransform LifeBar => GameObjectGeneral.GetGameObjectComponent<RectTransform>(gameObject, "Panel/LifeBar/Bar");
    float t;
    void Start()
    {
        
    }
    void Update()
    {
        t += Time.deltaTime;
        if (t > 1)
            t = 0;
        LifeBar.localScale = new Vector3(t, 1, 1);
        LifeBar.localPosition = new Vector3(0, 0, 0);
    }
}
