using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    private AudioSource AudioSource => GetComponent<AudioSource>();
    private void Start()
    {
        AudioSource.volume = PlayerPreferences.MusicVolumeScaled;
    }
}
