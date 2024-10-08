using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ObjectUtils;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private GameObject MainPanel => GameObjectGeneral.GetGameObject(gameObject, "Mainpanel");
    private Button PlayButton => GameObjectGeneral.GetGameObjectComponent<Button>(MainPanel, "Playbutton");

    private GameObject NewGamePanel => GameObjectGeneral.GetGameObject(gameObject, "Newgamepanel");
    private Button GenerateGameButton => GameObjectGeneral.GetGameObjectComponent<Button>(NewGamePanel, "Generatebutton");
    private TMP_InputField SeedInput => GameObjectGeneral.GetGameObjectComponent<TMP_InputField>(NewGamePanel, "Seedinput");

    private int seed;

    private GameObject[] PanelList = new GameObject[2];

    private enum Panel : byte
    {
        Mainpanel = 0,
        Newgamepanel = 1,
        Loadgamepanel = 2
    }
    private Panel currentPanel;

    void Start()
    {
        PanelList[0] = (MainPanel);
        PanelList[1] = (NewGamePanel);

        OpenMenu(Panel.Mainpanel);

        PlayButton.onClick.AddListener(() => { OpenMenu(Panel.Newgamepanel); });
        GenerateGameButton.onClick.AddListener(() => { GenerateNewWorld(); });

        SeedInput.onValueChanged.AddListener((text) => { if (text.Length == 0 || !int.TryParse(text, out var a)) { seed = UnityEngine.Random.Range(10000, 999999); SeedInput.text = seed + ""; } else seed = int.Parse(text); });
    }
    void Update()
    {
        
    }
    private void OpenMenu(Panel panel)
    {
        currentPanel = panel;
        foreach(var gpanel in PanelList)
        {
            if (gpanel == null)
                break;
            gpanel.SetActive(false);
        }
        PanelList[(byte)panel].SetActive(true);
    }
    private void GenerateNewWorld()
    {
        PlayerPrefs.SetInt("CURRENT_SEED", seed);
        PlayerPrefs.Save();
        SceneManager.LoadScene(1);
    }
}
