// --------------------------------------------------------------------------------------------------------------------
/// <copyright file="OptionPanel.cs">
///   Copyright (c) 2024, Pi14, All rights reserved.
/// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectUtils;
using UnityEngine.UI;
using TMPro;
using static LangSystem.Language;
using UnityEngine.SceneManagement;
using static GamePlayer;
using InputManagement;

public class OptionsPanel : MonoBehaviour
{
    public GameObject SubPanel => gameObject.GetGameObject("Subpanel");
    public GameObject ConfigPanel => gameObject.GetGameObject("Configurationspanel");
    public GameObject VolumePanel => gameObject.GetGameObject("Volumepanel");
    public GameObject KeybindPanel => gameObject.GetGameObject("Keybindpanel");

    public Button ConfigButton => SubPanel.GetGameObjectComponent<Button>("Configbutton");
    public Button VolumeButton => SubPanel.GetGameObjectComponent<Button>("Volumebutton");
    public Button KeybindButton => SubPanel.GetGameObjectComponent<Button>("Keybindbutton");
    public Button ExitButton => SubPanel.GetGameObjectComponent<Button>("Exitbutton");

    public Slider MasterVolume => VolumePanel.GetGameObjectComponent<Slider>("Mastervolume");
    public Slider MusicVolume => VolumePanel.GetGameObjectComponent<Slider>("Musicvolume");
    public Slider SoundEffectsVolume => VolumePanel.GetGameObjectComponent<Slider>("Soundeffectsvolume");
    public Slider UIVolume => VolumePanel.GetGameObjectComponent<Slider>("UIvolume");
    public Button VolumeExitButton => VolumePanel.GetGameObjectComponent<Button>("Exitbutton");

    public Button QuitGameButton;
    public Button ReturnToMainMenuButton;
    public static bool keyBinding = false;

