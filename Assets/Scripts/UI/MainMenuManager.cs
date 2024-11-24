using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ObjectUtils;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using static ObjectUtils.GameObjectGeneral;
using System;
using LangSystem;
using static LangSystem.Language;
using UnityEditor;
using EasterEggs;

public class MainMenuManager : MonoBehaviour
{
    public GameObject langTab;

    #region Main panel
    private GameObject MainPanel => GetGameObject(gameObject, "Mainpanel");
    private TextMeshProUGUI HotText => GetGameObjectComponent<TextMeshProUGUI>(MainPanel, "Hottext");
    private Button PlayButton => GetGameObjectComponent<Button>(MainPanel, "Playbutton");
    private Button LangButton => GetGameObjectComponent<Button>(MainPanel, "Languagebutton");
    private Button CreditsButton => GetGameObjectComponent<Button>(MainPanel, "Creditsbutton");
    private Button ExitButton => GetGameObjectComponent<Button>(MainPanel, "Exitbutton");
    #endregion

    #region New game panel
    private GameObject NewGamePanel => GetGameObject(gameObject, "Newgamepanel");
    private Button ToClassButton => GetGameObjectComponent<Button>(NewGamePanel, "Okbutton");
    private TMP_InputField SeedInput => GetGameObjectComponent<TMP_InputField>(NewGamePanel, "Seedinput");
    private Toggle Toggle80 => GetGameObjectComponent<Toggle>(NewGamePanel, "Toggle80");
    private Toggle Toggle100 => GetGameObjectComponent<Toggle>(NewGamePanel, "Toggle100");
    private Toggle Toggle150 => GetGameObjectComponent<Toggle>(NewGamePanel, "Toggle150");
    private Toggle Toggle200 => GetGameObjectComponent<Toggle>(NewGamePanel, "Toggle200");
    private Toggle ToggleEasy => GetGameObjectComponent<Toggle>(NewGamePanel, "Toggleeasy");
    private Toggle ToggleNormal => GetGameObjectComponent<Toggle>(NewGamePanel, "Togglenormal");
    private Toggle ToggleHard => GetGameObjectComponent<Toggle>(NewGamePanel, "Togglehard");
    private Toggle ToggleLunatic => GetGameObjectComponent<Toggle>(NewGamePanel, "Togglelunatic");
    private GameObject ClassPanel => GetGameObject(NewGamePanel, "Classpanel");
    private Button ReturnClassPanelButton => GetGameObjectComponent<Button>(ClassPanel, "Returnbutton");
    private Button GenerateGameButton => GetGameObjectComponent<Button>(ClassPanel, "Generatebutton");
    #endregion

    #region Language panel
    private ScrollConfigurator LanguagePanel => GetGameObjectComponent<ScrollConfigurator>(gameObject, "Languagepanel");
    #endregion

    #region Load panel
    private GameObject LoadPanel => GetGameObject(gameObject, "Loadpanel");
    private Slider LoadBar => GetGameObjectComponent<Slider>(LoadPanel, "Loadbar");
    #endregion

    private GameObject[] PanelList = new GameObject[3];
    private int selectedMapWidth;
    private int selectedMapHeight;
    private string selectedDifficulty;
    private string selectedClass;

    [SerializeField]
    [Serializable]
    public enum Panel : byte
    {
        Mainpanel = 0,
        Newgamepanel = 1,
        LanguagePanel = 2,
    }
    private Panel _currentPanel;
    public Panel currentPanel
    {
        get
        {
            return _currentPanel;
        }
        set
        {
            _currentPanel = value;
            foreach (var gpanel in PanelList)
            {
                if (gpanel == null)
                    break;
                gpanel.SetActive(false);
            }
            PanelList[(byte)value].SetActive(true);
        }
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("DEFAULTED", 0) == 0)
        {
            Debug.Log("Game defaulted succesfullyyy");
            PlayerPrefs.SetInt("DEFAULTED", 1);
            MakeDefaultConfig();
        }
        LoadPanel.SetActive(false);

        selectedMapWidth = 100;
        selectedMapHeight = 100;
        selectedDifficulty = "Normal";

        GetLanguage();

        float offset = 0f;

        foreach (var language in Resources.LoadAll<Language>("Lang"))
        {
            var languageTab = Instantiate(langTab, LanguagePanel.scrollContainer.transform);
            languageTab.GetComponent<RectTransform>().localPosition = new Vector3(0, (float)offset, 0f);
            languageTab.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = language.name;

            languageTab.GetComponent<Button>().onClick.AddListener(() => { SetLanguage(language.name); SetAllLang(); });

            offset += langTab.GetComponent<RectTransform>().sizeDelta.y + 3f;
        }

        PanelList[0] = (MainPanel);
        PanelList[1] = (NewGamePanel);
        PanelList[2] = (LanguagePanel.gameObject);

