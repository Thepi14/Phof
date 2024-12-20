// --------------------------------------------------------------------------------------------------------------------
/// <copyright file="Credits.cs">
///   Copyright (c) 2024, Marcos Henrique, All rights reserved.
/// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using InputManagement;
using ObjectUtils;
using TMPro;
using LangSystem;

public class Credits : MonoBehaviour
{
    public static bool telaRin = false;
    public AudioSource audioSource;
    public Button rinButton;
    public Button voltarMenu;
    public Animator[] animator;
    void Start()
    {
        audioSource.volume = PlayerPreferences.MusicVolumeScaled;
        rinButton.onClick.AddListener(() => { telaRin = true; });
        voltarMenu.onClick.AddListener(delegate { BackToMenu(); });
        voltarMenu.GetGameObjectComponent<TextMeshProUGUI>("Text").text = Language.currentLanguage.returnToMainMenu;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R) && telaRin)
        {
            Debug.Log("Tezuka Rin");
        }
        if (InputManager.GetKeyDown(KeyBindKey.Escape))
        {
            BackToMenu();
        }
    }

    public void AllWalk()
    {
        for(int i = 0; i < animator.Length; i++)
        {
            animator[i].SetBool("Walking", true);
        }
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