    public enum Panel : byte
    {
        None = 0,
        ConfigurationsPanel = 1,
        VolumePanel = 2,
        KeybindPanel = 3,
    }
    public Panel currentPanel;
    public void OpenPanel(Panel panel)
    {
        currentPanel = panel;
        CloseAllPanels();
        if (panel == Panel.KeybindPanel)
            keyBinding = true;
        gameObject.GetGameObjectChildren()[(byte)panel].SetActive(true);
    }
    public void CloseAllPanels()
    {
        SubPanel.SetActive(false);
        ConfigPanel.SetActive(false);
        VolumePanel.SetActive(false);
        KeybindPanel.SetActive(false);
        keyBinding = false;
    }
    public void ExitMenu()
    {
        OpenPanel(0);
        gameObject.SetActive(false);
    }
    void Start()
    {
        ConfigButton.onClick.AddListener(() => { OpenPanel(Panel.ConfigurationsPanel); });
        VolumeButton.onClick.AddListener(() => { OpenPanel(Panel.VolumePanel); });
        KeybindButton.onClick.AddListener(() => { OpenPanel(Panel.KeybindPanel); });
        ExitButton.onClick.AddListener(() => { ExitMenu(); });

        MasterVolume.onValueChanged.AddListener((value) => { PlayerPrefs.SetFloat("MASTER_VOLUME", value); PlayerPrefs.Save(); });
        MusicVolume.onValueChanged.AddListener((value) => { PlayerPrefs.SetFloat("MUSIC_VOLUME", value); PlayerPrefs.Save(); });
        SoundEffectsVolume.onValueChanged.AddListener((value) => { PlayerPrefs.SetFloat("SOUND_EFFECTS_VOLUME", value); PlayerPrefs.Save(); });
        UIVolume.onValueChanged.AddListener((value) => { PlayerPrefs.SetFloat("UI_VOLUME", value); PlayerPrefs.Save(); });

        VolumeExitButton.onClick.AddListener(() => { OpenPanel(0); });
        ConfigPanel.GetGameObjectComponent<Button>("Exitbutton").onClick.AddListener(() => { OpenPanel(0); });
        KeybindPanel.GetGameObjectComponent<Button>("Exitbutton").onClick.AddListener(() => { OpenPanel(0); });

        MasterVolume.value = PlayerPrefs.GetFloat("MASTER_VOLUME", 0.8f);
        MusicVolume.value = PlayerPrefs.GetFloat("MUSIC_VOLUME", 1f);
        SoundEffectsVolume.value = PlayerPrefs.GetFloat("SOUND_EFFECTS_VOLUME", 1f);
        UIVolume.value = PlayerPrefs.GetFloat("UI_VOLUME", 1f);

        ConfigPanel.GetGameObjectComponent<Toggle>("Toggleshowdamagemsg").onValueChanged.AddListener((a) => { PlayerPreferences.ShowDamage = a; PlayerPrefs.Save(); });
        ConfigPanel.GetGameObjectComponent<Toggle>("Toggleorthographic").onValueChanged.AddListener((a) => { PlayerPreferences.Orthographic = a; PlayerPrefs.Save(); });
        ConfigPanel.GetGameObjectComponent<Toggle>("Toggleshowcoord").onValueChanged.AddListener((a) => { PlayerPreferences.ShowCoordinates = a; PlayerPrefs.Save(); });
        ConfigPanel.GetGameObjectComponent<Toggle>("Toggleshowfps").onValueChanged.AddListener((a) => { PlayerPreferences.ShowFPS = a; PlayerPrefs.Save(); });
        ConfigPanel.GetGameObjectComponent<Button>("Resetbutton").onClick.AddListener(() => { PlayerPreferences.Reset(); SetConfigAll(); });

        void SetConfigAll()
        {
            ConfigPanel.transform.Find("Toggleshowdamagemsg").GetComponent<Toggle>().SetIsOnWithoutNotify(PlayerPreferences.ShowDamage);
            ConfigPanel.transform.Find("Toggleorthographic").GetComponent<Toggle>().SetIsOnWithoutNotify(PlayerPreferences.Orthographic);
            ConfigPanel.transform.Find("Toggleshowcoord").GetComponent<Toggle>().SetIsOnWithoutNotify(PlayerPreferences.ShowCoordinates);
            ConfigPanel.transform.Find("Toggleshowfps").GetComponent<Toggle>().SetIsOnWithoutNotify(PlayerPreferences.ShowFPS);
        }
        SetConfigAll();

        VolumeExitButton.onClick.AddListener(() => { OpenPanel(0); });

        SetAllLang();

        if (ReturnToMainMenuButton != null)
        {
            ReturnToMainMenuButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = currentLanguage.returnToMainMenu;
            ReturnToMainMenuButton.onClick.AddListener(() => 
            { 
                if (player != null && !PlayerPreferences.Died)
                    PlayerPreferences.SavePlayerData(player.EntityData);
                else
                    PlayerPreferences.GameSaved = false;

                PlayerPreferences.Died = false;
                PlayerPrefs.Save();
                DontDestroyOnLoadManager.DestroyAll();
                SceneManager.LoadSceneAsync(0);
            });
        }
        if (QuitGameButton != null)
        {
            QuitGameButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = currentLanguage.exitGame;
            QuitGameButton.onClick.AddListener(() =>
            {
                if (player != null && !PlayerPreferences.Died)
                    PlayerPreferences.SavePlayerData(player.EntityData);
                else
                    PlayerPreferences.GameSaved = false;

                PlayerPreferences.Died = false;
                PlayerPrefs.Save();
                DontDestroyOnLoadManager.DestroyAll();
#if UNITY_EDITOR
                SceneManager.LoadSceneAsync(0);
#else
                Application.Quit();
#endif
            });
        }

        OpenPanel(0);
    }
    private void SetAllLang()
    {
        SubPanel.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = currentLanguage.options;
        ConfigButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = currentLanguage.configurations;
        VolumeButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = currentLanguage.volume;
        KeybindButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = currentLanguage.keybind;
        ExitButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = currentLanguage.exitOptionsMenu;

        //volume panel
        VolumePanel.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = currentLanguage.volume;
        MasterVolume.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = currentLanguage.masterVolume;
        MusicVolume.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = currentLanguage.musicVolume;
        SoundEffectsVolume.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = currentLanguage.soundEffectVolume;
        UIVolume.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = currentLanguage.UIVolume;
        VolumePanel.GetGameObjectComponent<TextMeshProUGUI>("Exitbutton/Text").text = currentLanguage.exit;

        //configurations panel
        ConfigPanel.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = currentLanguage.configurations;
        ConfigPanel.GetGameObjectComponent<TextMeshProUGUI>("Toggleshowdamagemsg/Label").text = currentLanguage.showDamage;
        ConfigPanel.GetGameObjectComponent<TextMeshProUGUI>("Toggleorthographic/Label").text = currentLanguage.orthographic;
        ConfigPanel.GetGameObjectComponent<TextMeshProUGUI>("Toggleshowcoord/Label").text = currentLanguage.showCoordinates;
        ConfigPanel.GetGameObjectComponent<TextMeshProUGUI>("Toggleshowfps/Label").text = currentLanguage.showFPS;
        ConfigPanel.GetGameObjectComponent<TextMeshProUGUI>("Resetbutton/Text").text = currentLanguage.reset;
        ConfigPanel.GetGameObjectComponent<TextMeshProUGUI>("Exitbutton/Text").text = currentLanguage.exit;

        //keybind panel
        KeybindPanel.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = currentLanguage.keybind;

        KeybindPanel.GetGameObjectComponent<TextMeshProUGUI>("Exitbutton/Text").text = currentLanguage.exit;
    }
    void Update()
    {
        if (keyBinding)
            return;
        if (InputManager.GetKeyDown(KeyBindKey.Escape))
        {
            if (currentPanel != 0)
                OpenPanel(0);
            else
                ExitMenu();
        }
    }
}
