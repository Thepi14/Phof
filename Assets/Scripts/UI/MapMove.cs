using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TerrainGeneration;
using static CanvasGameManager;
using static GamePlayer;
using ObjectUtils;
using InputManagement;

public class MapMove : MonoBehaviour
{
    public static MapMove MapMoveInstance;
    public Vector2 playerPosition;
    public Vector2 crossPosition;
    public Image map => gameObject.GetGameObjectComponent<Image>("Map");
    public Image mapCross => gameObject.GetGameObjectComponent<Image>("Cross");
    private Vector2 moveVision;

    public void Awake()
    {
        if (MapMoveInstance == null)
            MapMoveInstance = this;
        else
            Destroy(gameObject);
    }
    private void LateUpdate()
    {
        if (player == null || TerrainGeneration.Instance == null || MapMoveInstance == null)
            return;
        playerPosition = new Vector2(player.transform.position.x, player.transform.position.z);
        if (!canvasInstance.seeingMap)
        {
            GetComponent<Mask>().enabled = true;
            mapCross.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            mapCross.GetComponent<RectTransform>().localScale = Vector3.one * 0.5f;
            moveVision = Vector2.zero;

            GetComponent<RectTransform>().sizeDelta = new Vector2(240f, 240f);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(802, 383);

            map.GetComponent<RectTransform>().pivot = (new Vector2(playerPosition.x / Instance.MapWidth, playerPosition.y / Instance.MapHeight));
            mapCross.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

            map.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            mapCross.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        else
        {
            GetComponent<Mask>().enabled = false;
            mapCross.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
            mapCross.GetComponent<RectTransform>().localScale = new Vector3(0.1f, 0.1f, 1);
            moveVision += InputManager.GetAxis() * Time.deltaTime * 40;

            GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            map.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            mapCross.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

            map.GetComponent<RectTransform>().anchoredPosition = -moveVision;
            mapCross.GetComponent<RectTransform>().anchoredPosition = new Vector2((((playerPosition.x * map.GetComponent<RectTransform>().rect.width) / Instance.MapWidth) - map.GetComponent<RectTransform>().rect.width / 2) - moveVision.x, (((playerPosition.y * map.GetComponent<RectTransform>().rect.height) / Instance.MapHeight) - map.GetComponent<RectTransform>().rect.height / 2) - moveVision.y);
        }
        crossPosition = mapCross.GetComponent<RectTransform>().anchoredPosition;
    }
    private void OnMouseOver()
    {
        
    }
}
