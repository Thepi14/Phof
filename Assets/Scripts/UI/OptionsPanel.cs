using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectUtils;
using static ObjectUtils.GameObjectGeneral;
using UnityEngine.UI;
using TMPro;
using static LangSystem.Language;

public class OptionsPanel : MonoBehaviour
{
    public Button ConfigButton => GetGameObjectComponent<Button>(gameObject, "Subpanel\\Configbutton");
    public Button VolumeButton => GetGameObjectComponent<Button>(gameObject, "Subpanel\\Volumebutton");
    public Button KeybindButton => GetGameObjectComponent<Button>(gameObject, "Subpanel\\Keybindbutton");
    public Button ExitButton => GetGameObjectComponent<Button>(gameObject, "Subpanel\\Exitbutton");

    public Button QuitGameButton;

    void Start()
    {
        ConfigButton.onClick.AddListener(() => { });
        VolumeButton.onClick.AddListener(() => { });
        KeybindButton.onClick.AddListener(() => { });
        ExitButton.onClick.AddListener(() => { gameObject.SetActive(false); });

        //Lang
        ExitButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = currentLanguage.exit;

        if (QuitGameButton != null)
        {
            QuitGameButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = currentLanguage.exitGame;
            QuitGameButton.onClick.AddListener(() => { Application.Quit(); });
        }
    }
    void Update()
    {
        
    }
}
