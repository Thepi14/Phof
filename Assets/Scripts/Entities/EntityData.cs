using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ItemSystem;
using static CameraControl;
using static GameManagerSystem.GameManager;
using static GamePlayer;
using static NavMeshUpdate;
using ObjectUtils;

using UnityEngine.Events;
using UnityEngine.AI;
using UnityEngine.Windows;
using ProjectileSystem;
using System.Linq;

namespace EntityDataSystem
{
    [Serializable]
    public class EntityData
    {
        public GameObject gameObject;

        [Header("Nome da entidade haha")]
        public string name;

        [Header("Atributos")]
        public int level = 1;
        public int strength = 1;
        public int resistence = 1;
        public int intelligence = 1;
        public int defense = 1;
        public float speed = 2.5f;
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
        public bool damaged = false;
        public bool dead = false;

        public void ResetStatus()
        {
            currentHealth = maxHealth;
            currentMana = maxMana;
            currentStamina = maxStamina;

            currentStrength = strength;
            currentResistence = resistence;
            currentIntelligence = intelligence;
            currentDefense = defense;
            currentSpeed = speed;
        }
        public void ResetAttributesStatus()
        {
            currentStrength = strength;
            currentResistence = resistence;
            currentIntelligence = intelligence;
            currentDefense = defense;
            currentSpeed = speed;
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
        public void GiveEffect(Effect effect)
        {
            bool thereIs = false;
            foreach (var Ceffect in currentEffects.ToList())
            {
                if (effect.name == Ceffect.name)
                {
                    thereIs = true;
                    break;
                }
            }
            //se há um efeito com o nome do effects[i]
            if (thereIs)
            {
                int i = 0;
                //iteração para cada efeito atual
                foreach (var Ceffect in currentEffects)
                {
                    //se o nome do efeito atual condiz com o que vai ser adicionado
                    if (Ceffect.name == effect.name)
                    {
                        if (Ceffect.stackable && effect.stackable)
                        {
                            Ceffect.level += 1;
                        }
                        else if (Ceffect.duration < effect.duration && effect.level >= Ceffect.level)
                        {
                            currentEffects[i] = new Effect(effect);
                        }
                    }
                    i++;
                    break;
                }
            }
            else
                currentEffects.Add(effect);
        }
        public void GiveEffects(List<Effect> effects)
        {
            foreach (var effect in effects)
            {
                GiveEffect(new Effect(effect));
            }
        }
    }
    [Serializable]
    public class Effect
    {
        public string name;
        public float duration = 3f;
        public float tickDelay = 0.5f;
        public byte level = 1;
        public float currentTick = 0;
        public bool stackable = false;
        [HideInInspector]
        public int frameAdded = 0;

