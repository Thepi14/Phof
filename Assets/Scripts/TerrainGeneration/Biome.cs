using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FumoFumo", menuName = "Bioma/Novo Bioma")]
[Serializable]
public class Biome : ScriptableObject
{
    public List<BlockClass> groundBlocks;
    public List<BlockClass> wallBlocks;
    public List<BlockClass> pillarBlocks;
}
