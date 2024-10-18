using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TextureFunction;
using GameManagerSystem;
using Pathfindingsystem;
using RoomSystem;
using static NavMeshUpdate;

public class TerrainGeneration : MonoBehaviour
{
    public static TerrainGeneration Instance;
    [SerializeField] private GameObject wallParent, floorParent;

    public Biome biome;

    public List<RoomNode> rooms;

    public Texture2D dotMap;
    public Texture2D physicalMap;
    public Texture2D roomsMap;
    public Texture2D roomDetectionMap;
    public Texture2D corridorMap;

    public Mesh DEFAULT_MESH;
    public Material DEFAULT_MATERIAL;

    public int seed; public float frequency, limit, scattering;

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
    #endregion
    public GameObject[,] walls;
    public GameObject[,] floors;

    private const int DEFAULT_SEED = 24556;
    [SerializeField]
    private bool configSeed = false;

    void Start()
    {
        Instance = this;

        if (!configSeed)
        {
            seed = PlayerPrefs.GetInt("CURRENT_SEED", DEFAULT_SEED);
            MapWidth = PlayerPrefs.GetInt("MAP_WIDTH", 100);
            MapHeight = PlayerPrefs.GetInt("MAP_HEIGHT", 100);
        }

        pathfinding = new Pathfinding(MapWidth, MapHeight);

        rooms = new List<RoomNode>();

        GenerateLevel();
    }
    public void GenerateLevel()
    {
        Random.InitState(seed);

        walls = new GameObject[MapWidth, MapHeight];
        floors = new GameObject[MapWidth, MapHeight];

        dotMap = new Texture2D(MapWidth, MapHeight);
        physicalMap = new Texture2D(MapWidth, MapHeight);
        roomDetectionMap = new Texture2D(MapWidth, MapHeight);

        dotMap = GenerateNoiseTexture(MapWidth, MapHeight, seed, frequency, limit, scattering, true, StageOffSet * CurrentStage, StageOffSet * CurrentStage);

        dotMap = SeparateWhiteDots(dotMap, minimumDistanceBetweenRooms, maxRoomSize + 4);

        roomsMap = ExpandWhiteDotsRandomly(dotMap, minRoomSize, minRoomSize, maxRoomSize, maxRoomSize, 3);

        roomDetectionMap = GenerateCrossesInMap(ExpandWhiteSquare(roomsMap, 4), dotMap);

        dotMap = GeneratePathsOnMap(roomDetectionMap);
        corridorMap = AddNotBlack(SubtractToBlack(dotMap, roomDetectionMap), roomsMap);

        physicalMap = OtherColorsToTwo(dotMap, true);
        physicalMap = ExpandWhiteSquare(physicalMap, corridorSize);

        Texture2D[] list = new Texture2D[2];
        list[0] = physicalMap;
        list[1] = roomsMap;

        physicalMap = MergeWhite(list);

        Color[,] colors = new Color[MapWidth, MapHeight];

        bool DetectCorner(int x, int y, int ex, int ey) => !PixelIsBW(corridorMap, x + ex, y) && !PixelIsBW(corridorMap, x, y + ey);

        foreach (RoomNode room in rooms)
        {
            PlaceBlock(new Vector3(room.LeftDownCornerPosition.x, 1.5f, room.LeftDownCornerPosition.y), biome.pillarBlocks[0], true, null, null);
            PlaceBlock(new Vector3(room.RightUpCornerPosition.x, 1.5f, room.RightUpCornerPosition.y), biome.pillarBlocks[0], true, null, null);
            PlaceBlock(new Vector3(room.LeftDownCornerPosition.x, 1.5f, room.RightUpCornerPosition.y), biome.pillarBlocks[0], true, null, null);
            PlaceBlock(new Vector3(room.RightUpCornerPosition.x, 1.5f, room.LeftDownCornerPosition.y), biome.pillarBlocks[0], true, null, null);

            for (int x = room.LeftDownCornerPosition.x; x <= room.RightUpCornerPosition.x; x++)
            {
                for (int y = room.LeftDownCornerPosition.y; y <= room.RightUpCornerPosition.y; y++)
                {
                    if (corridorMap.GetPixel(x, y) == Color.white || corridorMap.GetPixel(x, y) == Color.black)
                        continue;
                    room.doors.Add(new Door(new Vector2Int(x, y), x == room.LeftDownCornerPosition.x ? new Vector2Int(-1, 0) : x == room.RightUpCornerPosition.x ? new Vector2Int(1, 0) : y == room.LeftDownCornerPosition.y ? new Vector2Int(0, -1) : new Vector2Int(0, 1)));
                }
            }

            foreach (Door door in room.doors)
            {
            remadeDoor:;
                var doorObj = PlaceBlock(new Vector3(door.position.x, 1.5f, door.position.y), biome.doorBlock, true, null, null);
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
                        doorObj.transform.rotation = Quaternion.Euler(biome.doorBlock.blockRotation.x, biome.doorBlock.blockRotation.y, biome.doorBlock.blockRotation.z - 90f);
                    else
                        doorObj.transform.rotation = Quaternion.Euler(biome.doorBlock.blockRotation.x, biome.doorBlock.blockRotation.y, biome.doorBlock.blockRotation.z + 90f);

                    //PlaceBlock(new Vector3(door.position.x, 3f, door.position.y), biome.pillarBlocks[0], true, null, null);
                    PlaceBlock(new Vector3(door.position.x, 1.5f, door.position.y + 2), biome.pillarBlocks[0], true, null, null);
                    PlaceBlock(new Vector3(door.position.x, 1.5f, door.position.y - 2), biome.pillarBlocks[0], true, null, null);
                }
                else if (door.facing.y == 1 || door.facing.y == -1)
                {
                    if (door.facing.y == 1)
                        doorObj.transform.rotation = Quaternion.Euler(biome.doorBlock.blockRotation.x, biome.doorBlock.blockRotation.y, biome.doorBlock.blockRotation.z + 180f);
                    else
                        doorObj.transform.rotation = Quaternion.Euler(biome.doorBlock.blockRotation.x, biome.doorBlock.blockRotation.y, biome.doorBlock.blockRotation.z);

                    //PlaceBlock(new Vector3(door.position.x, 3f, door.position.y), biome.pillarBlocks[0], true, null, null);
                    PlaceBlock(new Vector3(door.position.x - 2, 1.5f, door.position.y), biome.pillarBlocks[0], true, null, null);
                    PlaceBlock(new Vector3(door.position.x + 2, 1.5f, door.position.y), biome.pillarBlocks[0], true, null, null);
                }
            }
        }

