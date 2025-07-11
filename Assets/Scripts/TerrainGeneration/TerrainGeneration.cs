// --------------------------------------------------------------------------------------------------------------------
/// <copyright file="TerrainGeneration.cs">
///   Copyright (c) 2024, Pi14, All rights reserved.
/// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CanvasGameManager;
using static TextureFunction;
using GameManagerSystem;
using Pathfindingsystem;
using RoomSystem;
using static NavMeshUpdate;
using System.Threading.Tasks;
using EntityDataSystem;
using UnityEditor;
using static Unity.Collections.AllocatorManager;
using B83.MeshHelper;

public class TerrainGeneration : MonoBehaviour
{
    public static TerrainGeneration Instance;
    [SerializeField] private GameObject wallParent, floorParent;

    public Biome biome;

    public List<RoomNode> rooms;

    public Texture2D noiseMap;
    public Texture2D dotMap;
    public Texture2D physicalMap;
    public Texture2D roomsMap;
    public Texture2D roomDetectionMap;
    public Texture2D corridorMap;
    public Texture2D torchMap;

    public Mesh DEFAULT_MESH;
    public Material DEFAULT_MATERIAL;

    public int seed; public float frequency, limit, scattering;
    public ushort torchOffset = 3;
    public int torchMinDistance = 1;

    public int minRoomSize = 3;
    public int maxRoomSize = 6;

    #region Vari�veis pr� definidas e constantes
    public Pathfinding pathfinding;

    [SerializeField]
    private int _mapWidth, _mapHeight;
    public int MapWidth
    {
        get { return _mapWidth; }
        private set { _mapWidth = value; }
    }
    public int MapHeight
    {
        get { return _mapHeight; }
        private set { _mapHeight = value; }
    }
    private int StageOffSet => PlayerPrefs.GetInt("STAGE_OFFSET", PlayerPrefs.GetInt("MAP_WIDTH", 100));
    private int CurrentStage => PlayerPrefs.GetInt("CURRENT_STAGE", 1) - 1;

    public int corridorSize = 1;
    public int minimumDistanceBetweenRooms = 12;

    private const int DEFAULT_SEED = 24556;
    public const float DEFAULT_GROUND_HEIGHT = 0.5f;
    #endregion

    public Grid<GameObject> walls;
    public Grid<GameObject> floors;
    public Grid<bool> spawnTiles;
    public Grid<int> roomGrid;

    public GameObject corridorMesh;
    public GameObject corridorFloorMesh;

    [SerializeField]
    private bool configSeed = false;
    public bool mapLoaded = false;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Instance = this;

        //não se confunda, M'cos...
        if (!configSeed)
        {
            seed = PlayerPrefs.GetInt("CURRENT_SEED", DEFAULT_SEED);
            MapWidth = PlayerPrefs.GetInt("MAP_WIDTH", 100);
            MapHeight = PlayerPrefs.GetInt("MAP_HEIGHT", 100);
        }

        walls = new Grid<GameObject>(MapWidth, MapHeight, (grid, x, y) => { return null; });
        floors = new Grid<GameObject>(MapWidth, MapHeight, (grid, x, y) => { return null; });
        spawnTiles = new Grid<bool>(MapWidth, MapHeight, (spawntiles, x, y) => { return false; });
        roomGrid = new Grid<int>(MapWidth, MapHeight, (grid, x, y) => { return -1; });

        noiseMap = new Texture2D(MapWidth, MapHeight);
        dotMap = new Texture2D(MapWidth, MapHeight);
        physicalMap = new Texture2D(MapWidth, MapHeight);
        roomDetectionMap = new Texture2D(MapWidth, MapHeight);
        torchMap = new Texture2D(MapWidth, MapHeight);

        pathfinding = new Pathfinding(MapWidth, MapHeight);

        rooms = new List<RoomNode>();

        Random.InitState(seed);

