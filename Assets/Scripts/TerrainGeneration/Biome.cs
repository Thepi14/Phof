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
    public BlockClass doorBlock;
    public RuntimeAnimatorController doorAnimationController;

    public void OnValidate()
    {
        if (doorBlock != null && !doorBlock.isDoor)
        {
            doorBlock = null;
            Debug.LogWarning("A porta deve ter a variável \'isDoor\' ativada seu zé");
        }
    }
}
