using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EntityDataSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New room", menuName = "Bioma/Nova sala", order = 1)]
public class RoomInfo : ScriptableObject
{
    public List<GameObject> entities = new List<GameObject>();
    /// <summary>
    /// Calcula a densidade de entidades inimigas por quadrado.
    /// </summary>
    public float entityDensity = 0.1f;
    public Vector2Int size;
    public bool universal = false;
    [SerializeField]
    private List<BlockInfo> blocks = new List<BlockInfo>();
    private List<BlockInfo> previousBlocks = new List<BlockInfo>();
    public Grid<BlockInfo> grid;
    [HideInInspector]
    public bool changed = false;
    [SerializeField]
    [Tooltip("CUIDADO! Esse booleano vai resetar as configurações da sala para o padrão, ele não salva antes de excluir.")]
    private bool resetRoom = false;

    private void OnValidate()
    {
        foreach (GameObject obj in entities.ToList())
        {
            if (obj != null && obj.GetComponent<IEntity>() == null)
            {
                entities.Remove(obj);
                continue;
            }
        }

        size.x = Mathf.Max(size.x, 0);
        size.y = Mathf.Max(size.y, 0);
        universal = name.ToLower() == "default";

        if (!universal)
        {
            if (resetRoom)
            {
                grid = new Grid<BlockInfo>(size.x, size.y, (grid, x, y) => { return new BlockInfo(x, y); });
                blocks = grid.GridToList();
            }
            else if (grid.IsNull())
            {
                grid = new Grid<BlockInfo>(size.x, size.y, (grid, x, y) => { return new BlockInfo(x, y); });
                grid.ListToGrid(blocks, size.y);
            }
            //Room size changed
            if (grid.GetWidth() != size.x || grid.GetHeight() != size.y || grid.GridToList().Count != blocks.Count)
            {
                grid = new Grid<BlockInfo>(size.x, size.y, (grid, x, y) => { return new BlockInfo(x, y); });
                grid.ResizeGrid(size.x, size.y);
                blocks = grid.GridToList();
            }
            else
            {
                grid.ListToGrid(blocks, (x, y, i) => { blocks[i].x = x; blocks[i].y = y; return blocks[i]; }, size.x);
                string text = "";
                foreach (var blk in grid.GridToList()) text += blk.ToString() + ", ";
                //Debug.Log(text);
            }

            if (previousBlocks == null || previousBlocks.Count != blocks.Count || resetRoom)
                previousBlocks = blocks.ToList();

            foreach (var block in blocks)
            {
                block.name = "x " + block.x + ", y " + block.y;
                if (block.id != 0)
                    block.entityCanSpawn = false;
                else if (previousBlocks[blocks.IndexOf(block)].id != 0)
                    block.entityCanSpawn = true;

                if (block != previousBlocks[blocks.IndexOf(block)])
                {
                    previousBlocks[blocks.IndexOf(block)].id = block.id;
                }
            }
            changed = true;
        }
        else
        {
            grid = null;
            blocks = null;
            previousBlocks = null;
        }

        resetRoom = false;
    }
    public List<BlockInfo> RetrieveList() => blocks;
    [Serializable]
    public class BlockInfo
    {
        [HideInInspector]
        public string name;
        [HideInInspector]
        public int x;
        [HideInInspector]
        public int y;
        public byte id;
        public float height;
        public Vector3 rotation;
        public Vector3 scale;
        public bool entityCanSpawn = true;

        public BlockInfo(int x, int y)
        {
            this.x = x;
            this.y = y;
            id = 0;
            height = 0;
            scale = Vector3.one;
            rotation = Vector3.zero;
            entityCanSpawn = true;
        }

        public override bool Equals(object obj)
        {
            return obj is BlockInfo info &&
                   name == info.name &&
                   x == info.x &&
                   y == info.y &&
                   id == info.id &&
                   height == info.height &&
                   rotation.Equals(info.rotation) &&
                   scale.Equals(info.scale) &&
                   entityCanSpawn == info.entityCanSpawn;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, x, y, id, height, rotation, scale, entityCanSpawn);
        }

        public override string ToString()
        {
            return x + " " + y + " " + id;
        }

        public static bool operator ==(BlockInfo a, BlockInfo b)
        {
            return a.x == b.x && a.y == b.y && a.id == b.id;
        }

        public static bool operator !=(BlockInfo a, BlockInfo b)
        {
            return a.x != b.x || a.y != b.y || a.id != b.id;
        }
    }
}
