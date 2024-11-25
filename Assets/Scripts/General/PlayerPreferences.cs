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

    #region Configurations
    public static bool ShowDamage => PlayerPrefs.GetInt("SHOW_DAMAGE", 1) == 1;
    public static bool Orthographic => PlayerPrefs.GetInt("ORTHOGRAPHIC", 0) == 1;
    public static bool ShowCoordinates => PlayerPrefs.GetInt("SHOW_COORD", 0) == 1;
    public static bool GameSaved => PlayerPrefs.GetInt("SAVED_GAME", 0) == 1;

    #endregion
    public static void Reset()
    {
        PlayerPrefs.SetFloat("MASTER_VOLUME", 0.8f);
        PlayerPrefs.SetFloat("MUSIC_VOLUME", 1f);
        PlayerPrefs.SetFloat("SOUND_EFFECTS_VOLUME", 1f);
        PlayerPrefs.SetFloat("UI_VOLUME", 1f);
        PlayerPrefs.SetInt("MAP_WIDTH", 100);
        PlayerPrefs.SetInt("MAP_HEIGHT", 100);
        PlayerPrefs.SetString("CLASS", "Warrior");
        PlayerPrefs.SetInt("SHOW_DAMAGE", 1);
        PlayerPrefs.SetInt("ORTHOGRAPHIC", 0);
        PlayerPrefs.SetInt("SHOW_COORD", 0);
        InputManagement.InputManager.ResetKeyBindToDefault();
    }
    public static Difficulty Difficulty =>
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