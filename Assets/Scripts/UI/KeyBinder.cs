using System.Collections;
using System.Collections.Generic;
using InputManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LangSystem;
using ObjectUtils;

public class KeyBinder : MonoBehaviour
{
    public KeyBindKey keyBind;
    public Button InputButton => gameObject.GetGameObjectComponent<Button>("Button");
    public bool inputSelected = false;
    public bool keySelected = false;
    public void Start()
    {
        InputButton.onClick.AddListener(delegate { inputSelected = true; });
        InputManager.LoadKeyBinds();
        gameObject.GetGameObjectComponent<TextMeshProUGUI>("Label").text = Language.currentLanguage.keyNameList[(short)keyBind];
        var text = "None";
        foreach (var key in InputManager.Instance.keyBindList)
        {
            if (key.bind == keyBind)
            {
                text = key.key.ToString();
                continue;
            }
        }
        gameObject.GetGameObjectComponent<TextMeshProUGUI>("Button\\Text").text = text;
    }
    public void Update()
    {
        if (inputSelected)
        {
            gameObject.GetGameObjectComponent<TextMeshProUGUI>("Button\\Text").text = Language.currentLanguage.enterKey;
            if (!keySelected && InputManager.GetKeyCode() != KeyCode.None)
            {
                inputSelected = false;
                keySelected = true;
                SetKeyBind(InputManager.GetKeyCode());
            }
        }
    }
    private void SetKeyBind(KeyCode selectedKey)
    { 
        InputManager.ReplaceKeyBind(new KeyBind(selectedKey, keyBind));
        InputManager.SaveKeyBinds();
        keySelected = false;
        gameObject.GetGameObjectComponent<TextMeshProUGUI>("Button\\Text").text = selectedKey.ToString();
        CanvasGameManager.SetCollectLang();
    }
}
