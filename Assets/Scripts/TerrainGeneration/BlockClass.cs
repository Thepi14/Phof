using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FumoFumo", menuName = "Bloco/Admirável novo bloco")]
public class BlockClass : ScriptableObject
{
    [Header("Geral", order = 0)]
    public string blockName;
    public Mesh mesh;
    public Material material;
    [Header("Física", order = 1)]
    public Vector3 blockSize = Vector3.one;
    public Vector3 blockRotation = Vector3.zero;
    public bool isBlock = true;
    public bool hasCollider = true;
    public bool isDestructible = false;
    public int hitPoints = 0;
}
