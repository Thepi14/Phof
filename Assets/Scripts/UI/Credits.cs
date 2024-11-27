using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public bool telaRin = false;
    public AudioSource audioSource;
    public Button rinButton;
    void Start()
    {
        audioSource.volume = PlayerPreferences.MusicVolumeScaled;
        rinButton.onClick.AddListener(() => { telaRin = true; });
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R) && telaRin)
        {
            Debug.Log("Tezuka Rin");
        }
    }
}
