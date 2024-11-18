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
    public float currentVolume;
    public static float musicVolumeMultipliyer => PlayerPrefs.GetFloat("MASTER_VOLUME", 0.8f) * PlayerPrefs.GetFloat("MUSIC_VOLUME", 1f);

    private void Start()
    {
        if (SoundManagerInst == null)
            SoundManagerInst = this;
        else
            Destroy(gameObject);

        musicDictionary = new Dictionary<string, AudioClipMod>();
        foreach (AudioClipMod clip in musicList)
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
        if (currentMusic != null)
            AudioSource.volume = currentMusic.volume * currentVolume * musicVolumeMultipliyer;
        goToNextClip = false;
    }
    public static void StopMusic(float fade = 0f)
    {
        if (fade > 0f)
        {
            SoundManagerInst.StartCoroutine(SoundManagerInst.FadeOutCoroutine(fade));
        }
        else
        {
            SoundManagerInst.StartCoroutine(SoundManagerInst.FadeOutCoroutine(SoundManagerInst.currentFade));
        }
    }
    /*private void OnValidate()
    {
        musicDictionary = new Dictionary<string, AudioClipMod>();
        foreach (AudioClip clip in musicList)
        {
            musicDictionary.Add(clip.name, clip);
        }
    }*/
    public static void PlayMusic(string name, float fade = 0f)
    {
        SoundManagerInst.AudioSource.volume = SoundManagerInst.musicDictionary[name].volume * musicVolumeMultipliyer;
        SoundManagerInst.currentMusic = SoundManagerInst.musicDictionary[name];
        SoundManagerInst.StopCoroutine("FadeCoroutine");
        if (fade > 0f)
        {
            SoundManagerInst.StartCoroutine(SoundManagerInst.FadeInCoroutine(fade));
        }
        else
        {
            SoundManagerInst.StartCoroutine(SoundManagerInst.FadeInCoroutine(SoundManagerInst.currentFade));
        }
    }
    public static void PlayMusic(AudioClipMod clip, float fade = 0f)
    {
        SoundManagerInst.AudioSource.volume = clip.volume * musicVolumeMultipliyer;
        SoundManagerInst.currentMusic = clip;
        SoundManagerInst.StopCoroutine("FadeCoroutine");
        if (fade > 0f)
        {
            SoundManagerInst.StartCoroutine(SoundManagerInst.FadeInCoroutine(fade));
        }
        else
        {
            SoundManagerInst.StartCoroutine(SoundManagerInst.FadeInCoroutine(SoundManagerInst.currentFade));
        }
    }
    private IEnumerator FadeInCoroutine(float time)
    {
        time *= 100f;
        AudioSource.clip = currentMusic.clip;
        AudioSource.volume = 0f;
        AudioSource.Play();
        for (float a = 1f; a > 0f; a -= 1f / time)
        {
            currentVolume = 1f - a;
            yield return new WaitForSeconds(0.01f);
        }
        AudioSource.volume = currentMusic.volume * currentVolume * musicVolumeMultipliyer;
    }
    private IEnumerator FadeOutCoroutine(float time)
    {
        time *= 100f;
        AudioSource.volume = currentMusic.volume * currentVolume * musicVolumeMultipliyer;
        for (float a = 1f; a > 0f; a -= 1f / time)
        {
            currentVolume = a;
            yield return new WaitForSeconds(0.01f);
        }
        AudioSource.volume = 0f;
        AudioSource.Stop();
    }

    public static void PlaySound(string name, SoundType type, Vector3? position = null)
    {
        if (position == null)
            position = SoundManagerInst.gameObject.transform.position;

        var volumeMultipliyer = type == SoundType.Music ? PlayerPrefs.GetFloat("MUSIC_VOLUME", 1f) :
            type == SoundType.SoundEffect ? PlayerPrefs.GetFloat("SOUND_EFFECTS_VOLUME", 1f) :
            type == SoundType.UI ? PlayerPrefs.GetFloat("UI_VOLUME", 1f) : 1f;

        AudioClip clip = null;
        foreach (var audio in SoundManagerInst.SFXList)
        {
            if (audio.name == name)
            {
                clip = audio;
                break;
            }
        }

        var obj = new GameObject(name, typeof(AudioSource));
        obj.GetComponent<AudioSource>().clip = clip;
        obj.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MASTER_VOLUME", 0.8f) * volumeMultipliyer;
        obj.GetComponent<AudioSource>().Play();
        obj.transform.position = position.Value;
        SoundManagerInst.StartCoroutine(timer());

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
    public enum SoundType
    {
        Any = 0,
        Music = 1,
        SoundEffect = 2,
        UI = 3,
    }
}
