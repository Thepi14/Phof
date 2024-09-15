using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ItemSystem;
using static GameManagerSystem.GameManager;
using static GamePlayer;
using static NavMeshUpdate;

using UnityEngine.Events;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

namespace EntityDataSystem
{
    [Serializable]
    public class EntityData
    {
        [HideInInspector]
        public GameObject gameObject;

        [Header("Nome da entidade haha")]
        public string name;

        [Header("Atributos")]
        public int level = 1;
        public int strength = 1;
        public int resistence = 1;
        public int intelligence = 1;
        public int defense = 1;
        public float speed = 1f;
        public float attackSpeedMultipliyer = 1f;

        [Header("Valores máximos")]
        public int maxHealth;
        public int maxStamina;
        public int maxMana;

        [Header("Status atual")]
        public int currentHealth;
        public int currentStamina;
        public int currentMana;

        [Header("Atributos atuais")]
        public int currentStrength;
        public int currentResistence;
        public int currentIntelligence;
        public int currentDefense;
        public float currentSpeed;
        public float currentAttackSpeedMultipliyer;

        public Vector2 currentImpulse;

        public List<Effect> currentEffects = new List<Effect>();

        public Item currentAttackItem;

        [Header("Combate")]
        public float attackDistance = 0.5f;
        public float attackDelay = 1;
        public float visionRadius = 10f;
        public bool prioritizePlayer = false;
        public bool inCombat = false;
        public GameObject target = null;
        public bool attackReloaded = true;
        public bool canMove = true;
        public bool inRange = false;

        public void ResetStatus()
        {
            currentHealth = maxHealth;
            currentMana = maxMana;
            currentStamina = maxStamina;
        }
        /// <summary>
        /// Calcula os status da entidade.
        /// </summary>
        public void CalculateStatus()
        {
            maxHealth = (resistence * 10) + (level * 4) + 10;
            maxStamina = (resistence + strength) + (level * 2) + 5;
            maxMana = (intelligence * 10) + (level * 4) + 5;
        }

        /// <summary>
        /// Calcula o dano que a entidade irá receber baseado nas características dela e no dano da arma.
        /// </summary>
        /// <param name="weaponDamage">Dano da arma.</param>
        /// <returns>A quantidade de dano.</returns>
        public int CalculateDamage(int weaponDamage = 0)
        {
            int result = (currentStrength * 2) + (weaponDamage * 2) + (level * 3) + UnityEngine.Random.Range(1, 20);
            return result;
        }
        /// <summary>
        /// Calcula a defesa baseado nas características da entidade e na defesa da armadura.
        /// </summary>
        /// <param name="armorDefense"></param>
        /// <returns>O valor da defesa.</returns>
        public int CalculateDefense(int armorDefense = 0)
        {
            int result = (currentResistence * 2) + (level * 3) + armorDefense;
            return result;
        }
        /// <summary>
        /// Ataca com item.
        /// </summary>
        /// <param name="angle">Ângulo em radians</param>
        /// <param name="ignoreDefense"></param>
        /// <returns></returns>
        public DamageData AttackWithItem(float angle, bool ignoreDefense = false)
        {
            int dmg = UnityEngine.Random.Range(currentAttackItem.minDamage, currentAttackItem.maxDamage);
            return new DamageData(gameObject, CalculateDamage(dmg), MathEx.RadianToVector2(angle) * currentAttackItem.impulse, currentAttackItem.effects, ignoreDefense);
        }
    }
    [Serializable]
    public class Effect
    {
        public string name;
        public byte level;
        public float duration;
        public float tickDelay;
        public float currentTick;

        public Effect(string name, float duration, float tickDelay, byte level)
        {
            this.name = name;
            this.duration = duration;
            this.tickDelay = tickDelay;
            this.level = level;
        }
    }
    [Serializable]
    public struct DamageData
    {
        public GameObject sender;

        public int damage;
        public Vector2 impulse;

        public List<Effect> effects;
        public bool ignoreDefense;

        public DamageData(GameObject sender, int damage, Vector2 impulse)
        {
            this.sender = sender;
            this.damage = damage;
            this.impulse = impulse;
            effects = new List<Effect>();
            ignoreDefense = false;
        }
        public DamageData(GameObject sender, int damage, Vector2 impulse, List<Effect> effects) : this(sender, damage, impulse)
        {
            this.effects = effects;
        }
        public DamageData(GameObject sender, int damage, Vector2 impulse, List<Effect> effects, bool ignoreDefense) : this(sender, damage, impulse, effects)
        {
            this.ignoreDefense = ignoreDefense;
        }
    }
    /// <summary>
    /// Evento de dano, é chamado quando a entidade leva dano, ele devolve a entityData da entidade que levou dano e o gameObject e deu o dano.
    /// </summary>
    public class DamageEvent : UnityEvent<EntityData, GameObject> { }
    /// <summary>
    /// Evento de morte, é chamado quando a entidade morre, ele devolve a entityData da entidade que morreu e o gameObject e matou.
    /// </summary>
    public class DeathEvent : UnityEvent<EntityData, GameObject> { }
    public interface IEntity
    {
        public DeathEvent OnDeathEvent { get; set; }
        public DamageEvent OnDamageEvent { get; set; }
        public EntityData EntityData { get; set; }
        public void Die(GameObject killer);

