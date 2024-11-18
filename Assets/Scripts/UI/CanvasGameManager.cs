using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static GamePlayer;
using UnityEngine.UI;
using ItemSystem;
using System;
using static LangSystem.Language;
using ObjectUtils;
using static ObjectUtils.GameObjectGeneral;
using HabilitySystem;
using EntityDataSystem;

public class CanvasGameManager : MonoBehaviour
{
    public bool seeingMap = false;
    public static CanvasGameManager canvasInstance;
    public GameObject MainPanel => GetGameObject(gameObject, "Mainpanel");
    public RectTransform LifeBar => GetGameObjectComponent<RectTransform>(gameObject, "Mainpanel/LifeBar/Bar");
    public RectTransform StaminaBar => GetGameObjectComponent<RectTransform>(gameObject, "Mainpanel/StaminaBar/Bar");
    public RectTransform ManaBar => GetGameObjectComponent<RectTransform>(gameObject, "Mainpanel/ManaBar/Bar");
    public GameObject CardsMain => GetGameObject(gameObject, "Mainpanel/Cards");
    public GameObject LoadPanel => GetGameObject(gameObject, "Loadpanel");
    public GameObject CardPanel => GetGameObject(gameObject, "Cardpanel");
    public GameObject CardPanelExibition => GetGameObject(CardPanel, "Subpanel");
    public Slider LoadBar => GetGameObjectComponent<Slider>(LoadPanel, "Loadbar");

    public Button sword;
    public Button staff;

    public Item swordItem;
    public Item staffItem;

    public GameObject cardPrefab;
    public GameObject cardGamePrefab;

    public List<GameObject> currentCards = new List<GameObject>();
    public List<string> cardsAlreadyGotList = new List<string>();

    public List<string> currentPlayerCards
    {
        get
        {
            var newList = new List<string>();
            foreach (var items in player.EntityData.habilities)
            {
                newList.Add(items.Key);
            }
            return newList;
        }
    }

    private void Start()
    {
        if (canvasInstance == null)
            canvasInstance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        SetLang();
        sword.onClick.AddListener(() => { player.SetItem(swordItem); });
        staff.onClick.AddListener(() => { player.SetItem(staffItem); }); 
    }
    public void SetLang()
    {
        LoadPanel.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = currentLanguage.loading;
    }
    private void Update()
    {
        LoadPanel.SetActive(!TerrainGeneration.Instance.mapLoaded);
        if (!TerrainGeneration.Instance.mapLoaded)
        {
            LoadBar.value = TerrainGeneration.Instance.generationProgress;
        }
        if (player != null)
        {
            LifeBar.localScale = new Vector3(Mathf.Max((float)player.EntityData.currentHealth / player.EntityData.maxHealth, 0), 1, 1);
            StaminaBar.localScale = new Vector3(Mathf.Max((float)player.EntityData.currentStamina / player.EntityData.maxStamina, 0), 1, 1);
            ManaBar.localScale = new Vector3(Mathf.Max((float)player.EntityData.currentMana / player.EntityData.maxMana, 0), 1, 1);
        }
        BlockCards();
    }
    public enum GameMenu : byte
    {
        MainPanel = 0,
        CardsPanel = 1,
        LoadPanel = 2,
    }
    public GameMenu currentMenu;
    public void OpenMenu(GameMenu menu)
    {
        CloseAllPanels();
        switch (menu)
        {
            case GameMenu.MainPanel:
                MainPanel.SetActive(true);
                break;
            case GameMenu.CardsPanel:
                CardPanel.SetActive(true);
                break;
            case GameMenu.LoadPanel:
                LoadPanel.SetActive(true);
                break;
        }
    }
    public void CloseAllPanels()
    {
        MainPanel.SetActive(false);
        CardPanel.SetActive(false);
        LoadPanel.SetActive(false);
    }
    public GameObject AddCard(HabilityBehaviour hability)
    {
        var card = Instantiate(cardGamePrefab, CardsMain.transform);
        currentCards.Add(card);
        card.transform.Find("Image").GetComponent<Image>().sprite = hability.habilitySprite;
        hability.gameObject.GetComponent<IEntity>().EntityData.habilities.Add(hability.habilityID, hability);
        hability.card = card;
        card.GetComponent<Button>().onClick.AddListener(() => { hability.ExecuteHability(player.EntityData.target); });
        return card;
    }
    public void BlockCards()
    {
        if (player == null)
            return;
        foreach (var hability in player.EntityData.habilities)
        {
            switch (hability.Value.type)
            {
                case HabilityType.basic:
                    hability.Value.card.transform.Find("Reload").GetComponent<Image>().fillAmount = 1f - (hability.Value.cooldownTimer / hability.Value.cooldown);
                    break;
                case HabilityType.special or HabilityType.ultimate:
                    hability.Value.card.transform.Find("Reload").GetComponent<Image>().fillAmount = 1f;
                    break;
            }
        }
    }
    public void RemoveCard(HabilityBehaviour hability)
    {
        Destroy(hability.card);
        currentCards.Remove(hability.card);
        Destroy(player.EntityData.habilities[hability.habilityID]);
        player.EntityData.habilities.Remove(hability.habilityID);
    }
    public void RandomizeCards()
    {
        Debug.Log($"{cardsAlreadyGotList.Count}, {CardChoice.habilitiesIDs.Count - 2}, {CardPanelExibition.transform.childCount}");
        if (cardsAlreadyGotList.Count >= CardChoice.habilitiesIDs.Count - 2 && CardPanelExibition.transform.childCount > 1)
        {
            Destroy(CardPanelExibition.transform.GetChild(1).gameObject);
        }
        var cardObjList = new List<CardChoice>();
        for (int i = 0; i < CardPanelExibition.transform.childCount; i++)
        {
            cardObjList.Add(CardPanelExibition.transform.GetChild(i).GetComponent<CardChoice>());
        }
        if (cardsAlreadyGotList.Count < CardChoice.habilitiesIDs.Count)
            foreach (CardChoice choice in cardObjList)
            {
            remakeCard:;
                string id = CardChoice.habilitiesIDs[UnityEngine.Random.Range(0, CardChoice.habilitiesIDs.Count)];
                if (currentPlayerCards.Contains(id))
                {
                    goto remakeCard;
                }
                cardsAlreadyGotList.Add(id);
                choice.SetCard(id);
            }
    }
}
