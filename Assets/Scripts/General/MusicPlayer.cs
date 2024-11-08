using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public AudioSource AudioSource => GetComponent<AudioSource>();
    public List<AudioClipMod> musicList;
    public Dictionary<string, AudioClipMod> musicDictionary;
    public bool goToNextClip = false;

    private void Start()
    {
        
    }
    private void Update()
    {
        if (!goToNextClip)
            if (AudioSource.isPlaying)
                return;
        newMusic:
        var nextClip = musicList[Random.Range(0, musicList.Count)];
        if (nextClip == AudioSource.clip)
            goto newMusic;
        AudioSource.clip = nextClip;
        AudioSource.volume = nextClip.volume;
        AudioSource.Play();
        goToNextClip = false;
    }
    private void OnValidate()
    {
        musicDictionary = new Dictionary<string, AudioClipMod>();
        foreach (AudioClip clip in musicList)
        {
            musicDictionary.Add(clip.name, clip);
        }
    }
}