        public void Damage(DamageData damageData)
        {
            OnDamageEvent.Invoke(EntityData, damageData.sender);

            int total = !damageData.ignoreDefense ? Mathf.Max(damageData.damage - EntityData.CalculateDefense(), 0) : damageData.damage;

            EntityData.currentHealth -= total;

            if (EntityData.currentHealth <= 0)
            {
                Die(damageData.sender);
            }
        }
        public void Attack();
    }
    public abstract class BasicEntityBehaviour : MonoBehaviour, IEntity
    {
        public Transform target;

        [SerializeField]
        private DeathEvent _onDeathEvent;
        public DeathEvent OnDeathEvent { get => _onDeathEvent; set => _onDeathEvent = value; }

        [SerializeField]
        private DamageEvent _onDamageEvent;
        public DamageEvent OnDamageEvent { get => _onDamageEvent; set => _onDamageEvent = value; }

        [SerializeField]
        private EntityData _entityData;
        public EntityData EntityData { get => _entityData; set => _entityData = value; }

        private Rigidbody RB => GetComponent<Rigidbody>();
        public NavMeshAgent Agent => gameObject.GetComponent<NavMeshAgent>();
        public GameObject SpriteObj => transform.Find("SpriteObject").gameObject;
        public SpriteRenderer SpriteRenderer => SpriteObj.GetComponent<SpriteRenderer>();


        private void OnValidate()
        {
            EntityData.gameObject = gameObject;
        }
        public void StartEntity(string targetName)
        {
            gameManagerInstance.entities.Add(gameObject);

            EntityData.gameObject = gameObject;
            target = GameObject.Find(targetName).transform;

            EntityData.CalculateStatus();
            EntityData.ResetStatus();

            Agent.speed = EntityData.speed;
            Agent.stoppingDistance = EntityData.attackDistance;
        }
        public void Awake()
        {
            StartEntity("Player");
        }
        void Update()
        {
            if (Agent.isOnNavMesh)
            {
                Agent.SetDestination(target.position);
                Agent.isStopped = false;
            }

            Vector3 direction = target.position - transform.position;

            if (direction.x > 0)
            {
                SpriteRenderer.flipX = false;
            }
            else if (direction.x < 0)
            {
                SpriteRenderer.flipX = true;
            }

            FieldOfView();
        }
        private void FixedUpdate()
        {
            Attack();
        }

        public void Attack()
        {
            if (!EntityData.attackReloaded || !EntityData.inRange)
                return;
            if (EntityData.currentAttackItem != null)
            {
                switch (EntityData.currentAttackItem.type)
                {
                    case ItemType.None:

                        break;
                    case ItemType.MeleeWeapon:
                        var colliders = Physics.OverlapSphere(transform.position, (EntityData.attackDistance + 0.2f));
                        if (colliders.Length == 0)
                        {
                            Debug.Log("Atacou mas não tinha ninguém");
                            break;
                        }
                        GameObject target = colliders[0].gameObject;
                        foreach (var entity in colliders)
                        {
                            if (Vector3.Distance(transform.position, entity.transform.position) < Vector3.Distance(transform.position, target.transform.position) && target != entity)
                                target = entity.gameObject;
                        }
                        if (target.layer == 8)
                        {
                            target.gameObject.GetComponent<IEntity>().Damage(EntityData.AttackWithItem(MathEx.AngleRadian(transform.position, target.transform.position)));
                            Debug.Log("Atacou com item :)");
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case ItemType.RangedWeapon:
                        //Aqui Marcos
                        break;
                }
            }
            else
            {
                Debug.Log("Atacou, mas sem item :(");
                //Se não tiver item
            }
            StartCoroutine(AttackTimer());
        }

        private void FieldOfView()
        {
            if (EntityData.prioritizePlayer && Vector3.Distance(transform.position, player.transform.position) <= Vector3.Distance(transform.position, target.position))
            {
                target = player.transform;
            }
            if (Vector3.Distance(transform.position, target.position) <= EntityData.visionRadius / 2)
            {
                Agent.isStopped = false;
                EntityData.inRange = true;
                EntityData.inCombat = true;
            }
            else
            {
                Agent.isStopped = true;
                EntityData.inRange = false;
                EntityData.inCombat = false;
            }
            Agent.isStopped = !EntityData.canMove;
        }

        IEnumerator AttackTimer()
        {
            Debug.Log("Carregando...");
            EntityData.attackReloaded = false;
            if (EntityData.currentAttackItem != null)
                yield return new WaitForSeconds(EntityData.currentAttackItem.reloadTime + EntityData.attackDelay);
            else
                yield return new WaitForSeconds(EntityData.attackDelay);
            EntityData.attackReloaded = true;
            Debug.Log("Carregado");
        }

        public void Die(GameObject killer)
        {
            OnDeathEvent.Invoke(EntityData, killer);

            RB.velocity = Vector3.zero;
            EntityData.canMove = false;
            gameManagerInstance.entities.Remove(gameObject);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
