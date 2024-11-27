using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ObjectUtils;

public class FPS : MonoBehaviour
{
    public TextMeshProUGUI FpsText => gameObject.GetGameObjectComponent<TextMeshProUGUI>("FpsCounter");
    public float deltaTime;
    void Update()
    {
        FpsText.gameObject.SetActive(PlayerPreferences.ShowFPS);
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        if (PlayerPreferences.ShowFPS)
        {
            float fps = 1.0f / deltaTime;
            if (fps.ToString().Length > 5)
                FpsText.text = $"FPS: {fps.ToString().Remove(5)}";
        }
    }
}
