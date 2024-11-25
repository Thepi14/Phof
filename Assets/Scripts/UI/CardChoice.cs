using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HabilitySystem;
using static GamePlayer;
using Unity.VisualScripting;
using UnityEngine.UI;
using static CanvasGameManager;
using UnityEditor;
using ObjectUtils;
using TMPro;
using LangSystem;

public class CardChoice : MonoBehaviour
{
    public TextMeshProUGUI Title => GameObjectGeneral.GetGameObjectComponent<TextMeshProUGUI>(gameObject, "Name");
    public TextMeshProUGUI Description => GameObjectGeneral.GetGameObjectComponent<TextMeshProUGUI>(gameObject, "Desc");
    public Image Image => GameObjectGeneral.GetGameObjectComponent<Image>(gameObject, "Image");

    public static List<string> habilitiesIDs = new()
    {
        "Slash",
        "GroundSlash",
        "Berserk",
    };
    public static void AddCard(string cardID)
    {
        if (!habilitiesIDs.Contains(cardID))
            throw new System.Exception($"Hability of name: {cardID} doesn't exist, put it's name on the list or it's not a hability.");
        switch (cardID)
        {
            case "Slash":
                player.AddComponent<SlashHability>();
                break;
            case "GroundSlash":
                player.AddComponent<GroundSlashHability>();
                break;
            case "Berserk":
                player.AddComponent<BerserkHability>();
                break;
            case "FireTornado":

                break;
        }
    }
    public void SetCard(string cardID)
    {
        if (gameObject == null)
            return;
        gameObject.name = cardID;
        if (habilitiesIDs.Contains(cardID))
        {
            Title.text = Language.currentLanguage.habilityInfos[cardID].name;
            Description.text = Language.currentLanguage.habilityInfos[cardID].description;
        }
        else
            Debug.LogError($"There is no ID called \"{cardID}\" in the current Language {Language.currentLanguage.name}.");
        if (Resources.Load<Sprite>("CardSprites/" + cardID) != null)
            Image.sprite = Resources.Load<Sprite>("CardSprites/" + cardID);
        else
            Debug.LogError($"There is no Sprite to the hability called \"{cardID}\" in Assets/Resources/CardSprites/.");

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => { AddCard(gameObject.name); canvasInstance.OpenMenu(GameMenu.MainPanel); });
    }
}
