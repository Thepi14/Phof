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
    private GameObject MainPanel => gameObject.GetGameObject("Mainpanel");
    private TextMeshProUGUI HotText => MainPanel.GetGameObjectComponent<TextMeshProUGUI>("Hottext");
    private Button PlayButton => MainPanel.GetGameObjectComponent<Button>("Playbutton");
    private Button ContinueGameButton => MainPanel.GetGameObjectComponent<Button>("Continuegamebutton");
    private Button LangButton => MainPanel.GetGameObjectComponent<Button>("Languagebutton");
    private Button CreditsButton => MainPanel.GetGameObjectComponent<Button>("Creditsbutton");
    private Button ExitButton => MainPanel.GetGameObjectComponent<Button>("Exitbutton");
    #endregion

    #region New game panel
    private GameObject NewGamePanel => gameObject.GetGameObject("Newgamepanel");
    private Button ToClassButton => NewGamePanel.GetGameObjectComponent<Button>("Okbutton");
    private TMP_InputField SeedInput => NewGamePanel.GetGameObjectComponent<TMP_InputField>("Seedinput");
    private Toggle Toggle80 => NewGamePanel.GetGameObjectComponent<Toggle>("Toggle80");
    private Toggle Toggle100 => NewGamePanel.GetGameObjectComponent<Toggle>("Toggle100");
    private Toggle Toggle150 => NewGamePanel.GetGameObjectComponent<Toggle>("Toggle150");
    private Toggle Toggle200 => NewGamePanel.GetGameObjectComponent<Toggle>("Toggle200");
    private Toggle ToggleEasy => NewGamePanel.GetGameObjectComponent<Toggle>("Toggleeasy");
    private Toggle ToggleNormal => NewGamePanel.GetGameObjectComponent<Toggle>("Togglenormal");
    private Toggle ToggleHard => NewGamePanel.GetGameObjectComponent<Toggle>("Togglehard");
    private Toggle ToggleLunatic => NewGamePanel.GetGameObjectComponent<Toggle>("Togglelunatic");
    private GameObject ClassPanel => NewGamePanel.GetGameObject("Classpanel");
    private Button ReturnClassPanelButton => ClassPanel.GetGameObjectComponent<Button>("Returnbutton");
    private Button GenerateGameButton => ClassPanel.GetGameObjectComponent<Button>("Generatebutton");
    #endregion

    #region Language panel
    private ScrollConfigurator LanguagePanel => gameObject.GetGameObjectComponent<ScrollConfigurator>("Languagepanel");
    #endregion

    #region Load panel
    private GameObject LoadPanel => gameObject.GetGameObject("Loadpanel");
    private Slider LoadBar => LoadPanel.GetGameObjectComponent<Slider>("Loadbar");
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
        LoadPanel.SetActive(false);
        gameObject.GetGameObjectComponent<Animator>("Black").Play("FadeOut");

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
        MainPanel.GetGameObjectComponent<TextMeshProUGUI>("Version").text = "v" + Application.version;

        PlayButton.onClick.AddListener(() => { OpenMenu(Panel.Newgamepanel); });
        ContinueGameButton.onClick.AddListener(() => { SceneManager.LoadScene(1); });
        LangButton.onClick.AddListener(() => { OpenMenu(Panel.LanguagePanel); });
        CreditsButton.onClick.AddListener(() => { SceneManager.LoadScene(3); });
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

        ClassPanel.GetGameObjectComponent<Button>("Subpanel/Wizardmask/Wizardbutton").onClick.AddListener(() => { selectedClass = "Wizard"; DeactivateClassPanelHighlights(); ClassPanel.GetGameObject("Subpanel/Wizardmask/Wizardbutton/Highlight").SetActive(true); });
        ClassPanel.GetGameObjectComponent<Button>("Subpanel/Warriormask/Warriorbutton").onClick.AddListener(() => { selectedClass = "Warrior"; DeactivateClassPanelHighlights(); ClassPanel.GetGameObject("Subpanel/Warriormask/Warriorbutton/Highlight").SetActive(true); });
        ClassPanel.GetGameObjectComponent<Button>("Subpanel/Archermask/Archerbutton").onClick.AddListener(() => { selectedClass = "Archer"; DeactivateClassPanelHighlights(); ClassPanel.GetGameObject("Subpanel/Archermask/Archerbutton/Highlight").SetActive(true); });

        Toggle100.isOn = true;
        ToggleNormal.isOn = true;

        SeedInput.text = UnityEngine.Random.Range(10000, 999999) + "";

        DeactivateClassPanelHighlights();
        ClassPanel.GetGameObject("Subpanel/Warriormask/Warriorbutton/Highlight").SetActive(true);
        selectedClass = "Warrior";
        OpenMenu(Panel.Mainpanel);
    }
    void Update()
    {
        
    }
    public void DeactivateClassPanelHighlights()
    {
        ClassPanel.GetGameObject("Subpanel/Wizardmask/Wizardbutton/Highlight").SetActive(false);
        ClassPanel.GetGameObject("Subpanel/Warriormask/Warriorbutton/Highlight").SetActive(false);
        ClassPanel. GetGameObject("Subpanel/Archermask/Archerbutton/Highlight").SetActive(false);
    }
    public void SetAllLang()
    {
        //main menu
        PlayButton.gameObject.GetGameObjectComponent<TextMeshProUGUI>("Text").text = currentLanguage.playNewGame;
        ContinueGameButton.gameObject.GetGameObjectComponent<TextMeshProUGUI>("Text").text = currentLanguage.continueGame;
        LangButton.gameObject.GetGameObjectComponent<TextMeshProUGUI>("Text").text = currentLanguage.language;
        CreditsButton.gameObject.GetGameObjectComponent<TextMeshProUGUI>("Text").text = currentLanguage.credits;
        ExitButton.gameObject.GetGameObjectComponent<TextMeshProUGUI>("Text").text = currentLanguage.exitGame;

        //new game menu
        NewGamePanel.GetGameObjectComponent<TextMeshProUGUI>("Title").text = currentLanguage.generateNewWorld;

        NewGamePanel.GetGameObjectComponent<TextMeshProUGUI>("Seedtext").text = currentLanguage.seed;
        SeedInput.placeholder.gameObject.GetComponent<TextMeshProUGUI>().text = currentLanguage.enterNumber + "...";

        NewGamePanel.GetGameObjectComponent<TextMeshProUGUI>("Mapsizetext").text = currentLanguage.mapSize;
        GenerateGameButton.gameObject.GetGameObjectComponent<TextMeshProUGUI>("Text").text = currentLanguage.ok;

        //class menu
        ClassPanel.GetGameObjectComponent<TextMeshProUGUI>("Subpanel/Wizardmask/Wizardbutton/Name").text = currentLanguage.wizard;
        ClassPanel.GetGameObjectComponent<TextMeshProUGUI>("Subpanel/Warriormask/Warriorbutton/Name").text = currentLanguage.warrior;
        ClassPanel.GetGameObjectComponent<TextMeshProUGUI>("Subpanel/Archermask/Archerbutton/Name").text = currentLanguage.archer;
        GenerateGameButton.gameObject.GetGameObjectComponent<TextMeshProUGUI>("Text").text = currentLanguage.generate;
        ReturnClassPanelButton.gameObject.GetGameObjectComponent<TextMeshProUGUI>("Text").text = currentLanguage.Return;

        //language menu
        LanguagePanel.gameObject.GetGameObjectComponent<TextMeshProUGUI>("Title").text = currentLanguage.language;
        LanguagePanel.gameObject.GetGameObjectComponent<TextMeshProUGUI>("Exitbutton/Text").text = currentLanguage.exit;
        ClassPanel.GetGameObjectComponent<TextMeshProUGUI>("Title").text = currentLanguage.chooseYourClass;

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
}
