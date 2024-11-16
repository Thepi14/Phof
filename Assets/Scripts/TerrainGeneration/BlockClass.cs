using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FumoFumo", menuName = "Bloco/Admirável novo bloco")]
public class BlockClass : ScriptableObject
{
    [Header("Geral", order = 0)]
    public string blockName;
    public GameObject blockPrefab;
    [Header("Física", order = 1)]
    public bool isBlock = true;
    public bool isDoor = false;
    public bool hasCollider = true;

    public void OnValidate()
    {
        if (isDoor)
        {
            isBlock = false;
            hasCollider = true;
        }
    }
}
