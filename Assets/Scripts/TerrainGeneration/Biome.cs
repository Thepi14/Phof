using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FumoFumo", menuName = "Bioma/Novo Bioma")]
[Serializable]
public class Biome : ScriptableObject
{
    [Header("Blocks configuration", order = 0)]
    public List<BlockClass> groundBlocks;
    public List<BlockClass> wallBlocks;
    public List<BlockClass> pillarBlocks;
    [Header("Door configuration", order = 1)]
    public BlockClass doorBlock;
    public RuntimeAnimatorController doorAnimationController;
    [Header("General blocks configuration", order = 2)]
    public List<BlockClass> generalBlocks;

    [Header("Room configuration", order = 3)]
    public RoomInfo defaultRoom;
    public List<RoomInfo> rooms;

    public void OnValidate()
    {
        if (doorBlock != null && !doorBlock.isDoor)
        {
            doorBlock = null;
            Debug.LogWarning("A porta deve ter a variável \'isDoor\' ativada seu zé");
        }
    }
}
