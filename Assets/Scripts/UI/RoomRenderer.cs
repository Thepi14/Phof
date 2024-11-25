using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using ObjectUtils;
using static ObjectUtils.MathEx;
using EntityDataSystem;
using UnityEditor.UI;

public class RoomRenderer : MonoBehaviour
{
    public GameObject cameraObj;
    public GameObject blackBackGroundObj;
    public Biome biome;
    private RoomInfo info;
    public List<GameObject> blocks = new List<GameObject>();
    private float currentTimer, timer = 15f;
    public Vector3 roomCenterPivot;
    public float cameraRotationVelocity = 3f;
    private float cameraRotation;
    public float cameraAngle = 45f;

    public void Start()
    {
        currentTimer = 0;
        Generate();
    }
    public void Update()
    {
        currentTimer += Time.deltaTime;
        cameraRotation += Time.deltaTime * cameraRotationVelocity;
        if (cameraRotation >= 180f)
            cameraRotation = 0;
        cameraObj.transform.rotation = Quaternion.Euler(cameraAngle, cameraRotation, 0);
        if (currentTimer >= timer - 3f)
        {
            if (blackBackGroundObj.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name != "FadeInOutBackground")
                blackBackGroundObj.GetComponent<Animator>().Play("FadeInOutBackground");
        }
        if (currentTimer >= timer)
        {
            Generate();
        }

        roomCenterPivot = ((Vector3.one - Vector3.up) * (info.size / 2)) + (Vector3.up * 5);
        cameraObj.transform.position = roomCenterPivot;
    }
    public void Generate()
    {
        currentTimer = 0;
        var list = biome.rooms;
    remade: list.Shuffle();
        if (info != list[0])
            info = list[0];
        else
            goto remade;
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
            if (block.id == 0)
                continue;
            PlaceBlock(biome.generalBlocks[block.id - 1], block.x, block.y, 1f + block.height, block.rotation, block.scale - Vector3.one);
        }
        bool detectCorner(int x, int y) => (x == -1 && y == -1) || (x == -1 && y == info.size) || (x == info.size && y == -1) || (x == info.size && y == info.size);
        bool detectWall(int x, int y) => (x == -1) || (y == info.size) || (y == -1) || (x == info.size);
    }
    public GameObject PlaceBlock(BlockClass blockClass, float x, float y, float z, Vector3? rotation = null, Vector3? scale = null)
    {
        rotation = rotation == null ? rotation = Vector3.zero : (Vector3)rotation;
        scale = scale == null ? scale = Vector3.zero : (Vector3)scale;

        if (blockClass.blockPrefab == null)
            throw new System.Exception("Block is null!");
        var obj = Instantiate(blockClass.blockPrefab, transform, true);
        if (obj.GetComponent<BlockEntity>() != null)
        {
            var it = obj.GetComponent<BlockEntity>();
            Destroy(it as Object);
        }
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
        GameObject[] objects = gameObject.GetGameObjectChildren();
        foreach (var block in objects)
        {
            Destroy(block);
            if (blocks.Contains(block))
                blocks.Remove(block);
        }
    }
}
