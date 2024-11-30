// --------------------------------------------------------------------------------------------------------------------
/// <copyright file="AudioClipMod.cs">
///   Copyright (c) 2024, Pi14, All rights reserved.
/// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New soundMod", menuName = "New sound mod")]
[Serializable]
public class AudioClipMod : ScriptableObject
{
    public AudioClip clip;
    public float volume = 1f;
    public AudioClipMod(AudioClip clip) => this.clip = clip;
    public static implicit operator AudioClipMod(AudioClip clip) => Resources.Load<AudioClipMod>($"Resources/Musics/{clip.name}");
    public static implicit operator AudioClip(AudioClipMod clip) => clip.clip;
    public void OnValidate()
    {
        if (clip == null) return;
        name = clip.name;
    }
}
