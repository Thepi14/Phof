// --------------------------------------------------------------------------------------------------------------------
/// <copyright file="CutsceneManager.cs">
///   Copyright (c) 2024, Marcos Henrique, All rights reserved.
/// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using InputManagement;
public class CutsceneManager : MonoBehaviour
{
    public Button startGame;
    void Start()
    {
        startGame.onClick.AddListener( delegate { StartGame(); });
        if (InputManager.GetKeyDown(KeyBindKey.Escape))
            StartGame();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }
}
