using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TextureFunction;
using Pathfindingsystem;
using RoomSystem;

public class TerrainGeneration : MonoBehaviour
{
    public static TerrainGeneration Instance;

    public BlockClass teste;

    public Texture2D dotMap;
    public Texture2D physicalMap;
    public Texture2D roomsMap;
    public Texture2D testMap;

    public Mesh DEFAULT_MESH;
    public Material DEFAULT_MATERIAL;

    public int seed; public float frequency, limit, scattering;

    public int minRoomSize = 3;
    public int maxRoomSize = 6;

    #region Vari�veis pr� definidas e constantes
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

    public Pathfinding pathfinding;

    public int corridorSize = 1;
    public int minimumDistanceBetweenRooms = 12;
    #endregion
    public GameObject[,] blocks;

    void Start()
    {
        Instance = this;

        Random.InitState(seed);

        pathfinding = new Pathfinding(MapWidth, MapHeight);

        blocks = new GameObject[MapWidth, MapHeight];

        dotMap = new Texture2D(MapWidth, MapHeight);
        physicalMap = new Texture2D(MapWidth, MapHeight);
        physicalMap.name = "Physical";

        dotMap = GenerateNoiseTexture(MapWidth, MapHeight, seed, frequency, limit, scattering, true);

        dotMap = SeparateWhiteDots(dotMap, minimumDistanceBetweenRooms, 8);

        dotMap = GeneratePathsOnMap(dotMap);

        physicalMap = GetTexture(dotMap);
        dotMap = DefineColorListForPaths(dotMap);
        roomsMap = ExpandWhiteDotsRandomly(dotMap, minRoomSize, minRoomSize, maxRoomSize, maxRoomSize, 1);
        physicalMap = OtherColorsToTwo(physicalMap, true);
        testMap = ResizeTextureUp(physicalMap, 3, 3);
        physicalMap = ExpandWhiteSquare(physicalMap, corridorSize);

        dotMap.name = "Dungeons";
        GeneratePng(dotMap);
        testMap.name = "Test";
        GeneratePng(testMap);   

        Texture2D[] list = new Texture2D[2];
        list[0] = physicalMap;
        list[1] = roomsMap;

        physicalMap = MergeWhite(list);

        Color[,] colors = new Color[MapWidth, MapHeight];
        //X = X, Z = Y
        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                if (physicalMap.GetPixel(x, z) == Color.white)
                    PlaceBlock(x, 0, z, teste);
                else
                {
                    bool generateWall = false;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if ((j == 0 && i == 0) || !IsInside2DArray(x + i, z + j, colors))
                                continue;
                            if (physicalMap.GetPixel(x + i, z + j) == Color.white)
                                generateWall = true;
                        }
                    }
                    if (generateWall)
                        PlaceBlock(x, 1.5f, z, teste);
                }
            }
        }
        transform.parent.Find("Player").position = new Vector3(MapWidth / 2, 10, MapHeight / 2);
    }
    #region medo
    void Update()
    {
        
    }
    #endregion
    public void PlaceBlock(int x, float y, int z, BlockClass type)
    {
        //GameObject block = new GameObject(type.blockName);
        GameObject block = new GameObject(type.blockName + " " + x + " " + y + " " + z, typeof(MeshFilter), typeof(MeshRenderer));
        block.layer = 6;

        block.transform.position = new Vector3(x + 0.5f, y, z + 0.5f);

        block.GetComponent<MeshFilter>().mesh = type.mesh == null ? DEFAULT_MESH : type.mesh;

        block.GetComponent<MeshRenderer>().material = type.material == null ? DEFAULT_MATERIAL : type.material;

        block.transform.localScale = new Vector3(type.blockSize.x, type.blockSize.y, type.blockSize.z);

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
        }

        blocks[x, z] = block;
    }
    public GameObject SpawnEntity(float x, float z, string tag)
    {
        var entity = new GameObject();

        return entity;
    }
    public void PlaceBlock(Vector3Int coord, BlockClass type)
    {
        PlaceBlock(coord.x, coord.y, coord.z, type);
    }
    public GameObject GetBlockObj(int x, int y) => blocks[x, y];
}
