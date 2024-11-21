using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPreferences
{
    public static string CurrentDifficulty => PlayerPrefs.GetString("DIFFICULTY", "Normal");
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