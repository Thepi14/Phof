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
using static GameManagerSystem.GameManager;
using System.Threading.Tasks;

public class CanvasGameManager : MonoBehaviour
{
    public static CanvasGameManager canvasInstance;
    public bool espada = false;
    public bool seeingMap = false, seeingInventory = false;
    public GameObject inventory => gameObject.GetGameObject("Mainpanel/Inventory");
    public Inventory slots => gameObject.GetGameObjectComponent<Inventory>("Mainpanel/Inventory");
    public GameObject MainPanel => gameObject.GetGameObject("Mainpanel");
    public Slider LifeBar => gameObject.GetGameObjectComponent<Slider>("Mainpanel/LifeBar/Bar");
    public Slider StaminaBar => gameObject.GetGameObjectComponent<Slider>("Mainpanel/StaminaBar/Bar");
    public Slider ManaBar => gameObject.GetGameObjectComponent<Slider>("Mainpanel/ManaBar/Bar");
    public Slider KarmaBar => gameObject.GetGameObjectComponent<Slider>("Mainpanel/KarmaBar/Bar");
    public RectTransform KarmaBarRect => gameObject.GetGameObjectComponent<RectTransform>("Mainpanel/KarmaBar/Bar");
    public Image Bar => gameObject.GetGameObjectComponent<Image>("Mainpanel/KarmaBar/Bar/FillArea/Fill");
    public GameObject CardsMain => gameObject.GetGameObject("Mainpanel/Cards");
    public GameObject LoadPanel => gameObject.GetGameObject("Loadpanel");
    public GameObject CardPanel => gameObject.GetGameObject("Cardpanel");
    public GameObject CardPanelExibition => CardPanel.GetGameObject("Subpanel");
    public Slider LoadBar => LoadPanel.GetGameObjectComponent<Slider>("Loadbar");
    public GameObject AttributesPanel => gameObject.GetGameObject("Attributespanel");
    public GameObject AttributesPanelExibition => AttributesPanel.GetGameObject("Subpanel");

    public Button sword;
    public Button staff;

    public Image iconSword;
    public Image iconStaff;

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
        gameObject.DontDestroyOnLoad();
        SetLang();
        sword.onClick.AddListener(() => { espada = false; });
        staff.onClick.AddListener(() => { espada = true; });
        Instantiate(slots.itemPrefab, slots.equipmentSlots[0].transform).Initialize(slots.equipmentSlots[0], swordItem);
        Instantiate(slots.itemPrefab, slots.equipmentSlots[1].transform).Initialize(slots.equipmentSlots[1], staffItem);
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
            LifeBar.maxValue = player.EntityData.maxHealth;
            StaminaBar.maxValue = player.EntityData.maxStamina;
            ManaBar.maxValue = player.EntityData.maxMana;
            KarmaBar.maxValue = player.EntityData.maxKarma;

            LifeBar.value = player.EntityData.currentHealth;
            StaminaBar.value = player.EntityData.currentStamina;
            ManaBar.value = player.EntityData.currentMana;
            KarmaBar.value = Mathf.Abs(player.EntityData.currentKarma);

            KarmaBarRect.rotation = Quaternion.Euler(0, player.EntityData.currentKarma < 0 ? 180 : 0, 0);
            Bar.color = player.EntityData.currentKarma >= 0 ? Color.gray : Color.white;

            if (Input.GetKeyDown(KeyCode.E))
            {
                seeingInventory = !seeingInventory;
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                espada = !espada;
            }

            if (slots.equipmentSlots[0].myItem == null)
            {
                swordItem = null;
                iconSword.color= new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                swordItem = slots.equipmentSlots[0].myItem.myItem;
                iconSword.sprite = swordItem.itemSprite;
                iconSword.color = new Color(1f, 1f, 1f, 1f);
            }

            if (slots.equipmentSlots[1].myItem == null)
            {
                staffItem = null;
                gameManagerInstance.targetObject.SetActive(false);
                iconStaff.color = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                staffItem = slots.equipmentSlots[1].myItem.myItem;
                iconStaff.sprite = staffItem.itemSprite;
                iconStaff.color = new Color(1f, 1f, 1f, 1f);
            }

