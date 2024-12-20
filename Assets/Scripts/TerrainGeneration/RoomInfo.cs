// --------------------------------------------------------------------------------------------------------------------
/// <copyright file="RoomInfo.cs">
///   Copyright (c) 2024, Pi14 & Marcos Henrique, All rights reserved.
/// </copyright>
// --------------------------------------------------------------------------------------------------------------------
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
    public float entityDensity = 0.075f;
    public int size;
    public bool universal = false;
    [SerializeField]
    private List<BlockInfo> blocks = new List<BlockInfo>();
    private List<BlockInfo> previousBlocks = new List<BlockInfo>();
    [SerializeField]
    public Grid<BlockInfo> grid
    {
        get
        {
            if (!universal)
            {
                var newGrid = new Grid<BlockInfo>(size, size, (grid, x, y) => { return new BlockInfo(x, y); });
                newGrid.ListToGrid(blocks, size);
                return newGrid;
            }
            else
            {
                var newGrid = new Grid<BlockInfo>(size, size, (grid, x, y) => { return new BlockInfo(x, y); });
                return newGrid;
            }
        }
    }
    [HideInInspector]
    public bool changed = false;
    [SerializeField]
    [Tooltip("CUIDADO! Esse booleano vai resetar as configura��es da sala para o padr�o, ele n�o salva antes de excluir.")]
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

        size = Mathf.Max(size, 0);
        universal = name.ToLower() == "default";

        if (!universal)
        {
            if (resetRoom)
            {
                blocks.Clear();
            }
            if (blocks == null || blocks.Count == 0)
            {
                blocks = new List<BlockInfo>();
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        blocks.Add(new BlockInfo(x, y));
                    }
                }
            }
            if (grid.IsNull())
            {
                grid.ListToGrid(blocks, size);
            }
            //Room size changed
            if (grid.GetWidth() != size || grid.GetHeight() != size || grid.GridToList().Count != blocks.Count)
            {
                blocks = new List<BlockInfo>();
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        blocks.Add(new BlockInfo(x, y));
                    }
                }
                grid.ResizeGrid(size, size);
            }
            else
            {
                grid.ListToGrid(blocks, (x, y, i) => { blocks[i].x = x; blocks[i].y = y; return blocks[i]; }, size);
                string text = "";
                foreach (var blk in grid.GridToList()) text += blk.ToString() + ", ";
                //Debug.Log(text);
            }

            if (previousBlocks == null || previousBlocks.Count != blocks.Count || resetRoom)
                previousBlocks = blocks.ToList();

            foreach (var block in blocks)
            {
                block.name = "x " + block.x + ", y " + block.y;

                if (block.id == 0 && previousBlocks[blocks.IndexOf(block)].id != 0)
                    block.entityCanSpawn = true;
                else if (block.id != 0 && previousBlocks[blocks.IndexOf(block)].id == 0)
                    block.entityCanSpawn = false;

                if (block != previousBlocks[blocks.IndexOf(block)])
                {
                    previousBlocks[blocks.IndexOf(block)].id = block.id;
                }
            }
            changed = true;
        }
        else
        {
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
        public BlockInfo CopyID(BlockInfo block)
        {
            x = block.x;
            y = block.y;
            id = block.id;
            return this;
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
            if (a is null)
            {
                return b is null;
            }
            else if (b is null)
            {
                return a is null;
            }
            return a.x == b.x && a.y == b.y && a.id == b.id;
        }
        public static bool operator !=(BlockInfo a, BlockInfo b)
        {
            if (a is null)
            {
                return b is not null;
            }
            else if (b is null)
            {
                return a is not null;
            }
            return a.x != b.x || a.y != b.y || a.id != b.id;
        }
    }
}