        public Effect(string name, float duration, float tickDelay, byte level = 1, bool stackable = false)
        {
            this.name = name;
            this.duration = duration;
            this.tickDelay = tickDelay;
            this.level = level;
            this.stackable = stackable;
            frameAdded = Time.frameCount + 0;
        }
        public Effect(string name, float duration, float tickDelay, bool stackable, byte level = 1)
        {
            this.name = name;
            this.duration = duration;
            this.tickDelay = tickDelay;
            this.level = level;
            this.stackable = stackable;
            frameAdded = Time.frameCount + 0;
        }
        public Effect(Effect effect)
        {
            name = effect.name;
            duration = effect.duration;
            tickDelay = effect.tickDelay;
            level = effect.level;
            stackable = effect.stackable;
            frameAdded = Time.frameCount + 0;
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

        public DamageData(int damage, bool ignoreDefense = false)
        {
            sender = null;
            this.damage = damage;
            impulse = Vector2.zero;
            effects = new List<Effect>();
            this.ignoreDefense = ignoreDefense;
        }
        public DamageData(GameObject sender, int damage, Vector2 impulse, bool ignoreDefense = false)
        {
            this.sender = sender;
            this.damage = damage;
            this.impulse = impulse;
            effects = new List<Effect>();
            this.ignoreDefense = ignoreDefense;
        }
        public DamageData(int damage, Vector2 impulse, bool ignoreDefense = false)
        {
            this.sender = null;
            this.damage = damage;
            this.impulse = impulse;
            this.effects = new List<Effect>();
            this.ignoreDefense = ignoreDefense;
        }
        public DamageData(int damage, Vector2 impulse, List<Effect> effects, bool ignoreDefense = false) : this(damage, impulse)
        {
            this.effects = effects;
            this.ignoreDefense = ignoreDefense;
        }
        public DamageData(GameObject sender, int damage, Vector2 impulse, List<Effect> effects, bool ignoreDefense = false) : this(sender, damage, impulse)
        {
            this.effects = effects;
            this.ignoreDefense = ignoreDefense;
        }
    }
    /// <summary>
    /// Evento de dano, é chamado quando a entidade leva dano, ele devolve a entityData da entidade que levou dano e o gameObject e deu o dano.
    /// </summary>
    [Serializable]
    public class DamageEvent : UnityEvent<EntityData, GameObject> { }
    /// <summary>
    /// Evento de morte, é chamado quando a entidade morre, ele devolve a entityData da entidade que morreu e o gameObject e matou.
    /// </summary>
    [Serializable]
    public class DeathEvent : UnityEvent<EntityData, GameObject> { }
    public interface IEntity
    {
        public DeathEvent OnDeathEvent { get; set; }
        public DamageEvent OnDamageEvent { get; set; }
        public EntityData EntityData { get; set; }
        public void Damage(DamageData damageData);
        public void Attack();
        public void Die(GameObject killer);
    }
    public abstract class BasicEntityBehaviour : MonoBehaviour, IEntity
    {
        [SerializeField]
        private EntityData _entityData;
        public EntityData EntityData { get => _entityData; set => _entityData = value; }

        [SerializeField]
        private DeathEvent _onDeathEvent;
        public DeathEvent OnDeathEvent { get => _onDeathEvent; set => _onDeathEvent = value; }

        [SerializeField]
        private DamageEvent _onDamageEvent;
        public DamageEvent OnDamageEvent { get => _onDamageEvent; set => _onDamageEvent = value; }

        private Rigidbody RB => GetComponent<Rigidbody>();
        public NavMeshAgent Agent => gameObject.GetComponent<NavMeshAgent>();
        public GameObject SpriteObj => GameObjectGeneral.GetGameObject(gameObject, "SpriteObject");
        public GameObject ItemSpriteObj => GameObjectGeneral.GetGameObject(gameObject, "SpriteObject\\Item");
        public SpriteRenderer SpriteRenderer => SpriteObj.GetComponent<SpriteRenderer>();
        public SpriteRenderer ItemSpriteRenderer => ItemSpriteObj.GetComponent<SpriteRenderer>();

        public const float DEFAULT_SHOT_Y_POSITION = 1f;