        SetAllLang();
        GetGameObjectComponent<TextMeshProUGUI>(MainPanel, "Version").text = "v" + Application.version;

        PlayButton.onClick.AddListener(() => { OpenMenu(Panel.Newgamepanel); });
        LangButton.onClick.AddListener(() => { OpenMenu(Panel.LanguagePanel); });
        ExitButton.onClick.AddListener(() => { Application.Quit(); });

        ToClassButton.onClick.AddListener(() => { ClassPanel.SetActive(true); });
        ReturnClassPanelButton.onClick.AddListener(() => { ClassPanel.SetActive(false); });
        GenerateGameButton.onClick.AddListener(() => { GenerateNewWorld(); });

        #region secret
        HotText.text = HotTexts.texts[UnityEngine.Random.Range(0, HotTexts.texts.Count - 1)];
        #endregion

        SeedInput.onEndEdit.AddListener((text) => { if (text.Length == 0 || !int.TryParse(text, out var a)) { SeedInput.text = UnityEngine.Random.Range(10000, 999999) + ""; } });

        Toggle80.onValueChanged.AddListener((a) => { DeactivateAllMapSizeToggles(); Toggle80.SetIsOnWithoutNotify(true); selectedMapWidth = 80; selectedMapHeight = 80; });
        Toggle100.onValueChanged.AddListener((a) => { DeactivateAllMapSizeToggles(); Toggle100.SetIsOnWithoutNotify(true); selectedMapWidth = 100; selectedMapHeight = 100; });
        Toggle150.onValueChanged.AddListener((a) => { DeactivateAllMapSizeToggles(); Toggle150.SetIsOnWithoutNotify(true); selectedMapWidth = 150; selectedMapHeight = 150; });
        Toggle200.onValueChanged.AddListener((a) => { DeactivateAllMapSizeToggles(); Toggle200.SetIsOnWithoutNotify(true); selectedMapWidth = 200; selectedMapHeight = 200; });

        ToggleEasy.onValueChanged.AddListener((a) => { DeactivateAllDifficultyToggles(); ToggleEasy.SetIsOnWithoutNotify(true); selectedDifficulty = "Easy"; });
        ToggleNormal.onValueChanged.AddListener((a) => { DeactivateAllDifficultyToggles(); ToggleNormal.SetIsOnWithoutNotify(true); selectedDifficulty = "Normal"; });
        ToggleHard.onValueChanged.AddListener((a) => { DeactivateAllDifficultyToggles(); ToggleHard.SetIsOnWithoutNotify(true); selectedDifficulty = "Hard"; });
        ToggleLunatic.onValueChanged.AddListener((a) => { DeactivateAllDifficultyToggles(); ToggleLunatic.SetIsOnWithoutNotify(true); selectedDifficulty = "Lunatic"; });

        GetGameObjectComponent<Button>(ClassPanel, "Subpanel/Wizardmask/Wizardbutton").onClick.AddListener(() => { selectedClass = "Wizard"; DeactivateClassPanelHighlights(); GetGameObject(ClassPanel, "Subpanel/Wizardmask/Wizardbutton/Highlight").SetActive(true); });
        GetGameObjectComponent<Button>(ClassPanel, "Subpanel/Warriormask/Warriorbutton").onClick.AddListener(() => { selectedClass = "Warrior"; DeactivateClassPanelHighlights(); GetGameObject(ClassPanel, "Subpanel/Warriormask/Warriorbutton/Highlight").SetActive(true); });
        GetGameObjectComponent<Button>(ClassPanel, "Subpanel/Archermask/Archerbutton").onClick.AddListener(() => { selectedClass = "Archer"; DeactivateClassPanelHighlights(); GetGameObject(ClassPanel, "Subpanel/Archermask/Archerbutton/Highlight").SetActive(true); });

        Toggle100.isOn = true;
        ToggleNormal.isOn = true;

        SeedInput.text = UnityEngine.Random.Range(10000, 999999) + "";

