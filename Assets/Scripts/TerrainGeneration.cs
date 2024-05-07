using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public static TerrainGeneration Instance;

    public BlockClass teste;

    public Texture2D map;

    public Mesh DEFAULT_MESH;
    public Material DEFAULT_MATERIAL;

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

    public GameObject[,] blocks;


    void Start()
    {
        Instance = this;

        blocks = new GameObject[MapWidth, MapHeight];

        map = new Texture2D(MapWidth, MapHeight);

        map = GenerateNoiseTexture(MapWidth, MapHeight, 87654, 0.1f, 0.5f, 0.01f);

        for (int x = 0; x < MapWidth; x++)
        {
            for (int z = 0; z < MapHeight; z++)
            {
                PlaceBlock(x, 0, z, teste);
            }
        }

        transform.parent.Find("Player").position = new Vector3(MapWidth / 2, 10, MapHeight / 2);
    }
    #region medo
    void Update()
    {
        
    }
    #endregion
    public void PlaceBlock(int x, int y, int z, BlockClass type)
    {
        //GameObject block = new GameObject(type.blockName);
        GameObject block = new GameObject(type.blockName + " " + x + " " + y + " " + z, typeof(MeshFilter), typeof(MeshRenderer));
        block.layer = 6;

        block.transform.position = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);

        block.GetComponent<MeshFilter>().mesh = type.mesh == null ? DEFAULT_MESH : type.mesh;

        block.GetComponent<MeshRenderer>().material = type.material == null ? DEFAULT_MATERIAL : type.material;

        block.transform.localScale = new Vector3(type.blockSize.x, type.blockSize.y, type.blockSize.z);

        if (type.hasCollider)
        {
            if (type.isBlock)
            {
                block.AddComponent<BoxCollider>();
                block.GetComponent<BoxCollider>().size = new Vector3(1.05f, 1, 1.05f);
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
    public bool IsInsideArray(int x, int y, dynamic[,] a) => x >= 0 && y >= 0 && x < a.GetLength(0) && y < a.GetLength(1);
    public void PlaceBlock(Vector3Int coord, BlockClass type)
    {
        PlaceBlock(coord.x, coord.y, coord.z, type);
    }
    public GameObject GetBlockObj(int x, int y) => blocks[x, y];
    public Texture2D GenerateNoiseTexture(int width, int height, float seed, float frequency, float limit, float scattering)
    {
        Texture2D noiseTexture = new Texture2D(width, height);
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                noiseTexture.SetPixel(x, y, new Color(v, v, v, 1));

                /*if (v + UnityEngine.Random.Range(-scattering, scattering) > limit)
                    noiseTexture.SetPixel(x, y, Color.white);
                else
                    noiseTexture.SetPixel(x, y, Color.black);*/
            }
        }
        noiseTexture.Apply();
        return noiseTexture;
    }
}
