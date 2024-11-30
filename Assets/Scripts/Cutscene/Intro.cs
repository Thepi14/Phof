// --------------------------------------------------------------------------------------------------------------------
/// <copyright file="Intro.cs">
///   Copyright (c) 2024, Pi14, All rights reserved.
/// </copyright>
// --------------------------------------------------------------------------------------------------------------------
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