        DeactivateClassPanelHighlights();
        GetGameObject(ClassPanel, "Subpanel/Warriormask/Warriorbutton/Highlight").SetActive(true);
        selectedClass = "Warrior";
        OpenMenu(Panel.Mainpanel);
    }
    void Update()
    {
        
    }
    public void DeactivateClassPanelHighlights()
    {
        GetGameObject(ClassPanel, "Subpanel/Wizardmask/Wizardbutton/Highlight").SetActive(false);
        GetGameObject(ClassPanel, "Subpanel/Warriormask/Warriorbutton/Highlight").SetActive(false);
        GetGameObject(ClassPanel, "Subpanel/Archermask/Archerbutton/Highlight").SetActive(false);
    }
    public void SetAllLang()
    {
        //main menu
        GetGameObjectComponent<TextMeshProUGUI>(PlayButton.gameObject, "Text").text = currentLanguage.playNewGame;
        GetGameObjectComponent<TextMeshProUGUI>(LangButton.gameObject, "Text").text = currentLanguage.language;
        GetGameObjectComponent<TextMeshProUGUI>(ExitButton.gameObject, "Text").text = currentLanguage.exitGame;

        //new game menu
        GetGameObjectComponent<TextMeshProUGUI>(NewGamePanel, "Title").text = currentLanguage.generateNewWorld;

        GetGameObjectComponent<TextMeshProUGUI>(NewGamePanel, "Seedtext").text = currentLanguage.seed;
        SeedInput.placeholder.gameObject.GetComponent<TextMeshProUGUI>().text = currentLanguage.enterNumber + "...";

        GetGameObjectComponent<TextMeshProUGUI>(NewGamePanel, "Mapsizetext").text = currentLanguage.mapSize;
        GetGameObjectComponent<TextMeshProUGUI>(GenerateGameButton.gameObject, "Text").text = currentLanguage.ok;

        //class menu
        GetGameObjectComponent<TextMeshProUGUI>(ClassPanel, "Subpanel/Wizardmask/Wizardbutton/Name").text = currentLanguage.wizard;
        GetGameObjectComponent<TextMeshProUGUI>(ClassPanel, "Subpanel/Warriormask/Warriorbutton/Name").text = currentLanguage.warrior;
        GetGameObjectComponent<TextMeshProUGUI>(ClassPanel, "Subpanel/Archermask/Archerbutton/Name").text = currentLanguage.archer;
        GetGameObjectComponent<TextMeshProUGUI>(GenerateGameButton.gameObject, "Text").text = currentLanguage.generate;
        GetGameObjectComponent<TextMeshProUGUI>(ReturnClassPanelButton.gameObject, "Text").text = currentLanguage.Return;

        //language menu
        GetGameObjectComponent<TextMeshProUGUI>(LanguagePanel.gameObject, "Title").text = currentLanguage.language;
        GetGameObjectComponent<TextMeshProUGUI>(LanguagePanel.gameObject, "Exitbutton/Text").text = currentLanguage.exit;
        GetGameObjectComponent<TextMeshProUGUI>(ClassPanel, "Title").text = currentLanguage.chooseYourClass;

        //loading panel
        LoadPanel.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = currentLanguage.loading;
    }
    public void OpenMenu(Panel panel)
    {
        currentPanel = panel;
    }
    public void OpenMenu(int panel) => OpenMenu((Panel)panel);
    private void GenerateNewWorld()
    {
        PlayerPrefs.SetInt("CURRENT_SEED", int.Parse(SeedInput.text));
        PlayerPrefs.SetInt("MAP_WIDTH", selectedMapWidth);
        PlayerPrefs.SetInt("MAP_HEIGHT", selectedMapHeight);
        PlayerPrefs.SetInt("STAGE_OFFSET", PlayerPrefs.GetInt("MAP_WIDTH", 100));
        PlayerPrefs.SetInt("CURRENT_STAGE", 1);
        PlayerPrefs.SetString("DIFFICULTY", selectedDifficulty);
        PlayerPrefs.SetString("CLASS", selectedClass);
        PlayerPrefs.Save();
        StartCoroutine(LoadAsyncGame(1));
    }
    private IEnumerator LoadAsyncGame(int index)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
        LoadPanel.SetActive(true);

        while (!asyncLoad.isDone)
        {
            LoadBar.value = asyncLoad.progress;
            yield return null;
        }
    }
    private void DeactivateAllMapSizeToggles()
    {
        Toggle80.SetIsOnWithoutNotify(false);
        Toggle100.SetIsOnWithoutNotify(false);
        Toggle150.SetIsOnWithoutNotify(false);
        Toggle200.SetIsOnWithoutNotify(false);
    }
    private void DeactivateAllDifficultyToggles()
    {
        ToggleEasy.SetIsOnWithoutNotify(false);
        ToggleNormal.SetIsOnWithoutNotify(false);
        ToggleHard.SetIsOnWithoutNotify(false);
        ToggleLunatic.SetIsOnWithoutNotify(false);
    }
    private void MakeDefaultConfig()
    {
        PlayerPrefs.SetFloat("MASTER_VOLUME", 0.8f);
        PlayerPrefs.SetFloat("MUSIC_VOLUME", 1f);
        PlayerPrefs.SetFloat("SOUND_EFFECTS_VOLUME", 1f);
        PlayerPrefs.SetFloat("UI_VOLUME", 1f);
        PlayerPrefs.SetInt("MAP_WIDTH", 100);
        PlayerPrefs.SetInt("MAP_HEIGHT", 100);
        PlayerPrefs.SetString("CLASS", "Warrior");
        InputManagement.InputManager.ResetKeyBindToDefault();
    }
}
