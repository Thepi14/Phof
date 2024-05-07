using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FumoFumo", menuName = "Bloco/Admir�vel novo bloco")]
public class BlockClass : ScriptableObject
{
    [Header("Geral", order = 0)]
    public string blockName;
    public Mesh mesh;
    public Material material;
    [Header("F�sica", order = 1)]
    public Vector3Int blockSize = Vector3Int.one;
    public bool isBlock = true;
    public bool hasCollider = true;
    public bool isDestructible = false;
    public int hitPoints = 0;
}