        public void OnValidate()
        {
            EntityData.gameObject = gameObject;
            EntityData.attackReloaded = false;
        }
        public void StartEntity(string targetName)
        {
            EntityData.attackReloaded = false;

            MainCameraControl.spriteRenderers.Add(SpriteObj);
            EntityData.name = gameObject.name;
            gameManagerInstance.AddEntity(gameObject);

            EntityData.gameObject = gameObject;
            EntityData.target = GameObject.Find(targetName);

            EntityData.CalculateStatus();
            EntityData.ResetStatus();

            Agent.speed = EntityData.speed;
            Agent.stoppingDistance = EntityData.attackDistance;
            EntityData.attackReloaded = true;

            ItemSpriteRenderer.sprite = EntityData.currentAttackItem.itemSprite;

            AttackTimer();
        }
        public void Awake()
        {
            StartEntity("Player");
        }
        public void Update()
        {
            FieldOfView();

            if (Agent.velocity.x > 0.1f || Agent.velocity.x < -0.1f)
                SpriteObj.transform.localScale = Agent.destination.x < transform.position.x ? new Vector3(-1, 1, 1) : Vector3.one;

            if (Agent.isOnNavMesh)
            {
                Agent.SetDestination(EntityData.target.transform.position);
                Agent.isStopped = false;
            }

            if ((Agent.velocity.y != 0 || Agent.velocity.x != 0))
            {
                SpriteObj.GetComponent<Animator>().SetBool("Walking", true);
            }
            else
            {
                SpriteObj.GetComponent<Animator>().SetBool("Walking", false);
            }

            /*Vector3 direction = EntityData.target.transform.position - transform.position;
            SpriteRenderer.flipX = direction.x < 0;*/
        }
        public void FixedUpdate()
        {
            Attack();
            EntityData.currentImpulse *= 10 * Time.fixedDeltaTime;
            RB.velocity = EntityData.currentImpulse;
        }

        public void Damage(DamageData damageData)
        {
            OnDamageEvent.Invoke(EntityData, damageData.sender);

            int total = !damageData.ignoreDefense ? Mathf.Max(damageData.damage - EntityData.CalculateDefense(), 0) : damageData.damage;

            EntityData.currentHealth -= total;
            EntityData.currentImpulse += damageData.impulse;

            EntityData.GiveEffects(damageData.effects);

            StopCoroutine(SetDamageColor());
            StartCoroutine(SetDamageColor());

            if (EntityData.currentHealth <= 0)
            {
                Die(damageData.sender);
            }
        }

        private IEnumerator SetDamageColor()
        {
            EntityData.damaged = true;
            SpriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.3f);
            EntityData.damaged = false;
            SpriteRenderer.color = Color.white;
        }

