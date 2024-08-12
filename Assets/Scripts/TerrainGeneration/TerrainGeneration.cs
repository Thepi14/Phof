using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TextureFunction;
using Pathfindingsystem;
using RoomSystem;
using UnityEditor;

public class TerrainGeneration : MonoBehaviour
{
    public static TerrainGeneration Instance;

    public Biome biome;

    public List<RoomNode> rooms;

    public Texture2D dotMap;
    public Texture2D physicalMap;
    public Texture2D roomsMap;

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

        rooms = new List<RoomNode>();

        transform.parent.Find("Player").position = new Vector3(MapWidth / 2, 10, MapHeight / 2);

        GenerateLevel();
    }
    public void GenerateLevel()
    {
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
        //dotMap = ExpandBlackOnNonWhite(dotMap, 2);

        roomsMap = ExpandWhiteDotsRandomly(dotMap, minRoomSize, minRoomSize, maxRoomSize, maxRoomSize, 3);

        physicalMap = OtherColorsToTwo(roomsMap, true);
        physicalMap = ExpandWhiteSquare(physicalMap, corridorSize);

        dotMap.name = "Dungeons";
        GeneratePng(dotMap);

        Texture2D[] list = new Texture2D[2];
        list[0] = physicalMap;
        list[1] = roomsMap;

        physicalMap = MergeWhite(list);

        Color[,] colors = new Color[MapWidth, MapHeight];

        foreach (RoomNode room in rooms)
        {
            PlaceBlock(new Vector3(room.LeftDownCornerPosition.x, 0, room.LeftDownCornerPosition.y), biome.groundBlocks[0]);
            PlaceBlock(new Vector3(room.RightUpCornerPosition.x, 0, room.RightUpCornerPosition.y), biome.groundBlocks[0]);
            PlaceBlock(new Vector3(room.LeftDownCornerPosition.x, 0, room.RightUpCornerPosition.y), biome.groundBlocks[0]);
            PlaceBlock(new Vector3(room.RightUpCornerPosition.x, 0, room.LeftDownCornerPosition.y), biome.groundBlocks[0]);

            PlaceBlock(new Vector3(room.LeftDownCornerPosition.x, 1.5f, room.LeftDownCornerPosition.y), biome.pillarBlocks[0]);
            PlaceBlock(new Vector3(room.RightUpCornerPosition.x, 1.5f, room.RightUpCornerPosition.y), biome.pillarBlocks[0]);
            PlaceBlock(new Vector3(room.LeftDownCornerPosition.x, 1.5f, room.RightUpCornerPosition.y), biome.pillarBlocks[0]);
            PlaceBlock(new Vector3(room.RightUpCornerPosition.x, 1.5f, room.LeftDownCornerPosition.y), biome.pillarBlocks[0]);
        }

        //X = X, Z = Y
        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                if (physicalMap.GetPixel(x, z) == Color.white)
                    PlaceBlock(x, 0, z, biome.groundBlocks[0]);
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
                            {
                                generateWall = true;
                                break;
                            }
                        }
                    }
                    if (generateWall && blocks[x, z] == null)
                    {
                        PlaceBlock(x, 0, z, biome.groundBlocks[0]);
                        if (physicalMap.GetPixel(x - 1, z) == Color.white)
                        {
                            PlaceBlock(x, 1.5f, z, biome.wallBlocks[0], new Vector3(0, 0, -90));
                        }
                        else if (physicalMap.GetPixel(x + 1, z) == Color.white)
                        {
                            PlaceBlock(x, 1.5f, z, biome.wallBlocks[0], new Vector3(0, 0, 90));
                        }
                        else
                        {
                            PlaceBlock(x, 1.5f, z, biome.wallBlocks[0]);
                        }
                    }
                }
            }
        }
    }
    #region medo
    void Update()
    {
        
    }
    #endregion
    public void PlaceBlock(int x, float y, int z, BlockClass type, Vector3? rotation = null, Vector3? scale = null)
    {
        if (rotation == null)
            rotation = Vector3.zero;
        if (scale == null)
            scale = type.blockSize;
        //GameObject block = new GameObject(type.blockName);
        GameObject block = new GameObject(type.blockName + " " + x + " " + y + " " + z, typeof(MeshFilter), typeof(MeshRenderer));
        block.layer = 6;

        block.transform.position = new Vector3(x + 0.5f, y, z + 0.5f);
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
        }

        blocks[x, z] = block;
    }
    public GameObject SpawnEntity(float x, float z, string tag)
    {
        var entity = new GameObject();

        return entity;
    }
    public void PlaceBlock(Vector3 coord, BlockClass type, Vector3? rotation = null, Vector3? scale = null) => PlaceBlock((int)coord.x, coord.y, (int)coord.z, type, rotation, scale);
    public GameObject GetBlockObj(int x, int y) => blocks[x, y];
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
        Grid<pixelNode> analisedPixels = new Grid<pixelNode>(texture.width, texture.height, (Grid<pixelNode> g, int x, int y) => new pixelNode(x, y));
        Color[,] colors = new Color[texture.width, texture.height];
        int _space = space + 1;

        Color32 undefinedColor = new Color32(64, 64, 64, 255);
        var currentID = 0;
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

                    var room = new RoomNode(currentID, new Vector2Int(x, y), -Left + Right + 2, -Down + Up + 2);

                    room.LeftDownCornerPosition = new Vector2Int(Left + x - 2, Down + y - 2);
                    room.RightUpCornerPosition = new Vector2Int(Right + x + 1, Up + y + 1);

                    rooms.Add(room);
                    Debug.Log(room.ToString());

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
    private struct pixelNode
    {
        int x, y;
        bool analised;
        public pixelNode(int x, int y)
        {
            this.x = x;
            this.y = y;
            analised = false;
        }
    }
}
