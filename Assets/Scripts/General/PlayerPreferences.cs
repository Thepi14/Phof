using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPreferences
{
    #region General
    public static string CurrentDifficulty => PlayerPrefs.GetString("DIFFICULTY", "Normal");
    public static float MasterVolume => PlayerPrefs.GetFloat("MASTER_VOLUME", 0.8f);
    public static float MusicVolume => PlayerPrefs.GetFloat("MUSIC_VOLUME", 1f);
    public static float SoundEffectsVolume => PlayerPrefs.GetFloat("SOUND_EFFECTS_VOLUME", 1f);
    public static float UIVolume => PlayerPrefs.GetFloat("UI_VOLUME", 1f);
    #endregion

    #region Scaled volume
    public static float MusicVolumeScaled => PlayerPrefs.GetFloat("MUSIC_VOLUME", 1f) * PlayerPrefs.GetFloat("MASTER_VOLUME", 0.8f);
    public static float SoundEffectsVolumeScaled => PlayerPrefs.GetFloat("SOUND_EFFECTS_VOLUME", 1f) * PlayerPrefs.GetFloat("MASTER_VOLUME", 0.8f);
    public static float UIVolumeScaled => PlayerPrefs.GetFloat("UI_VOLUME", 1f) * PlayerPrefs.GetFloat("MASTER_VOLUME", 0.8f);
    #endregion

    public static Difficulty GetDifficulty() =>
        CurrentDifficulty == "Easy" ? Difficulty.easy :
        CurrentDifficulty == "Normal" ? Difficulty.normal :
        CurrentDifficulty == "Hard" ? Difficulty.hard : Difficulty.lunatic;
}
public enum Difficulty : byte
{
    easy = 1,
    normal = 2,
    hard = 3,
    lunatic = 4
}