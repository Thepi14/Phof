using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GamePlayer;
using static TerrainGeneration;
using Pathfindingsystem;
using UnityEngine.AI;
using EntityDataSystem;
using GameManagerSystem;
using Unity.VisualScripting;


public class Enemy : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private LayerMask areaVisao;

    public GameObject player;
    public GamePlayer playerPlayer;
    public EntityData entityData;
    public SpriteRenderer spritePlayer;
    public int tipo = 0;
    public float impulseForce;
    public float kbCount;
    public enum Tipo {Normal, RangedPoison, Impulse, RangedFreeze }
    
    private Rigidbody RB => GetComponent<Rigidbody>();
    private SpriteRenderer sprite;
    private Pathfinding pathfinding;
    private List<PathNode> path;

    public bool inRange;

    [SerializeField]
    private float raio;

    public bool freezed = false;
    
    void Start()
    {
        target = GameObject.Find("Player").transform;
        player = GameObject.Find("Player");
        spritePlayer = GameObject.Find("SpriteObject").GetComponent<SpriteRenderer>();

        entityData.combatCoroutine = true;
        entityData.inCombat = false;
        
        entityData.maxHealth = GameManager.CalculateHealth(entityData);
        entityData.maxMana = GameManager.CalculateMana(entityData);
        entityData.maxStamina = GameManager.CalculateStamina(entityData);

        entityData.currentHealth = entityData.maxHealth;
        entityData.currentMana = entityData.maxMana;
        entityData.currentStamina = entityData.maxStamina;

<<<<<<< HEAD

=======
        entityData.damage = GameManager.CalculateDamage(entityData);
        entityData.damage =  GameManager.CalculateDamage(entityData);
>>>>>>> 31cc3995429256f62ebeae524e8610e23e3ae4e3
        entityData.defense = GameManager.CalculateDefense(entityData);
        playerPlayer = player.GetComponent<GamePlayer>();

        sprite = GetComponentInChildren<SpriteRenderer>();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = entityData.speed;
        agent.stoppingDistance = entityData.attackDistance;
    }

    private void FixedUpdate()
    {
        MeleeAttack();
    }
    void Update()
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }


        Vector3 direction = target.position - transform.position; 

        if(direction.x > 0)
        {
            sprite.flipX = false;
        }else if(direction.x < 0)
        {
            sprite.flipX = true;
        }

        CampoDeVisao();
        if (player.GetComponent<Rigidbody>().velocity == Vector3.zero && tipo == (int)Tipo.Normal)
            playerPlayer.entity.canMove = true;
        RangedAttack();
        Dead();
    }
    /// <summary>
    /// Move a entidade em direção a algo com uma velocidade usando pathfinding.
    /// </summary>
    /// <param name="target">Alvo</param>
    /// <param name="speed">Velocidade</param>

    void MeleeAttack()
    {
        
        if (entityData.inCombat && entityData.combatCoroutine && tipo == (int)Tipo.Normal)
        {
            entityData.damage = GameManager.CalculateDamage(entityData);
            ImpulseAttack();
            StartCoroutine("AttackTimer");
            player.GetComponent<GamePlayer>().entity.currentHealth -= entityData.damage - player.GetComponent<GamePlayer>().entity.defense;
            StartCoroutine("ColorTimer");

        }

    }

    void RangedAttack()
    {
        if (entityData.combatCoroutine && inRange)
        {
            if (tipo != (int)Tipo.Normal)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Shoot");
                GameObject bullet = GameObject.Instantiate(prefab);

                Vector3 position = transform.position;
                bullet.transform.position = new Vector3(position.x + 0.5f, position.y, position.z + 0.5f);

                Shoots shoots = bullet.AddComponent<Shoots>();
                shoots.entity = entityData;
                shoots.entityAlvo = player.GetComponent<GamePlayer>().entity;
                shoots.target = target.position;
                shoots.target2 = bullet.transform.position;
                Debug.Log(shoots.target);
                StartCoroutine("AttackTimer");

                switch (tipo)
                {
                    case (int)Tipo.RangedPoison:
                        shoots.tipo = 0;
                        break;
                    case (int)Tipo.RangedFreeze:
                        shoots.tipo = 1;
                        break;
                }
            }

        }

    }

    void ImpulseAttack()
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        player.GetComponent<Rigidbody>().AddRelativeForce(dir.normalized * impulseForce,ForceMode.Impulse);
        playerPlayer.entity.canMove = false;
        Debug.Log("Rodando");
        Debug.Log(player.GetComponent<Rigidbody>().velocity);
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
        entityData.multiAtaque++;

    }

    IEnumerator ColorTimer()
    {
        spritePlayer.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        spritePlayer.color = Color.white;
        yield return new WaitForSeconds(0.25f);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, raio);
    }

    private void CampoDeVisao()
    {
       Collider[] inVision = Physics.OverlapSphere(transform.position, raio, this.areaVisao);

     if (Physics.CheckSphere(transform.position, raio, this.areaVisao))
     {
        Debug.Log("Player Detectado novamente");
        agent.isStopped = false;
        inRange = true;
        Debug.Log("Tudo Funciona");
     }
    else
    {
        inRange = false;
        agent.isStopped = true;
        Debug.Log("Nada Funciona");
    }

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