        GenerateLevel();
    }
    public async void GenerateLevel()
    {
        await _GenerateLevel();
    }
    public float GenerationProgress { get; private set; } = 0;

    public void RoomOcclusion(int roomIndex)
    {
        if (roomIndex > -1)
        {
            corridorMesh.SetActive(false);
            corridorFloorMesh.SetActive(false);
            for (int x = 0; x < MapWidth; x++)
            {
                for (int z = 0; z < MapHeight; z++)
                {
                    if (roomGrid.GetGridObject(x, z) == roomIndex)
                        continue;
                    if (GetWallObj(x, z) != null)
                        GetWallObj(x, z).SetActive(false);
                    /*if (GetFloorObj(x, z) != null)
                        GetFloorObj(x, z).SetActive(false);*/
                }
            }
        }
        else
        {
            corridorMesh.SetActive(true);
            corridorFloorMesh.SetActive(true);
            for (int x = 0; x < MapWidth; x++)
            {
                for (int z = 0; z < MapHeight; z++)
                {
                    if (GetWallObj(x, z) != null)
                        GetWallObj(x, z).SetActive(true);
                    /*if (GetFloorObj(x, z) != null)
                        GetFloorObj(x, z).SetActive(true);*/
                }
            }
        }

        foreach (var room in rooms)
        {
            if (roomIndex > -1)
                room.SetRoomActive(room.id == roomIndex);
            else
                room.SetRoomActive(true);
        }

        BuildNavMesh();
    }
    public void OnValidate()
    {
        noiseMap = new Texture2D(MapWidth, MapHeight);
        noiseMap = GenerateNoiseTexture(MapWidth, MapHeight, seed, frequency, limit, scattering, true, StageOffSet * CurrentStage, StageOffSet * CurrentStage);
    }
    private void GenerateCorridorMesh()
    {
        List<CombineInstance> combine = new List<CombineInstance>();
        //boring
        List<GameObject> allBlocks = new List<GameObject>();
        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                if (roomGrid.GetGridObject(x, z) != -1)
                    continue;
                if (GetWallObj(x, z) != null)
                {
                    allBlocks.Add(GetWallObj(x, z));
                }
            }
        }

        foreach (var block in allBlocks)
        {
            if (block == null) continue;
            if (block.GetComponent<BlockEntity>() != null || block.GetComponent<Animator>() != null) continue;

            MeshFilter filter = block.GetComponent<MeshFilter>();
            if (filter == null) continue;

            var b = new CombineInstance();
            b.mesh = filter.sharedMesh;
            b.transform = block.transform.localToWorldMatrix;
            combine.Add(b);
            //block.SetActive(false);
        }

        corridorMesh = new GameObject("CorridorMesh");
        corridorMesh.transform.parent = transform;
        corridorMesh.transform.position = Vector3.zero;
        corridorMesh.layer = 6;

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine.ToArray());
        var welder = new MeshWelder(mesh);
        welder.mergeWithoutCheck = true;
        welder.Weld();
        corridorMesh.AddComponent<MeshFilter>().sharedMesh = welder.GetMesh();
        corridorMesh.AddComponent<MeshCollider>();
        //roomMeshChild.AddComponent<MeshRenderer>();
        corridorMesh.gameObject.SetActive(true);

        //floors
        allBlocks.Clear();
        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                if (roomGrid.GetGridObject(x, z) != -1)
                    continue;
                if (GetFloorObj(x, z) != null)
                {
                    allBlocks.Add(GetFloorObj(x, z));
                }
            }
        }

        combine.Clear();
        foreach (var floor in allBlocks)
        {
            if (floor == null) continue;
            if (floor.GetComponent<BlockEntity>() != null || floor.GetComponent<Animator>() != null) continue;

            MeshFilter filter = floor.GetComponent<MeshFilter>();
            if (filter == null) continue;

            var b = new CombineInstance();
            b.mesh = filter.sharedMesh;
            b.transform = floor.transform.localToWorldMatrix;
            combine.Add(b);
            floor.SetActive(false);
        }
        corridorFloorMesh = new GameObject("CorridorFloorMesh");
        corridorFloorMesh.transform.parent = transform;
        corridorFloorMesh.transform.position = Vector3.zero;
        corridorFloorMesh.layer = 7;

        mesh = new Mesh();
        mesh.CombineMeshes(combine.ToArray());
        welder = new MeshWelder(mesh);
        welder.Weld();

        corridorFloorMesh.AddComponent<MeshFilter>().sharedMesh = welder.GetMesh();
        //MASSIVE HACK
        corridorFloorMesh.AddComponent<MeshRenderer>().material = biome.groundBlocks[0].blockPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        corridorFloorMesh.gameObject.SetActive(true);
    }
    private async Task _GenerateLevel()
    {
        if (GamePlayer.player != null)
            GamePlayer.player.transform.position = new Vector3(0, 1f, 0);

        mapLoaded = false;
        GenerationProgress = 0;

        noiseMap = GenerateNoiseTexture(MapWidth, MapHeight, seed, frequency, limit, scattering, true, StageOffSet * CurrentStage, StageOffSet * CurrentStage);

        dotMap = SeparateWhiteDots(noiseMap, minimumDistanceBetweenRooms, maxRoomSize + 4);

        roomsMap = ExpandWhiteDotsRandomly(dotMap, minRoomSize, maxRoomSize, 3);

        roomDetectionMap = GenerateCrossesInMap(ExpandWhiteSquare(roomsMap, 4), dotMap);

        dotMap = GeneratePathsOnMap(roomDetectionMap);
        corridorMap = AddNotBlack(SubtractToBlack(dotMap, roomDetectionMap), roomsMap);

        physicalMap = OtherColorsToTwo(dotMap, true);
        physicalMap = ExpandWhiteSquare(physicalMap, corridorSize);

        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                spawnTiles.SetGridObject(x, z, physicalMap.GetPixel(x, z) == Color.white);
            }
        }

        Texture2D[] list = new Texture2D[2];
        list[0] = physicalMap;
        list[1] = roomsMap;

        physicalMap = MergeWhite(list);

        Color[,] colors = new Color[MapWidth, MapHeight];

        var genOffset = 0.1f;
        GenerationProgress = genOffset;
        var GenOffset2 = 0f;

        bool DetectCorner(int x, int y, int ex, int ey) => 
            (PixelIsB(physicalMap, x + ex, y + ey) && (PixelIsB(physicalMap, x + ex, y) && PixelIsB(physicalMap, x, y + ey)) || 
            (PixelIsB(physicalMap, x + ex, y + ey) && PixelIsW(physicalMap, x + ex, y) && PixelIsW(physicalMap, x, y + ey)));

        foreach (RoomNode room in rooms)
        {
            room.blocks.Add(PlaceBlock(new Vector3(room.LeftDownCornerPosition.x, 1.5f, room.LeftDownCornerPosition.y), biome.pillarBlocks[0], true, null, null));
            room.blocks.Add(PlaceBlock(new Vector3(room.RightUpCornerPosition.x, 1.5f, room.RightUpCornerPosition.y), biome.pillarBlocks[0], true, null, null));
            room.blocks.Add(PlaceBlock(new Vector3(room.LeftDownCornerPosition.x, 1.5f, room.RightUpCornerPosition.y), biome.pillarBlocks[0], true, null, null));
            room.blocks.Add(PlaceBlock(new Vector3(room.RightUpCornerPosition.x, 1.5f, room.LeftDownCornerPosition.y), biome.pillarBlocks[0], true, null, null));

            for (int x = room.LeftDownCornerPosition.x; x <= room.RightUpCornerPosition.x; x++)
            {
                for (int y = room.LeftDownCornerPosition.y; y <= room.RightUpCornerPosition.y; y++)
                {
                    if (corridorMap.GetPixel(x, y) == Color.white || corridorMap.GetPixel(x, y) == Color.black)
                        continue;
                    room.doors.Add(new Door(new Vector2Int(x, y), x == room.LeftDownCornerPosition.x ? new Vector2Int(-1, 0) : x == room.RightUpCornerPosition.x ? new Vector2Int(1, 0) : y == room.LeftDownCornerPosition.y ? new Vector2Int(0, -1) : new Vector2Int(0, 1)));
                }
            }
            GenOffset2 += 0.1f / rooms.Count;
            GenerationProgress = genOffset + GenOffset2;

            foreach (Door door in room.doors)
            {
            remadeDoor:;
                var doorObj = PlaceBlock(new Vector3(door.position.x, 1.5f, door.position.y), biome.doorBlock, true, null, null);
                room.blocks.Add(doorObj);
                door.doorBlock = doorObj;

                if (doorObj == null)
                {
                    if (walls[door.position.x, door.position.y] != null)
                    {
                        Destroy(walls[door.position.x, door.position.y]);
                        walls[door.position.x, door.position.y] = null;
                    }
                    goto remadeDoor;
                }
                doorObj.AddComponent<Animator>();
                doorObj.GetComponent<Animator>().runtimeAnimatorController = biome.doorAnimationController;

                if (door.facing.x == 1 || door.facing.x == -1)
                {
                    if (door.facing.x == 1)
                        doorObj.transform.rotation = Quaternion.Euler(doorObj.transform.rotation.eulerAngles + new Vector3(0, 0, 90f));
                    else
                        doorObj.transform.rotation = Quaternion.Euler(doorObj.transform.rotation.eulerAngles + new Vector3(0, 0, 90f));

                    //PlaceBlock(new Vector3(door.position.x, 3f, door.position.y), biome.pillarBlocks[0], true, null, null);
                    PlaceBlock(new Vector3(door.position.x, 1.5f, door.position.y + 2), biome.pillarBlocks[0], true, null, null);
                    PlaceBlock(new Vector3(door.position.x, 1.5f, door.position.y - 2), biome.pillarBlocks[0], true, null, null);
                }
                else if (door.facing.y == 1 || door.facing.y == -1)
                {
                    if (door.facing.y == 1)
                        doorObj.transform.rotation = Quaternion.Euler(doorObj.transform.rotation.eulerAngles + new Vector3(0, 0, + 180f));
                    else
                        doorObj.transform.rotation = Quaternion.Euler(doorObj.transform.rotation.eulerAngles + new Vector3(0, 0, 0));

                    //PlaceBlock(new Vector3(door.position.x, 3f, door.position.y), biome.pillarBlocks[0], true, null, null);
                    PlaceBlock(new Vector3(door.position.x - 2, 1.5f, door.position.y), biome.pillarBlocks[0], true, null, null);
                    PlaceBlock(new Vector3(door.position.x + 2, 1.5f, door.position.y), biome.pillarBlocks[0], true, null, null);
                }
            }
            await Task.Delay(1);
        }
        genOffset = 0.2f;
        GenOffset2 = 0;
        //generate columns borders in corridors
        //X = X, Z = Y
        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                if (corridorMap.GetPixel(x, z) == Color.white || physicalMap.GetPixel(x, z) == Color.black)
                    continue;
                if (DetectCorner(x, z, 1, 1))
                {
                    PlaceBlock(x + 1, 1.5f, z + 1, biome.pillarBlocks[0], true, null, null);
                }
                else if (DetectCorner(x, z, -1, -1))
                {
                    PlaceBlock(x - 1, 1.5f, z - 1, biome.pillarBlocks[0], true, null, null);
                }
                else if (DetectCorner(x, z, 1, -1))
                {
                    PlaceBlock(x + 1, 1.5f, z - 1, biome.pillarBlocks[0], true, null, null);
                }
                else if (DetectCorner(x, z, -1, 1))
                {
                    PlaceBlock(x - 1, 1.5f, z + 1, biome.pillarBlocks[0], true, null, null);
                }
            }
            GenOffset2 += 0.1f / MapWidth;
            GenerationProgress = genOffset + GenOffset2;
            await Task.Delay(1);
        }
        genOffset = 0.3f;
        GenOffset2 = 0;
        //General walls
        //X = X, Z = Y
        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                if (physicalMap.GetPixel(x, z) == Color.white)
                    PlaceBlock(x, DEFAULT_GROUND_HEIGHT, z, biome.groundBlocks[0]);

                if (physicalMap.GetPixel(x, z) == Color.black)
                {
                    bool generateWall = false;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if ((j == 0 && i == 0) || !IsInside2DArray(x + i, z + j, colors))
                                continue;
                            if (physicalMap.GetPixel(x + i, z + j) == Color.white)
                            {
                                generateWall = true;
                                break;
                            }
                        }
                    }
                    if (generateWall && walls[x, z] == null)
                    {
                        var ground = PlaceBlock(x, DEFAULT_GROUND_HEIGHT, z, biome.groundBlocks[0]);
                        if (roomGrid[x, z] > -1 && ground != null)
                            rooms[roomGrid[x, z] - 1].blocks.Add(ground);
                        if (physicalMap.GetPixel(x - 1, z) == Color.white)
                        {
                            PlaceBlock(x, 1.5f, z, biome.wallBlocks[0], true, new Vector3(0, 0, -90), null);
                        }
                        else if (physicalMap.GetPixel(x + 1, z) == Color.white)
                        {
                            PlaceBlock(x, 1.5f, z, biome.wallBlocks[0], true, new Vector3(0, 0, 90), null);
                        }
                        else
                        {
                            PlaceBlock(x, 1.5f, z, biome.wallBlocks[0], true, null, null);
                        }
                    }
                    else if (generateWall && floors[x, z] == null)
                    {
                        var ground = PlaceBlock(x, DEFAULT_GROUND_HEIGHT, z, biome.groundBlocks[0]);
                        if (roomGrid[x, z] != -1 && ground != null)
                        {
                            /*Debug.Log(roomGrid[x, z]);
                            Debug.Log(rooms.Count);*/
                            rooms[roomGrid[x, z] - 1].blocks.Add(ground);
                        }
                    }
                }
            }

            GenOffset2 += 0.6f / MapWidth;
            GenerationProgress = genOffset + GenOffset2;
            await Task.Delay(1);
        }
        genOffset = 0.9f;
        GenOffset2 = 0;

        //generates torchs
        //torchMap = MakePointsSeparated(torchMap, torchOffset);
        torchMap = GenerateNoiseTexture(MapWidth, MapHeight, seed, 0.8f, 0.5f, 0f, true);

        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                if (torchMap.GetPixel(x, z) == Color.black || physicalMap.GetPixel(x, z) == Color.black || walls[x, z] != null || (roomGrid[x, z] != -1 && !rooms[roomGrid[x, z] - 1].info.universal))
                    continue;
                for (int ex = -torchMinDistance; ex <= torchMinDistance; ex++)
                {
                    for (int ez = -torchMinDistance; ez <= torchMinDistance; ez++)
                    {
                        if (ex == 0 && ez == 0)
                            continue;
                        if (IsInside2DArray(x, z, roomGrid.GetArray()))
                        {
                            if (walls[x + ex, z + ez] != null)
                                if (walls[x + ex, z + ez].name.Contains(biome.torchBlock.blockPrefab.name) || walls[x + ex, z + ez].name.Contains(biome.doorBlock.blockPrefab.name))
                                    goto exitTorch;
                        }
                        else
                            goto exitTorch;
                    }
                }

                if (walls[x + 1, z] != null && !walls[x + 1, z].name.Contains(biome.torchBlock.blockPrefab.name))
                {
                    PlaceBlock(x, 1.5f, z, biome.torchBlock, true, new Vector3(0, 90f, 0));
                }
                else if (walls[x - 1, z] != null && !walls[x - 1, z].name.Contains(biome.torchBlock.blockPrefab.name))
                {
                    PlaceBlock(x, 1.5f, z, biome.torchBlock, true, new Vector3(0, -90f, 0));
                }
                else if (walls[x, z + 1] != null && !walls[x, z + 1].name.Contains(biome.torchBlock.blockPrefab.name))
                {
                    PlaceBlock(x, 1.5f, z, biome.torchBlock, true);
                }
                else if (walls[x, z - 1] != null && !walls[x, z - 1].name.Contains(biome.torchBlock.blockPrefab.name))
                {
                    PlaceBlock(x, 1.5f, z, biome.torchBlock, true, new Vector3(0, -180f, 0));
                }
            exitTorch:;
            }

            GenOffset2 += 0.1f / MapWidth;
            GenerationProgress = genOffset + GenOffset2;

            await Task.Delay(1);
        }
        //rooms reconstitution?
        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                if (roomGrid.GetGridObject(x, z) == -1)
                    continue;
                var room = rooms[roomGrid.GetGridObject(x, z) - 1];
                if (GetWallObj(x, z) != null && !room.blocks.Contains(GetWallObj(x, z)))
                    room.blocks.Add(GetWallObj(x, z));
                /*if (GetFloorObj(x, z) != null && !room.blocks.Contains(GetFloorObj(x, z)))
                    room.blocks.Add(GetFloorObj(x, z));*/
            }
        }

        //mesh gen
        foreach (var room in rooms)
        {
            room.CombineRoomBlocksMesh();
        }
        GenerateCorridorMesh();

        await Task.Delay(1);
        BuildNavMesh(10);

        if (GameManager.gameManagerInstance == null)
            throw new System.Exception("Manager is null");
        if (GameManager.gameManagerInstance.playerPrefab == null)
            throw new System.Exception("Manager player prefab is null");

        Debug.Log($"DIED = {PlayerPreferences.Died}, GAME_SAVED = {PlayerPreferences.GameSaved}, NEW_GAME = {PlayerPreferences.NewGame}");
        await Task.Delay(20);

        redoPlayerPos:
        var xP = Random.Range(rooms[0].LeftDownCornerPositionInternal.x, rooms[0].RightUpCornerPositionInternal.x);
        var yP = Random.Range(rooms[0].LeftDownCornerPositionInternal.y, rooms[0].RightUpCornerPositionInternal.y);
        if (spawnTiles[xP, yP] == false)
            goto redoPlayerPos;
        Vector3 PlayerPos = new Vector3 (xP + 0.5f, 1f, yP + 0.5f);

        if (GamePlayer.player == null)
        {
            var entityPlayer = Instantiate(GameManager.gameManagerInstance.playerPrefab, PlayerPos, Quaternion.identity, null);
            if (PlayerPreferences.NewGame)
            {
                PlayerPreferences.SavePlayerData(entityPlayer.GetComponent<GamePlayer>().EntityData);
            }
            else
            {
                PlayerPreferences.LoadPlayerData();
                PlayerPreferences.SavePlayerData(GamePlayer.player.EntityData);
            }
        }
        else
            GamePlayer.player.transform.position = PlayerPos;

        GamePlayer.player.OnDeathEvent.AddListener(delegate { GameManager.gameManagerInstance.ExcludePlayerDataOnDeath(); });
        var mapTexture = GetTexture(physicalMap);
        for (int x = 0; x < mapTexture.width; x++)
        {
            for (int y = 0; y < mapTexture.height; y++)
            {
                if (mapTexture.GetPixel(x, y) == Color.white)
                {
                    mapTexture.SetPixel(x, y, Color.gray);
                }
            }
        }
        mapTexture.Apply();
        mapTexture.filterMode = FilterMode.Point;
        mapTexture.Compress(false);
        if (MapMove.MapMoveInstance != null)
            MapMove.MapMoveInstance.map.sprite = Sprite.Create(mapTexture, new Rect(0, 0, mapTexture.width, mapTexture.height), new Vector2(0.5f, 0.5f), 64);

        GamePlayer.player.OnDeathEvent.AddListener((a, b) => { PlayDeathScene(); });

        mapLoaded = true;
        return;
    }
    public GameObject PlaceBlock(int x, float y, int z, BlockClass type, bool wall = false, Vector3? rotation = null, Vector3? scale = null)
    {
        if (floors[x, z] != null && !wall)
            return null;
        if (walls[x, z] != null && wall)
            return null;
        if (rotation == null)
            rotation = Vector3.zero;
        if (scale == null)
            scale = type.blockPrefab.transform.localScale;

        GameObject block = Instantiate(type.blockPrefab, null, true);
        block.name = type.blockName + " " + x + " " + y + " " + z;
        GameObject parent = null;

        if (type.isDoor)
        {
            parent = new GameObject("Door_Parent " + x + " " + y + " " + z);
            block.transform.position = Vector3.zero;
            block.transform.parent = parent.transform;
            parent.transform.position = new Vector3(x + 0.5f, y, z + 0.5f);
        }
        else
        {
            block.transform.position = new Vector3(x + 0.5f, y, z + 0.5f);
        }
        block.transform.localScale = (Vector3)scale;
        block.transform.rotation = Quaternion.Euler(type.blockPrefab.transform.rotation.eulerAngles + (Vector3)rotation);

        if (block.GetComponent<Collider>() != null)
        {
            /*block.AddComponent<Rigidbody>();
            block.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            block.GetComponent<Rigidbody>().isKinematic = true;
            block.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;*/
            /*if (type.isDestructible)
            {
                block.AddComponent<BlockEntity>();
                block.GetComponent<BlockEntity>().EntityData.maxHealth = type.hitPoints;
                block.GetComponent<BlockEntity>().EntityData.currentHealth = type.hitPoints;
            }*/
        }

        if (!wall)
        {
            floors.SetGridObject(x, z, block);
            block.transform.parent = floorParent.transform;
            block.layer = 7;
        }
        else
        {
            if (!type.isDoor)
            {
                walls.SetGridObject(x, z, block);
                block.transform.parent = wallParent.transform;
            }
            else
            {
                walls.SetGridObject(x, z, block);
                rooms[roomGrid[x, z] - 1].blocks.Add(block);
                parent.transform.parent = wallParent.transform;
            }
            block.layer = 6;
        }

        return block;
    }
    public GameObject PlaceBlock(Vector3 coord, BlockClass type, bool wall = false, Vector3? rotation = null, Vector3? scale = null) => PlaceBlock((int)coord.x, coord.y, (int)coord.z, type, wall, rotation, scale);
    public GameObject GetWallObj(int x, int y) => walls.GetGridObject(x, y);
    public GameObject GetFloorObj(int x, int y) => floors.GetGridObject(x, y);
    /// <summary>
    /// Gera caminhos entre pixeis brancos, ótimo para gerar Dungeons!
    /// </summary>
    /// <param name="texture">A textura com os pontos.</param>
    /// <returns>Nova textura com os caminhos em verde. (Color.green)</returns>
    public Texture2D GeneratePathsOnMap(Texture2D texture)
    {
        Texture2D dotMap = GetTexture(texture);

        Pathfinding pathfinding = new Pathfinding(texture.width, texture.height)
        {
            MOVE_DIAGONAL_COST = 25
        };
        List<Vector2> pointsFound = new();
        List<Vector2> pointsReached = new();
        List<Vector2> pointsAnalyzed = new();

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (dotMap.GetPixel(x, y).r == 0)
                    continue;

                pointsFound.Add(new Vector2(x, y));
            }
        }
        Vector2 point1 = pointsFound[UnityEngine.Random.Range(0, pointsFound.Count - 1)];
        Vector2 point2 = new Vector2(-1000, -1000);

        for (int i = 0; i < pointsFound.Count - 1; i++)
        {
            int range = 10;
            bool Rx = UnityEngine.Random.Range(0, range) >= range / 2;
            bool Ry = UnityEngine.Random.Range(0, range) >= range / 2;

            //Debug.Log("The interaction " + i + " has the parameters: X = " + Rx + " Y = " + Ry);

            bool firstDot = true;
            for (int x = Rx == true ? 0 : texture.width - 1;
                x >= 0 && x < texture.height;
                x = Rx == true ? x + 1 : x - 1)
            {
                for (int y = Ry == true ? 0 : texture.width - 1;
                    y >= 0 && y < texture.height;
                    y = Ry == true ? y + 1 : y - 1)
                {
                    if (firstDot)
                    {
                        if (dotMap.GetPixel(x, y).r == 1)
                        {
                            if (IsReachedPoint(new Vector2(x, y)))
                                continue;
                            point1 = new Vector2(x, y);
                            pointsReached.Add(point1);
                            firstDot = false;
                        }
                    }
                    else
                    {
                        if (dotMap.GetPixel(x, y).r == 0 || (x == (int)point1.x && y == (int)point1.y) || IsReachedPoint(new Vector2(x, y)))
                            continue;
                        point2 = Vector2.Distance(point1, new Vector2(x, y)) < Vector2.Distance(point1, point2) ? new Vector2(x, y) : point2;
                    }
                }
            }
            foreach (PathNode node in pathfinding.FindPathB((int)point1.x, (int)point1.y, (int)point2.x, (int)point2.y, (x, y) => { return texture.GetPixel(x, y) != Color.green; }).ToArray())
            {
                if (node == null || dotMap.GetPixel(node.x, node.y) == Color.white)
                    continue;
                dotMap.SetPixel(node.x, node.y, Color.green);
            }
            point1 = point2;
            point2 = new Vector2(-1000, -1000);
        }
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (texture.GetPixel(x, y) == Color.green)
                    dotMap.SetPixel(x, y, Color.black);
            }
        }
        dotMap.Apply();
        return dotMap;
        bool IsReachedPoint(Vector2 currentPoint)
        {
            foreach (Vector2 point in pointsReached)
            {
                if (point.Equals(currentPoint))
                    return true;
            }
            return false;
        }
    }
    /// <summary>
    /// Script que aumenta pontos brancos únicos para quadrados/retângulos maiores que oscilam entre os tamanhos mínimos e máximos de x e y, definindo também um espaço máximo entre esses retângulos.
    /// </summary>
    /// <param name="texture">A textura com os pontos.</param>
    /// <param name="minX">Tamanho mínimo em x.</param>
    /// <param name="minY">Tamanho mínimo em y.</param>
    /// <param name="maxX">Tamanho máximo em x.</param>
    /// <param name="maxY">Tamanho máximo em y</param>
    /// <param name="space">Espaço máximo entre os retângulos.</param>
    /// <returns></returns>
    public Texture2D ExpandWhiteDotsRandomly(Texture2D texture, int minX, int maxX, int space)
    {
        var newTex = GetTexture(texture);
        Color[,] colors = new Color[texture.width, texture.height];
        int _space = space + 1;

        Color32 undefinedColor = new Color32(64, 64, 64, 255);
        byte currentID = 0;

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (texture.GetPixel(x, y) == Color.white)
                {
                    currentID++;
                    newTex.SetPixel(x, y, Color.black);
                    newTex.Apply();

                    int Size1 = UnityEngine.Random.Range(minX, maxX) / 2;
                    int Size2 = UnityEngine.Random.Range(minX, maxX) / 2;

                    var roomObj = new GameObject("Room " + currentID);
                    roomObj.transform.SetParent(transform);
                    roomObj.layer = 2;

                    var room = roomObj.AddComponent<RoomNode>();
                    room.NewRoomNode(currentID, new Vector2Int(x, y), Size1 + Size2);

                    RoomInfo info = null;
                    foreach (var rInfo in biome.rooms)
                    {
                        if (rInfo.size == room.size)
                        {
                            info = rInfo;
                            Debug.Log(rInfo.name);
                            break;
                        }
                    }

                    room.LeftDownCornerPosition = new Vector2Int(-Size1 + x - 1, -Size1 + y - 1);
                    room.RightUpCornerPosition = new Vector2Int(Size2 + x + 0, Size2 + y + 0);

                    if (info != null)
                        room.SetRoomInfo(info);
                    else
                        room.SetRoomInfo(biome.defaultRoom);

                    roomObj.transform.position = new Vector3(room.LeftDownCornerPositionInternal.x + (room.size / 2f), 1.5f, room.LeftDownCornerPositionInternal.y + (room.size / 2f));
                    roomObj.AddComponent<BoxCollider>().size = new Vector3(room.size, 2, room.size);
                    roomObj.GetComponent<BoxCollider>().isTrigger = true;

                    rooms.Add(room);

                    //Debug.Log(room.ToString());

                    for (int xE = -Size1 + x; xE < Size2 + x; xE++)
                    {
                        for (int yE = -Size1 + y; yE < Size2 + y; yE++)
                        {
                            //ponto atual
                            if (!IsInside2DArray(xE, yE, colors) ||
                                //retos
                                !IsInside2DArray(xE + _space, yE, colors) ||
                                !IsInside2DArray(xE - _space, yE, colors) ||
                                !IsInside2DArray(xE, yE + _space, colors) ||
                                !IsInside2DArray(xE, yE - _space, colors) ||
                                //diagonais
                                !IsInside2DArray(xE + _space, yE + _space, colors) ||
                                !IsInside2DArray(xE - _space, yE + _space, colors) ||
                                !IsInside2DArray(xE + _space, yE - _space, colors) ||
                                !IsInside2DArray(xE - _space, yE - _space, colors))
                                continue;
                            //reto
                            if ((newTex.GetPixel(xE + space, yE) == Color.black) ||
                                (newTex.GetPixel(xE - space, yE) == Color.black) ||
                                (newTex.GetPixel(xE, yE + space) == Color.black) ||
                                (newTex.GetPixel(xE, yE - space) == Color.black) ||
                                //diagonais
                                (newTex.GetPixel(xE + space, yE + space) == Color.black) ||
                                (newTex.GetPixel(xE - space, yE + space) == Color.black) ||
                                (newTex.GetPixel(xE + space, yE - space) == Color.black) ||
                                (newTex.GetPixel(xE - space, yE - space) == Color.black))
                                newTex.SetPixel(xE, yE, undefinedColor);
                        }
                    }
                    for (int xE = -Size1 - 1 + x; xE < Size2 + 1 + x; xE++)
                    {
                        for (int yE = -Size1 - 1 + y; yE < Size2 + 1 + y; yE++)
                        {
                            roomGrid.SetGridObject(xE, yE, currentID);
                        }
                    }
                    newTex.Apply();

                    for (int xR = 0; xR < texture.width; xR++)
                    {
                        for (int yR = 0; yR < texture.height; yR++)
                        {
                            if (newTex.GetPixel(xR, yR) == undefinedColor)
                                newTex.SetPixel(xR, yR, Color.white);
                        }
                    }
                }
            }
        }

        newTex.Apply();
        return newTex;
    }
    public Texture2D GenerateCrossesInMap(Texture2D whiteSquaresTex, Texture2D dotsTex)
    {
        Texture2D newTex = GetTexture(whiteSquaresTex);
         
        for (int x = 0; x < whiteSquaresTex.width; x++)
        {
            for (int y = 0; y < whiteSquaresTex.height; y++)
            {
                if (whiteSquaresTex.GetPixel(x, y) == Color.white && dotsTex.GetPixel(x, y) != Color.white)
                {
                    newTex.SetPixel(x, y, Color.green);
                }
            }
        }
        for (int x = 0; x < whiteSquaresTex.width; x++)
        {
            for (int y = 0; y < whiteSquaresTex.height; y++)
            {
                if (dotsTex.GetPixel(x, y) == Color.white)
                {
                    ContinueCross(x + 1, y, 0);
                    ContinueCross(x - 1, y, 1);
                    ContinueCross(x, y + 1, 2);
                    ContinueCross(x, y - 1, 3);
                }
            }
        }

        newTex.Apply();
        return newTex;

        ///side 0 = x 1, side 1 = x -1, side 2 = y 1, side 3 = y -1
        void ContinueCross(int x, int y, byte side)
        {
            if (whiteSquaresTex.GetPixel(x, y) == Color.white)
                newTex.SetPixel(x, y, Color.black);
            else
                return;
            ContinueCross(
                x + (side == 0 ? 1 : side == 1 ? -1 : 0), 
                y + (side == 2 ? 1 : side == 3 ? -1 : 0), 
                side);
        }
    }
}
