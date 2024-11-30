// --------------------------------------------------------------------------------------------------------------------
/// <copyright file="RoomViewer.cs">
///   Copyright (c) 2024, Pi14, All rights reserved.
/// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using static RoomInfo;

public class RoomViewer : MonoBehaviour
{
    [HelpAttribute("Ao mudar essas variáveis o mapa vai atualizar automáticamente, se nada der errado. Os erros irão aparecer no console mesmo.", order = -1)]
    [Header("Configuration", order = 0)]
    public Biome biome;
    private Biome previousBiome;
    public RoomInfo info;
    private RoomInfo previousInfo;
    [SerializeField]
    private Grid<BlockInfo> previousGrid;
    private bool variableChanged = false;
    [Header("Interface", order = 1)]
    public GameObject spawnTilePrefab;
    public GameObject coordPrefab;
    public Material red;
    public Material green;
    public GameObject Camera => GameObject.Find("Main Camera");
    [Header("Status", order = 2)]
    public List<GameObject> blocks = new List<GameObject>();
    public bool showSpawnTiles = false;
    public bool showCoordTiles = false;
    [HelpAttribute("Aqui você clica nas booleanas para modificar diretamente os blocos do mapa caso algo dê errado.", order = 3)]
    [Header("Room regen", order = 4)]
    public bool create = false;
    [HideInInspector]
    public bool destroy = false;
    #region Teto
    [HelpAttribute(
        "近頃噂のあの漫画\r\n" +
        "みんなはずっと話してる\r\n" +
        "それそれめっちゃ良かったよネ\r\n" +
        "見たことないけど\r\n" +
        "このあと　バレた\r\n" +
        "譲るのが面倒で道変えた\r\n" +
        "最初から行き先違うフリをして\r\n" +
        "それでもゴールは変わらないのなら\r\n" +
        "きっとそうやって生きてもいいの\r\n" +
        "嘘が先か 真が先かなんてさ\r\n" +
        "いつか来るその日を前にはどちらも\r\n" +
        "変わらない\r\n" +
        "踊れ 踊れ 嘘に踊れ\r\n" +
        "今までを捨てて 腕を振れよ\r\n" +
        "中身がなんもなくても\r\n" +
        "未来はあるのさ\r\n" +
        "ライアー ライアー ダンサー\r\n" +
        "素直で傷ついたあの日を\r\n" +
        "ライアー ライアー ダンサー\r\n" +
        "嘘で踊るのさ\r\n", 
        order = 4)]

    public bool TetoKasaneLiarDance = false;
    #endregion

