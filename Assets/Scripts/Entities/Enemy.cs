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
        Gizmos.color = Color.red;
        if (EntityData.target != null)
        {
            Gizmos.DrawLine(transform.position, EntityData.target.transform.position);
            Gizmos.DrawSphere(gizmoPos, 0.25f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(gizmoPos, transform.position);
        }
    }
}