// --------------------------------------------------------------------------------------------------------------------
/// <copyright file="BlockClass.cs">
///   Copyright (c) 2024, Pi14, All rights reserved.
/// </copyright>
// --------------------------------------------------------------------------------------------------------------------
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
    public bool isDoor = false;

    public void OnValidate()
    {
        if (isDoor)
        {

        }
    }
}
