using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New soundMod", menuName = "New sound mod")]
[Serializable]
public class AudioClipMod : ScriptableObject
{
    public AudioClip clip;
    public float volume = 1f;
    public AudioClipMod(AudioClip clip) => this.clip = clip;
    public static implicit operator AudioClipMod(AudioClip clip) => new AudioClipMod(clip);
    public static implicit operator AudioClip(AudioClipMod clip) => clip.clip;
}
