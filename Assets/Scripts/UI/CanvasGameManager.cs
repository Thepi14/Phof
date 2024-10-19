using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ObjectUtils;
using static GameManagerSystem.GameManager;
using static GamePlayer;
using UnityEngine.UI;
using ItemSystem;

public class CanvasGameManager : MonoBehaviour
{
    public RectTransform LifeBar => GameObjectGeneral.GetGameObjectComponent<RectTransform>(gameObject, "MainPanel/LifeBar/Bar");
    public RectTransform StaminaBar => GameObjectGeneral.GetGameObjectComponent<RectTransform>(gameObject, "MainPanel/StaminaBar/Bar");
    public RectTransform ManaBar => GameObjectGeneral.GetGameObjectComponent<RectTransform>(gameObject, "MainPanel/ManaBar/Bar");

    public Button sword;
    public Button staff;

    public Item swordItem;
    public Item staffItem;

    private void Start()
    {
        sword.onClick.AddListener(() => { player.SetItem(swordItem); });
        staff.onClick.AddListener(() => { player.SetItem(staffItem); });
    }
    private void Update()
    {
        if (player == null)
            return;
        LifeBar.localScale = new Vector3(Mathf.Max((float)player.EntityData.currentHealth / player.EntityData.maxHealth, 0), 1, 1);
        StaminaBar.localScale = new Vector3(Mathf.Max((float)player.EntityData.currentStamina / player.EntityData.maxStamina, 0), 1, 1);
        ManaBar.localScale = new Vector3(Mathf.Max((float)player.EntityData.currentMana / player.EntityData.maxMana, 0), 1, 1);
    }
}