    public void OnValidate()
    {
        runInEditMode = true;
        variableChanged = true;
    }
    public void Start()
    {
        TetoKasaneLiarDance = false;
        ClearBlocks();
    }
    public void Update()
    {
        runInEditMode = true;
        #region Kasane
        if (TetoKasaneLiarDance)
        {
            TetoKasaneLiarDance = false;
            ClearBlocks();
            EditorApplication.Exit(404);
        }
        #endregion
        if (Input.GetKeyDown(KeyCode.C) || create || info.changed || biome != previousBiome || variableChanged)
        {
            if (biome == null)
                throw new System.Exception("There's no biome!");
            else if (info == null)
                throw new System.Exception("There's no room info!");
            /*else if (previousGrid.IsNull())
            {
                Debug.Log("previous grid changed");
                previousGrid.ListToGrid(info.grid.GridToList(), (x, y, i) => { BlockInfo newBlock = new BlockInfo(x, y); return newBlock.CopyID(info.grid.GetGridObject(x, y)); }, info.size.x);
            }*/

            ClearBlocks();

            for (int x = -1; x <= info.size; x++)
            {
                for (int y = -1; y <= info.size; y++)
                {
                    if (detectCorner(x, y))
                    {
                        PlaceBlock(biome.pillarBlocks[0], x, y, 1.5f);
                    }
                    else if (detectWall(x, y))
                    {
                        if (x == -1)
                            PlaceBlock(biome.wallBlocks[0], x, y, 1.5f, new Vector3(0, 0, 90));
                        else if (x == info.size)
                            PlaceBlock(biome.wallBlocks[0], x, y, 1.5f, new Vector3(0, 0, -90));
                        else
                            PlaceBlock(biome.wallBlocks[0], x, y, 1.5f);
                    }
                    PlaceBlock(biome.groundBlocks[0], x, y, 0.5f);
                }
            }
            foreach (var block in info.RetrieveList().ToList())
            {
                if (coordPrefab != null && showCoordTiles)
                {
                    var tile = Instantiate(coordPrefab, new Vector3(block.x, 0.516f, block.y), Quaternion.Euler(90, 0, 0), transform);
                    tile.GetComponent<TextMeshPro>().text = $"x: {block.x}\ny: {block.y}";
                }
                if (block.id > biome.generalBlocks.Count)
                    continue;
                /*if (previousGrid.GetGridObject(block.x, block.y).id != block.id)
                {
                    if (block.id == 0)
                    {
                        info.grid.SetGridObject(block.x, block.y, new BlockInfo(block.x, block.y){x = block.x, y = block.y, id = block.id, changed = false, entityCanSpawn = true, height = block.height, name = block.name, rotation = block.rotation, scale = block.scale});
                        Debug.Log($"> {info.grid.GetGridObject(block.x, block.y).entityCanSpawn}");
                    }
                    else if (block.id != 0 && biome.generalBlocks[block.id - 1].blockPrefab.GetComponent<Collider>() != null)
                    {
                        info.grid.SetGridObject(block.x, block.y, new BlockInfo(block.x, block.y) { x = block.x, y = block.y, id = block.id, changed = false, entityCanSpawn = false, height = block.height, name = block.name, rotation = block.rotation, scale = block.scale });
                        Debug.Log($"> {info.grid.GetGridObject(block.x, block.y).entityCanSpawn}");
                    }
                }*/
                if (spawnTilePrefab != null && showSpawnTiles)
                {
                    var tile = Instantiate(spawnTilePrefab, new Vector3(block.x, 5f, block.y), Quaternion.identity, transform);
                    if (block.entityCanSpawn)
                    {
                        tile.GetComponent<MeshRenderer>().material = green;
                    }
                    else
                    {
                        tile.GetComponent<MeshRenderer>().material = red;
                    }
                }
                if (block.id == 0)
                    continue;
                PlaceBlock(biome.generalBlocks[block.id - 1], block.x, block.y, 1f + block.height, block.rotation, block.scale - Vector3.one);
            }
        }
        else if (Input.GetKeyDown(KeyCode.D) || destroy)
        {
            ClearBlocks();
        }

        info.changed = false;
        create = false;
        destroy = false;

        variableChanged = false;
        previousInfo = info;
        previousGrid.ListToGrid(info.grid.GridToList(), (x, y, i) => { BlockInfo newBlock = new BlockInfo(x, y); return newBlock.CopyID(info.grid.GetGridObject(x, y)); }, info.size);
        previousBiome = biome;

        bool detectCorner(int x, int y) => (x == -1 && y == -1) || (x == -1 && y == info.size) || (x == info.size && y == -1) || (x == info.size && y == info.size);
        bool detectWall(int x, int y) => (x == -1) || (y == info.size) || (y == -1) || (x == info.size);

        Camera.transform.position = new Vector3((info.size / 2f) - 0.5f, Camera.transform.position.y, Camera.transform.position.z);
    }
    public GameObject PlaceBlock(BlockClass blockClass, float x, float y, float z, Vector3? rotation = null, Vector3? scale = null)
    {
        rotation = rotation == null ? rotation = Vector3.zero : (Vector3)rotation;
        scale = scale == null ? scale = Vector3.zero : (Vector3)scale;

        if (blockClass.blockPrefab == null)
            throw new System.Exception("Block is null!");
        var obj = Instantiate(blockClass.blockPrefab, transform, true);
        obj.name = x + " " + y;
        obj.transform.parent = transform;
        obj.transform.position = new Vector3(x, z, y);
        obj.transform.localScale = blockClass.blockPrefab.transform.localScale + scale.Value;
        obj.transform.rotation = Quaternion.Euler(blockClass.blockPrefab.transform.rotation.eulerAngles + rotation.Value);
        blocks.Add(obj);

        return obj;
    }
    public void ClearBlocks()
    {
        List<GameObject> objects = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
            objects.Add(transform.GetChild(i).gameObject);
        foreach (var block in objects)
        {
            DestroyImmediate(block);
            if (blocks.Contains(block))
                blocks.Remove(block);
        }
    }
}
#endif