        //X = X, Z = Y
        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                if (corridorMap.GetPixel(x, z) == Color.white || corridorMap.GetPixel(x, z) == Color.black)
                    continue;
                if (DetectCorner(x, z, 1, 1) || DetectCorner(x, z, -1, -1))
                {
                    PlaceBlock(x + 2, 1.5f, z + 2, biome.pillarBlocks[0], true, null, null);
                    PlaceBlock(x - 2, 1.5f, z - 2, biome.pillarBlocks[0], true, null, null);
                }
                if (DetectCorner(x, z, 1, -1) || DetectCorner(x, z, -1, 1))
                {
                    PlaceBlock(x + 2, 1.5f, z - 2, biome.pillarBlocks[0], true, null, null);
                    PlaceBlock(x - 2, 1.5f, z + 2, biome.pillarBlocks[0], true, null, null);
                }
            }
        }

        //X = X, Z = Y
        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                if (physicalMap.GetPixel(x, z) == Color.white)
                    PlaceBlock(x, 0, z, biome.groundBlocks[0]);

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
                        PlaceBlock(x, 0, z, biome.groundBlocks[0]);
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
                        PlaceBlock(x, 0, z, biome.groundBlocks[0]);
                    }
                }
            }
        }

        navMeshUpdateInstance.BuildNavMesh();
        if (GameManager.gameManagerInstance == null)
            throw new System.Exception("Manager is null");
        if (GameManager.gameManagerInstance.playerPrefab == null)
            throw new System.Exception("Manager player prefab is null");
        Instantiate(GameManager.gameManagerInstance.playerPrefab, new Vector3(rooms[0].transform.position.x, 1f, rooms[0].transform.position.z), Quaternion.identity, transform.parent);
    }
    public
    #region medo
    void Update()
    {
        
    }
    #endregion
    public GameObject PlaceBlock(int x, float y, int z, BlockClass type, bool wall = false, Vector3? rotation = null, Vector3? scale = null)
    {
        if (floors[x, z] != null && !wall)
            return null;
        if (walls[x, z] != null && wall)
            return null;
        if (rotation == null)
            rotation = Vector3.zero;
        if (scale == null)
            scale = type.blockSize;

        GameObject block = new GameObject(type.blockName + " " + x + " " + y + " " + z, typeof(MeshFilter), typeof(MeshRenderer));
        GameObject parent = null;
        //block.isStatic = true;

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
        block.transform.rotation = Quaternion.Euler(type.blockRotation + (Vector3)rotation);

        block.GetComponent<MeshFilter>().mesh = type.mesh == null ? DEFAULT_MESH : type.mesh;

        block.GetComponent<MeshRenderer>().material = type.material == null ? DEFAULT_MATERIAL : type.material;

        //block.transform.localScale = new Vector3(type.blockSize.x, type.blockSize.y, type.blockSize.z);

        if (type.hasCollider)
        {
            if (type.isBlock)
            {
                block.AddComponent<BoxCollider>();
                block.GetComponent<BoxCollider>().size += new Vector3(0.1f, 0, 0.1f);
            }
            else
            {
                block.AddComponent<MeshCollider>();
            }
            block.AddComponent<Rigidbody>();
            //block.GetComponent<Rigidbody>().freezeRotation = true;
            block.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            block.GetComponent<Rigidbody>().isKinematic = true;
            block.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        if (!wall)
        {
            floors[x, z] = block;
            block.transform.parent = floorParent.transform;
            block.layer = 7;
        }
        else
        {
            walls[x, z] = block;
            if (!type.isDoor)
                block.transform.parent = wallParent.transform;
            else
                parent.transform.parent = wallParent.transform;
            block.layer = 6;
        }
        return block;
    }
    public GameObject SpawnEntity(float x, float z, string tag)
    {
        var entity = new GameObject();

        return entity;
    }
    public GameObject PlaceBlock(Vector3 coord, BlockClass type, bool wall = false, Vector3? rotation = null, Vector3? scale = null) => PlaceBlock((int)coord.x, coord.y, (int)coord.z, type, wall, rotation, scale);
    public GameObject GetWallObj(int x, int y) => walls[x, y];
    public GameObject GetFloorObj(int x, int y) => floors[x, y];
    /// <summary>
    /// Gera caminhos entre pixeis brancos, ótimo para gerar Dungeons!
    /// </summary>
    /// <param name="texture">A textura com os pontos.</param>
    /// <returns>Nova textura com os caminhos em verde. (Color.green)</returns>
    public Texture2D GeneratePathsOnMap(Texture2D texture)
    {
        Texture2D dotMap = GetTexture(texture);

        Pathfinding pathfinding = new Pathfinding(texture.width, texture.height);

        pathfinding.MOVE_DIAGONAL_COST = 25;
        List<Vector2> pointsFound = new List<Vector2>();
        List<Vector2> pointsReached = new List<Vector2>();
        List<Vector2> pointsAnalyzed = new List<Vector2>();

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
            bool Rx = UnityEngine.Random.Range(0, range) < range / 2 ? false : true;
            bool Ry = UnityEngine.Random.Range(0, range) < range / 2 ? false : true;

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
    public Texture2D ExpandWhiteDotsRandomly(Texture2D texture, int minX, int minY, int maxX, int maxY, int space)
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

                    int Left = UnityEngine.Random.Range(-minX, -maxX);
                    int Right = UnityEngine.Random.Range(minX, maxX);

                    int Up = UnityEngine.Random.Range(minY, maxY);
                    int Down = UnityEngine.Random.Range(-minY, -maxY);

                    var roomObj = new GameObject("Room " + currentID);
                    var room = roomObj.AddComponent<RoomNode>();
                    room.info = biome.defaultRoom;

                    room.NewRoomNode(currentID, new Vector2Int(x, y), -Left + Right + 2, -Down + Up + 2);

                    room.LeftDownCornerPosition = new Vector2Int(Left + x - 1, Down + y - 1);
                    room.RightUpCornerPosition = new Vector2Int(Right + x + 0, Up + y + 0);

                    roomObj.transform.position = new Vector3(room.LeftDownCornerPosition.x + (room.width / 2f), 1.5f, room.LeftDownCornerPosition.y + (room.height / 2f));
                    roomObj.AddComponent<BoxCollider>().size = new Vector3(room.width - 2f, 2, room.height - 2f);
                    roomObj.GetComponent<BoxCollider>().isTrigger = true;

                    rooms.Add(room);
                    //Debug.Log(room.ToString());

                    for (int xE = Left + x; xE < Right + x; xE++)
                    {
                        for (int yE = Down + y; yE < Up + y; yE++)
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
