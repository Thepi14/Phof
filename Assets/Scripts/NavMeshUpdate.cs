using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshUpdate : MonoBehaviour
{
    [SerializeField]
    private NavMeshSurface surface;
    public static NavMeshUpdate navMeshUpdateInstance;
    /// <summary>
    /// Hahaha
    /// </summary>
    public bool updateNavMesh
    {
        set
        {
            if (value)
                BuildNavMesh();
            updateNavMesh = false;
        }
    }
    public void Start()
    {
        navMeshUpdateInstance = this;
    }
    /// <summary>
    /// Atualiza o navmesh.
    /// </summary>
    public void BuildNavMesh()
    {
        surface.BuildNavMesh();
    }
    //SIM EU SEI ISSO É UMA GAMBIARRA
    //mas ninguém pode me impedir ha (só o Marcos)
    //para contexto, eu tava testando esse projeto na escola e estava funcionando normalmente,
    //mas quando eu coloquei ele na minha casa ele só não funcionou, então eu resolvi da minha forma.
    //evitei de colocar no Update porque trava muito.
    //O projeto que eu peguei é do commit "NavMesh aqui prr".
    //-- Jão
}
