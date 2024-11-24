using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public GameObject CameraObj;
    void Start()
    {
        CameraObj.GetComponent<AudioSource>().volume = PlayerPreferences.MusicVolumeScaled;
        CameraObj.GetComponent<AudioSource>().Play();
    }
    void Update()
    {
        
    }
}
