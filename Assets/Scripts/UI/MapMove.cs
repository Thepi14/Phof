using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TerrainGeneration;
using static CanvasGameManager;

public class MapMove : MonoBehaviour
{
    public Vector2 playerPosition;
    public Vector2 crossPosition;
    public GameObject player;
    public GameObject map;
    public GameObject blurMap;
    public GameObject mapCross;
    private Vector2 moveVision;

    void Start()
    {
        playerPosition = player.transform.position;
        map = transform.Find("Map").gameObject;
        blurMap = transform.Find("BlurMap").gameObject;
        mapCross = transform.Find("Cross").gameObject;
    }
    void LateUpdate()
    {
        playerPosition = player.transform.position;
        if (!canvasInstance.seeingMap)
        {
            mapCross.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            mapCross.GetComponent<RectTransform>().localScale = Vector3.one * 0.5f;
            moveVision = Vector2.zero;

            map.GetComponent<RectTransform>().pivot = (new Vector2(playerPosition.x / Instance.MapWidth, playerPosition.y / Instance.MapHeight));
            blurMap.GetComponent<RectTransform>().pivot = (new Vector2(playerPosition.x / Instance.MapWidth, playerPosition.y / Instance.MapHeight));
            mapCross.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

            map.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            blurMap.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            mapCross.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        else
        {
            mapCross.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
            mapCross.GetComponent<RectTransform>().localScale = new Vector3(0.1f, 0.1f, 1);
            moveVision += new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime * 40;

            map.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            blurMap.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            mapCross.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

            map.GetComponent<RectTransform>().anchoredPosition = -moveVision;
            blurMap.GetComponent<RectTransform>().anchoredPosition = -moveVision;
            mapCross.GetComponent<RectTransform>().anchoredPosition = new Vector2((((playerPosition.x * map.GetComponent<RectTransform>().rect.width) / Instance.MapWidth) - map.GetComponent<RectTransform>().rect.width / 2) - moveVision.x, (((playerPosition.y * map.GetComponent<RectTransform>().rect.height) / Instance.MapHeight) - map.GetComponent<RectTransform>().rect.height / 2) - moveVision.y);
        }
        crossPosition = mapCross.GetComponent<RectTransform>().anchoredPosition;
    }
    private void OnMouseOver()
    {
        
    }
}
