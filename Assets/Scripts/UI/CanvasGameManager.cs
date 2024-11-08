using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static GamePlayer;
using UnityEngine.UI;
using ItemSystem;
using System;
using static WarningTextManager;
using ObjectUtils;
using static ObjectUtils.GameObjectGeneral;
using HabilitySystem;
using EntityDataSystem;

public class CanvasGameManager : MonoBehaviour
{
    public bool seeingMap = false;
    public static CanvasGameManager canvasInstance;
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

    public List<GameObject> cards = new List<GameObject>();

    private void Start()
    {
        canvasInstance = this;
        sword.onClick.AddListener(() => { player.SetItem(swordItem); });
        staff.onClick.AddListener(() => { player.SetItem(staffItem); });
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
    public GameObject AddCard(HabilityBehaviour hability)
    {
        var card = Instantiate(cardGamePrefab, CardsMain.transform);
        cards.Add(card);
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
        cards.Remove(hability.card);
        Destroy(player.EntityData.habilities[hability.habilityID]);
        player.EntityData.habilities.Remove(hability.habilityID);
    }
}