            //kkkkkkk
            switch (espada)
            {
                case false:
                    player.SetItem(swordItem);
                    break;
                case true:
                    player.SetItem(staffItem);
                    break;
            }
            inventory.SetActive(seeingInventory);
        }
        BlockCards();
        if (player != null)
            player.EntityData.canAttack = !seeingInventory;
    }
    public void OpenAttributesMenu(bool open)
    {
        AttributesPanel.SetActive(open);
        UpdateAllAttributes();
        Time.timeScale = open ? 0f : 1f;
    }
    public void SetActiveInventory() => seeingInventory = !seeingInventory;
    public void UpdateAllAttributes()
    {
        AttributesPanelExibition.GetGameObjectComponent<TextMeshProUGUI>("Strength/Level").text = $"{currentLanguage.level}: {player.EntityData.strength}";
        AttributesPanelExibition.GetGameObjectComponent<TextMeshProUGUI>("Resistance/Level").text = $"{currentLanguage.level}: {player.EntityData.resistance}";
        AttributesPanelExibition.GetGameObjectComponent<TextMeshProUGUI>("Intelligence/Level").text = $"{currentLanguage.level}: {player.EntityData.intelligence}";
        AttributesPanelExibition.GetGameObjectComponent<TextMeshProUGUI>("Defense/Level").text = $"{currentLanguage.level}: {player.EntityData.defense}";
        AttributesPanelExibition.GetGameObjectComponent<TextMeshProUGUI>("Speed/Level").text = $"{currentLanguage.level}: {player.EntityData.speed}";

        AttributesPanel.GetGameObjectComponent<TextMeshProUGUI>("Currentlevel").text = $"{currentLanguage.currentLevel}: {player.EntityData.level}";
        AttributesPanel.GetGameObjectComponent<TextMeshProUGUI>("Points").text = $"{currentLanguage.availablePoints}: {player.disponiblePoints}";
    }
    public void LevelUpAttribute(string name)
    {
        if (player.disponiblePoints > 0)
        {
            switch (name.ToLower())
            {
                case "strength":
                    player.EntityData.strength++;
                    AttributesPanelExibition.GetGameObjectComponent<TextMeshProUGUI>("Strength/Level").text = $"{currentLanguage.level}: {player.EntityData.strength}";
                    break;
                case "resistance":
                    player.EntityData.resistance++;
                    AttributesPanelExibition.GetGameObjectComponent<TextMeshProUGUI>("Resistance/Level").text = $"{currentLanguage.level}: {player.EntityData.resistance}";
                    break;
                case "intelligence":
                    player.EntityData.intelligence++;
                    AttributesPanelExibition.GetGameObjectComponent<TextMeshProUGUI>("Intelligence/Level").text = $"{currentLanguage.level}: {player.EntityData.intelligence}";
                    break;
                case "defense":
                    player.EntityData.defense++;
                    AttributesPanelExibition.GetGameObjectComponent<TextMeshProUGUI>("Defense/Level").text = $"{currentLanguage.level}: {player.EntityData.defense}";
                    break;
                case "speed":
                    player.EntityData.speed++;
                    AttributesPanelExibition.GetGameObjectComponent<TextMeshProUGUI>("Speed/Level").text = $"{currentLanguage.level}: {player.EntityData.speed}";
                    break;
            }
            player.disponiblePoints--;
            AttributesPanel.GetGameObjectComponent<TextMeshProUGUI>("Currentlevel").text = $"{currentLanguage.currentLevel}: {player.EntityData.level}";
            AttributesPanel.GetGameObjectComponent<TextMeshProUGUI>("Points").text = $"{currentLanguage.availablePoints}: {player.disponiblePoints}";
        }
        else
            WarningTextManager.ShowWarning(currentLanguage.cantLevelUpAttribute, 2.5f, 0.2f, WarningTextManager.WarningTextColor);
        player.EntityData.CalculateStatus();
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
    [Obsolete("Precisa de correção.")]
    public async void RandomizeCards()
    {
        cardsAlreadyGotList.Clear();
        //Debug.Log($"{cardsAlreadyGotList.Count}, {CardChoice.habilitiesIDs.Count - 2}, {CardPanelExibition.transform.childCount}");
        if (currentPlayerCards.Count >= CardChoice.habilitiesIDs.Count - 2 && CardPanelExibition.transform.childCount > 1)
        {
            Destroy(CardPanelExibition.transform.GetChild(1).gameObject);
        }
        var cardObjList = new List<CardChoice>();
        for (int i = 0; i < CardPanelExibition.transform.childCount; i++)
        {
            cardObjList.Add(CardPanelExibition.transform.GetChild(i).GetComponent<CardChoice>());
        }
        string strin = "";
        int maxBuffer = 100;
        int currentBuffer = 0;

        //if (cardsAlreadyGotList.Count < CardChoice.habilitiesIDs.Count)
        foreach (CardChoice choice in cardObjList)
        {
        remakeCard: string id = CardChoice.habilitiesIDs[UnityEngine.Random.Range(0, CardChoice.habilitiesIDs.Count)];
            //await Task.Delay(1);
            if (currentPlayerCards.Contains(id) || cardsAlreadyGotList.Contains(id))
            {
                strin += id + " // ";
                Debug.Log(id);
                currentBuffer++;
                if (currentBuffer >= maxBuffer)
                    break;
                goto remakeCard;
            }
            else
            {
                cardsAlreadyGotList.Add(id);
                currentBuffer++;
                if (currentBuffer >= maxBuffer)
                    break;
                choice.SetCard(id);
            }
        }
        Debug.Log(strin);
    }
}
