using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager SoundManagerInst { get; private set; }
    public AudioSource AudioSource => GetComponent<AudioSource>();
    [SerializeReference]
    public AudioClipMod currentMusic;
    public List<AudioClip> SFXList;
    public List<AudioClipMod> musicList;
    public Dictionary<string, AudioClipMod> musicDictionary;
    public bool goToNextClip = false;
    public bool playRandomOnList = true;
    public float currentFade = 5f;

    private void Start()
    {
        if (SoundManagerInst == null)
            SoundManagerInst = this;
        else
            Destroy(gameObject);

        musicDictionary = new Dictionary<string, AudioClipMod>();
        foreach (AudioClip clip in musicList)
        {
            musicDictionary.Add(clip.name, clip);
        }
    }
    private void Update()
    {
        if (!Application.isFocused)
            return;
        if (playRandomOnList)
        {
            if (!goToNextClip)
                if (AudioSource.isPlaying)
                    return;
                newMusic:
            var nextClip = musicList[Random.Range(0, musicList.Count)];
            if (nextClip == AudioSource.clip)
                goto newMusic;
            PlayMusic(nextClip);
        }
        goToNextClip = false;
    }
    public void StopMusic()
    {
        AudioSource.Stop();
    }
    /*private void OnValidate()
    {
        musicDictionary = new Dictionary<string, AudioClipMod>();
        foreach (AudioClip clip in musicList)
        {
            musicDictionary.Add(clip.name, clip);
        }
    }*/
    public void PlayMusic(string name, float fade = 0f)
    {
        AudioSource.volume = musicDictionary[name].volume;
        currentMusic = musicDictionary[name].clip;
        AudioSource.Play();
        StopCoroutine("FadeCoroutine");
        if (fade > 0f)
        {
            StartCoroutine(FadeCoroutine(fade));
        }
        else
        {
            StartCoroutine(FadeCoroutine(currentFade));
        }
    }
    public void PlayMusic(AudioClipMod clip, float fade = 0f)
    {
        AudioSource.volume = clip.volume;
        currentMusic = clip;
        StopCoroutine("FadeCoroutine");
        if (fade > 0f)
        {
            StartCoroutine(FadeCoroutine(fade));
        }
        else
        {
            StartCoroutine(FadeCoroutine(currentFade));
        }
    }
    private IEnumerator FadeCoroutine(float time)
    {
        time *= 100;

        for (float a = 1f; a > 0f; a -= 1f / time)
        {
            AudioSource.volume = (currentMusic.volume * a);
            yield return new WaitForSeconds(0.01f);
        }
        AudioSource.volume = 0f;
        AudioSource.clip = currentMusic;
        AudioSource.Play();
        for (float a = 1f; a > 0f; a -= 1f / time)
        {
            AudioSource.volume = (currentMusic.volume * (1f - a));
            yield return new WaitForSeconds(0.01f);
        }
        AudioSource.volume = currentMusic.volume;
    }
    public void PlaySound(string name, Vector3? position = null)
    {
        if (position == null)
            position = gameObject.transform.position;
        AudioClip clip = null;
        foreach (var audio in SFXList)
        {
            if (audio.name == name)
            {
                clip = audio;
                break;
            }
        }
        var obj = new GameObject(name, typeof(AudioSource));
        obj.GetComponent<AudioSource>().clip = clip;
        obj.GetComponent<AudioSource>().Play();
        obj.transform.position = position.Value;
        StartCoroutine(timer());
        IEnumerator timer()
        {
            yield return new WaitForSeconds(clip.length + 0.1f);
            Destroy(obj);
        }
    }
    /*private void PlaySoundSimple(string name)
    {
        AudioClip clip = null;
        foreach (var audio in SFXList)
        {
            if (audio.name == name)
            {
                clip = audio;
                break;
            }
        }
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }*/
}
