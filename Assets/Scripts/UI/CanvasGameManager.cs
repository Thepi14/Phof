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
using static CameraControl;
using System.Threading.Tasks;
using InputManagement;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class CanvasGameManager : MonoBehaviour
{
    public static CanvasGameManager canvasInstance;
    public bool espada = false;
    public bool seeingMap = false, seeingInventory = false, seeingAttributes = false;

    public GameObject InventoryGame => gameObject.GetGameObject("Mainpanel/Inventory");
    public Inventory Slots => gameObject.GetGameObjectComponent<Inventory>("Mainpanel/Inventory");
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
    public GameObject CollectItemSpam => gameObject.GetGameObject("Collectitemspam");
    public GameObject SubOverlay => gameObject.GetGameObject("Suboverlay");
    public GameObject RoomCompletionText => SubOverlay.GetGameObject("Roomcompletiontext");
    public GameObject Death => SubOverlay.GetGameObject("Death");

    public Button sword;
    public Button staff;

    public Image iconSword;
    public Image iconStaff;

    public ItemSystem.Item staffItem;
    public ItemSystem.Item swordItem;
    public ItemSystem.Item bowItem;

    public GameObject cardPrefab;
    public GameObject cardGamePrefab;

    public List<GameObject> currentCards = new();
    public List<string> cardsAlreadyGotList = new();

    public List<string> CurrentPlayerCards
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

        if (PlayerPreferences.NewGame)
        {
            switch (PlayerPrefs.GetString("CLASS", "Warrior"))
            {
                case "Warrior":
                    Instantiate(Slots.itemPrefab, Slots.equipmentSlots[0].transform).Initialize(Slots.equipmentSlots[0], swordItem);
                    break;
                case "Wizard":
                    Instantiate(Slots.itemPrefab, Slots.equipmentSlots[1].transform).Initialize(Slots.equipmentSlots[1], staffItem);
                    break;
                case "Archer":
                    Instantiate(Slots.itemPrefab, Slots.equipmentSlots[1].transform).Initialize(Slots.equipmentSlots[1], bowItem);
                    break;
            }
        }

        Death.GetGameObjectComponent<Button>("Exitbutton").onClick.AddListener(delegate
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

        StartCoroutine(Pauser());
    }
    public void SetLang()
    {
        LoadPanel.GetGameObjectComponent<TextMeshProUGUI>("Title").text = currentLanguage.loading;
        AttributesPanel.GetGameObjectComponent<TextMeshProUGUI>("Title").text = currentLanguage.chooseAttributeToLevelUp;
        AttributesPanel.GetGameObjectComponent<TextMeshProUGUI>("Subpanel\\Strength\\Name").text = currentLanguage.strength;
        AttributesPanel.GetGameObjectComponent<TextMeshProUGUI>("Subpanel\\Resistance\\Name").text = currentLanguage.resistance;
        AttributesPanel.GetGameObjectComponent<TextMeshProUGUI>("Subpanel\\Intelligence\\Name").text = currentLanguage.intelligence;
        AttributesPanel.GetGameObjectComponent<TextMeshProUGUI>("Subpanel\\Defense\\Name").text = currentLanguage.defense;
        AttributesPanel.GetGameObjectComponent<TextMeshProUGUI>("Subpanel\\Speed\\Name").text = currentLanguage.speed;
        AttributesPanel.GetGameObjectComponent<TextMeshProUGUI>("Exitbutton\\Text").text = currentLanguage.exit;
        Death.GetGameObjectComponent<TextMeshProUGUI>("Exitbutton\\Text").text = currentLanguage.exit;
        Death.GetGameObjectComponent<TextMeshProUGUI>("Title").text = currentLanguage.youReDead;
        SetCollectLang();
    }
    public static void SetCollectLang()
    {
        canvasInstance.CollectItemSpam.GetGameObjectComponent<TextMeshProUGUI>("Text").text = currentLanguage.pressToCollectItem.Replace("\\", InputManager.Instance.GetKeyCodeByKeyBind(KeyBindKey.Collect).ToString());
    }
    public static void PlayRoomCompletionAnimation(int karma)
    {
        canvasInstance.RoomCompletionText.GetComponent<Animator>().Play("Play");
        canvasInstance.RoomCompletionText.GetComponent<TextMeshProUGUI>().text = currentLanguage.roomCompleted;
        canvasInstance.RoomCompletionText.GetGameObjectComponent<TextMeshProUGUI>("Karmaadded").text = $"{currentLanguage.karmaEarned}: {karma}";
    }
    public static void PlayDeathScene()
    {
        canvasInstance.Death.SetActive(true);
        canvasInstance.Death.GetComponent<Animator>().Play("Play");
    }
    private void Update()
    {
        if (GamePaused || OptionsPanel.keyBinding)
            return;
        LoadPanel.SetActive(!TerrainGeneration.Instance.mapLoaded);

        if (!TerrainGeneration.Instance.mapLoaded)
        {
            LoadBar.value = TerrainGeneration.Instance.GenerationProgress;
        }
        if (player != null)
        {
            if (player.EntityData.dead)
                return;
            LifeBar.maxValue = player.EntityData.maxHealth;
            StaminaBar.maxValue = player.EntityData.maxStamina;
            ManaBar.maxValue = player.EntityData.maxMana;
            KarmaBar.maxValue = player.EntityData.maxKarma;

            LifeBar.value = player.EntityData.currentHealth;
            StaminaBar.value = player.EntityData.currentStamina;
            ManaBar.value = player.EntityData.currentMana;
            KarmaBar.value = Mathf.Abs(player.EntityData.currentKarma);

            //KarmaBarRect.rotation = Quaternion.Euler(0, player.EntityData.currentKarma < 0 ? 180 : 0, 0);
            //Bar.color = player.EntityData.currentKarma >= 0 ? Color.gray : Color.white;
            Bar.color = Color.cyan;

            if (InputManager.GetKeyDown(KeyBindKey.Inventory) && !seeingAttributes)
            {
                seeingInventory = !seeingInventory;
                if (seeingInventory)
                {
                    seeingMap = false;
                }
                else
                {
                    if (Slots.draggablesTransform.childCount > 0)
                    {
                        if(Slots.draggablesTransform.transform.GetComponentInChildren<InventoryItem>().activeSlot.myItem == (Slots.draggablesTransform.transform.GetComponentInChildren<InventoryItem>()))
                            Slots.draggablesTransform.transform.GetComponentInChildren<InventoryItem>().activeSlot.SetItem(Inventory.carriedItem);
                        else
                        {
                            for (int i = 0; i < Slots.inventorySlots.Length; i++)
                            {
                                if (Slots.inventorySlots[i].myItem == null)
                                {
                                    Slots.inventorySlots[i].SetItem(Inventory.carriedItem);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if (InputManager.GetKeyDown(KeyBindKey.ItemHotbar))
            {
                espada = !espada;
            }
            else if (!seeingAttributes && InputManager.GetKeyDown(KeyBindKey.OpenMap))
            {
                seeingMap = !seeingMap;
                if (seeingMap)
                {
                    seeingInventory = false;
                }
            }

            if (Slots.equipmentSlots[0].myItem == null)
            {
                swordItem = null;
                iconSword.color= new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                swordItem = Slots.equipmentSlots[0].myItem.myItem;
                iconSword.sprite = swordItem.itemSprite;
                iconSword.color = new Color(1f, 1f, 1f, 1f);
            }

            if (Slots.equipmentSlots[1].myItem == null)
            {
                staffItem = null;
                gameManagerInstance.targetObject.SetActive(false);
                iconStaff.color = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                staffItem = Slots.equipmentSlots[1].myItem.myItem;
                iconStaff.sprite = staffItem.itemSprite;
                iconStaff.color = new Color(1f, 1f, 1f, 1f);
            }

            //Item collectment
            Ray ray = MainCameraControl.cam.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, Vector3.Distance(MainCameraControl.gameObject.transform.position, player.transform.position) + 250f, LayerMask.GetMask("Item"), QueryTriggerInteraction.Collide);

            if (hit.collider != null && !seeingMap && !seeingInventory && !seeingAttributes && Vector3.Distance(hit.point, player.transform.position) < 1.25f)
            {
                CollectItemSpam.SetActive(true);
                CollectItemSpam.transform.position = Input.mousePosition;
                if (InputManager.GetKeyDown(KeyBindKey.Collect))
                {
                    var drop = hit.collider.GetComponent<ItemDrop>().item;
                    Debug.Log(drop == null);
                    var collected = global::Inventory.Singleton.SpawnInventoryItem(drop);
                    if (collected)
                    {
                        Destroy(hit.collider.gameObject);
                    }
                    else
                    {
                        WarningTextManager.ShowWarning(currentLanguage.inventoryIsFull, 2f, 0.5f);
                    }
                }
            }
            else
            {
                CollectItemSpam.SetActive(false);
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
            InventoryGame.SetActive(seeingInventory);
        }
        BlockCards();
        if (player != null)
            player.EntityData.canAttack = !seeingInventory;
    }
    public IEnumerator Pauser()
    {
        while (true)
        {
            yield return new WaitForNextFrameUnit();
            if (InputManager.GetKeyDown(KeyBindKey.Pause))
            {
                TickPause();
            }
        }
    }
    public void OpenAttributesMenu(bool open)
    {
        AttributesPanel.SetActive(open);
        UpdateAllAttributes();
        seeingAttributes = open;
        PauseGame(open);
    }
    public bool GamePaused { get; private set; } = false;
    public void PauseGame(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        GamePaused = pause;
    }
    public void TickPause() => PauseGame(!GamePaused);
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
                    player.EntityData.speed += 0.2f;
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
    public void RandomizeCards()
    {
        cardsAlreadyGotList.Clear();
        //Debug.Log($"{cardsAlreadyGotList.Count}, {CardChoice.habilitiesIDs.Count - 2}, {CardPanelExibition.transform.childCount}");
        if (CurrentPlayerCards.Count >= CardChoice.habilitiesIDs.Count - 2 && CardPanelExibition.transform.childCount > 1)
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
            if (CurrentPlayerCards.Contains(id) || cardsAlreadyGotList.Contains(id))
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
