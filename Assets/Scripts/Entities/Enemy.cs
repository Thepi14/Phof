using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.VisualScripting;

using static GamePlayer;
using static TerrainGeneration;
using Pathfindingsystem;
using EntityDataSystem;
using static GameManagerSystem.GameManager;
using ProjectileSystem;

public class Enemy : BasicEntityBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, (EntityData.attackDistance + 0.2f));
    }
}