        public void Attack()
        {
            if (!EntityData.attackReloaded || !EntityData.inRange)
                return;
            if (EntityData.currentAttackItem != null)
            {
                Instantiate(EntityData.currentAttackItem.muzzlePrefab, transform.position, Quaternion.Euler(0, (-MathEx.AngleRadian(transform.position, EntityData.target.transform.position) * Mathf.Rad2Deg) - 90, 0));
                switch (EntityData.currentAttackItem.type)
                {
                    case ItemType.None:

                        break;
                    case ItemType.MeleeWeapon:
                        var colliders = Physics.OverlapSphere(transform.position, (EntityData.attackDistance + EntityData.currentAttackItem.attackDistance + 0.2f), LayerMask.GetMask("Player"));
                        if (colliders.Length == 0)
                        {
                            break;
                        }
                        GameObject target = colliders[0].gameObject;
                        foreach (var entity in colliders)
                        {
                            if (Vector3.Distance(transform.position, entity.transform.position) < Vector3.Distance(transform.position, target.transform.position) && target != entity)
                                target = entity.gameObject;
                        }
                        if (target.gameObject.GetComponent<IEntity>() != null)
                        {
                            target.gameObject.GetComponent<IEntity>().Damage(EntityData.AttackWithItem(MathEx.AngleRadian(transform.position, target.transform.position)));
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case ItemType.RangedWeapon:
                        var bullet = Instantiate(EntityData.currentAttackItem.bulletPrefab, new Vector3(transform.position.x, DEFAULT_SHOT_Y_POSITION, transform.position.z), Quaternion.Euler(0, (-MathEx.AngleRadian(transform.position, EntityData.target.transform.position) * Mathf.Rad2Deg) - 90, 0));
                        bullet.GetComponent<IBullet>().SetBullet(gameObject);
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

        public void FieldOfView()
        {
            if (!gameObject.activeSelf)
                return;
            if (EntityData.target == null)
            {
                if (!EntityData.prioritizePlayer)
                {
                    foreach(var ally in gameManagerInstance.allies)
                    {
                        if (EntityData.prioritizePlayer && Vector3.Distance(transform.position, ally.transform.position) <= EntityData.visionRadius)
                        {
                            EntityData.target = ally;
                        }
                    }
                }
                else if (EntityData.prioritizePlayer && Vector3.Distance(transform.position, player.transform.position) <= EntityData.visionRadius)
                {
                    EntityData.target = player.gameObject;
                }
            }
            if (EntityData.target != null)
            {
                if (EntityData.prioritizePlayer && Vector3.Distance(transform.position, player.transform.position) <= Vector3.Distance(transform.position, EntityData.target.transform.position))
                {
                    EntityData.target = player.gameObject;
                }
            }
            if (EntityData.target == null)
                return;
            EntityData.inRange = Vector3.Distance(transform.position, EntityData.target.transform.position) <= EntityData.attackDistance + 0.3f;
            Agent.isStopped = !EntityData.canMove;
        }

        private IEnumerator AttackTimer()
        {
            EntityData.attackReloaded = false;
            if (EntityData.currentAttackItem != null)
                yield return new WaitForSeconds(EntityData.currentAttackItem.reloadTime + EntityData.attackDelay);
            else
                yield return new WaitForSeconds(EntityData.attackDelay);
            EntityData.attackReloaded = true;
        }

        public void Die(GameObject killer)
        {
            if (EntityData.dead)
                return;
            EntityData.dead = true;
            OnDeathEvent.Invoke(EntityData, killer);

            RB.velocity = Vector3.zero;
            EntityData.canMove = false;
            GetComponent<Collider>().enabled = false;
            //gameObject.SetActive(false);

            DieAnim();
        }
        public void DieAnim()
        {
            SpriteObj.GetComponent<Animator>().SetBool("Dead", true);
            SpriteObj.GetComponent<Animator>().Play("Dead");
            StartCoroutine(DieAnimC());
            IEnumerator DieAnimC()
            {
                for (float t = 0; t < SpriteObj.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length; t += Time.deltaTime)
                {
                    yield return new WaitForEndOfFrame();
                }
                Destroy(gameObject);
            }
        }

        public void OnDestroy()
        {
            MainCameraControl.spriteRenderers.Remove(SpriteObj);
            gameManagerInstance.entities.Remove(gameObject);
        }
    }
    public interface IBullet
    {
        public GameObject sender { get; set; }
        public GameObject explosionPrefab { get; set; }
        public ProjectileProperties projectileProperties { get; set; }
        public bool started { get; set; }
        public void SetBullet(GameObject sender, float? radAngles = null);
        public void DeathEffect();
        public DamageData damageData { get; set; }
    }
    public class BaseBulletBehaviour : MonoBehaviour, IBullet
    {
        [SerializeField]
        private GameObject _sender;
        [SerializeField]
        private GameObject _explosionPrefab;
        [SerializeField]
        private ProjectileProperties _projectileProperties;

        private bool _started = false;

        public GameObject sender { get => _sender; set => _sender = value; }
        public GameObject explosionPrefab { get => _explosionPrefab; set => _explosionPrefab = value; }
        public ProjectileProperties projectileProperties { get => _projectileProperties; set => _projectileProperties = value; }
        public bool started { get => _started; set => _started = value; }
        public DamageData damageData { get; set; }

        public void SetBullet(GameObject sender, float? radAngles = null)
        {
            //Debug.Log((float)radAngles);
            this.sender = sender;
            gameObject.layer = sender.layer == 8 ? 12 : sender.layer == 10 ? 11 : 0;
            if (radAngles != null)
                transform.rotation = Quaternion.Euler(0, (float)radAngles * Mathf.Rad2Deg, 0);
            started = true;
        }
        public void DeathEffect()
        {
            var expObj = Instantiate(explosionPrefab);
            expObj.transform.position = transform.position;
        }
    }
}
