using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshUpdate : MonoBehaviour
{
    [SerializeField]
    private NavMeshSurface surface;
    void Update()
    {
        surface.BuildNavMesh();
    }
}
