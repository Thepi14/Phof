using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshUpdate : MonoBehaviour
{
    [SerializeField]
    private NavMeshSurface surface;

    public static NavMeshUpdate navMeshUpdateInstance;

    private void Start()
    {
        navMeshUpdateInstance = this;
    }
    public async void BuildNavMesh(int delay = 0)
    {
        await Task.Delay(delay);
        surface.BuildNavMesh();
    }
    //SIM EU SEI ISSO � UMA GAMBIARRA
    //mas ningu�m pode me impedir ha (s� o Marcos)
    //para contexto, eu tava testando esse projeto na escola e estava funcionando normalmente,
    //mas quando eu coloquei ele na minha casa ele s� n�o funcionou, ent�o eu resolvi da minha forma.
    //evitei de colocar no Update porque trava muito.
    //O projeto que eu peguei � do commit "NavMesh aqui prr".
    //-- J�o
}
