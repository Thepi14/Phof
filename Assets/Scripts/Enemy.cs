using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GamePlayer;
using static TerrainGeneration;
using Pathfindingsystem;
using UnityEngine.AI;
using GameManagerSystem;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private Transform target;

    public GameObject player;
    public EntityData entityData;
    
    private Rigidbody RB => GetComponent<Rigidbody>();
    private Pathfinding pathfinding;
    private List<PathNode> path;

    public bool freezed = false;
    
    void Start()
    {
        entityData.combatCoroutine = true;
        
        entityData.maxHealth = GameManager.CalculateHealth(entityData);
        entityData.maxMana = GameManager.CalculateMana(entityData);
        entityData.maxStamina = GameManager.CalculateStamina(entityData);

        entityData.currentHealth = entityData.maxHealth;
        entityData.currentMana = entityData.maxMana;
        entityData.currentStamina = entityData.maxStamina;

        entityData.damage = GameManager.CalculateDamage(entityData);
        entityData.damage =  GameManager.CalculateDamage(entityData);
        entityData.defense = GameManager.CalculateDefense(entityData);

        agent = GetComponent<NavMeshAgent>();
        agent.speed = entityData.speed;
        agent.stoppingDistance = entityData.attackDistance;
    }
    void Update()
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            Debug.Log("Agente não está no NavMesh");
        }
           
        Attack();
        Dead();
    }
    /// <summary>
    /// Move a entidade em direção a algo com uma velocidade usando pathfinding.
    /// </summary>
    /// <param name="target">Alvo</param>
    /// <param name="speed">Velocidade</param>

    void Attack()
    {
        if (entityData.inCombat && entityData.combatCoroutine)
        {
            StartCoroutine("AttackTimer");
            player.GetComponent<GamePlayer>().entity.currentHealth -= entityData.damage;

        }
    }

    void Dead()
    {
        if(entityData.currentHealth <= 0)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    IEnumerator AttackTimer()
    {
        entityData.combatCoroutine = false;
        yield return new WaitForSeconds(entityData.attackTimer);
        entityData.combatCoroutine = true;
    }
}


namespace UnityEngine
{
    public struct MathEx
    {
        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }
    }
}