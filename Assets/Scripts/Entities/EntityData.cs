using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CameraControl;
using static GameManagerSystem.GameManager;
using static GamePlayer;
using static NavMeshUpdate;
using ObjectUtils;
using ItemSystem;
using HabilitySystem;
using ProjectileSystem;

using UnityEngine.Events;
using UnityEngine.AI;
using System.Linq;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UIElements;

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
        public int resistance = 1;
        public int intelligence = 1;
        public int defense = 1;
        public float speed = 2.5f;
        public float attackSpeedMultipliyer = 1f;

        [SerializeReference]
        public Dictionary<string, HabilityBehaviour> habilities = new Dictionary<string, HabilityBehaviour>();

        [Header("Valores máximos")]
        public int maxKarma;
        public int maxHealth;
        public int maxStamina;
        public int maxMana;

        [Header("Status atual")]
        public int currentKarma;
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
        public bool canAttack = true;
        public void SetMaxKarma() => maxKarma = level * 100 * (int)PlayerPreferences.Difficulty;
        public void LevelUp()
        {
        repeat:;
            if (currentKarma >= maxKarma)
            {
                currentKarma -= maxKarma;
                level++;
                SetMaxKarma();
                CalculateStatus();
                if (gameObject.GetComponent<IEntity>().GetType() == typeof(GamePlayer))
                {
                    gameObject.GetComponent<GamePlayer>().disponiblePoints++;
                }
                goto repeat;
            }
        }
        public void ResetStatus()
        {
            currentHealth = maxHealth;
            currentKarma = 0;
            currentMana = maxMana;
            currentStamina = maxStamina;

            currentStrength = strength;
            currentResistence = resistance;
            currentIntelligence = intelligence;
            currentDefense = defense;
            currentSpeed = speed;
        }
        public void ResetAttributesStatus()
        {
            currentStrength = strength;
            currentResistence = resistance;
            currentIntelligence = intelligence;
            currentDefense = defense;
            currentSpeed = speed;
            attackSpeedMultipliyer = 1f;
        }
        /// <summary>
        /// Calcula os status da entidade.
        /// </summary>
        public void CalculateStatus()
        {
            SetMaxKarma();
            maxHealth = (resistance * 10) + (level * 4) + 10;
            maxStamina = (resistance + strength) + (level * 2) + 5;
            maxMana = (intelligence * 10) + (level * 4) + 5;
        }
        /// <summary>
        /// Gasta stamina, valores negativos aumentam a stamina.
        /// </summary>
        /// <param name="value">Valor que será retirado/adicionado</param>
        public void WasteStamina(int value)
        {
            var val = currentStamina - value;
            currentStamina = Mathf.Min(Mathf.Max(val, 0), maxStamina);
        }
        public bool CanWasteStamina(int value) => currentStamina - value >= 0;
        /// <summary>
        /// Gasta mana, valores negativos aumentam a mana.
        /// </summary>
        /// <param name="value">Valor que será retirado/adicionado</param>
        public void WasteMana(int value)
        {
            var val = currentMana - value;
            currentMana = Mathf.Min(Mathf.Max(val, 0), maxMana);
        }
        public bool CanWasteMana(int value) => currentMana - value >= 0;
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
            return new DamageData(gameObject, CalculateDamage(dmg), MathEx.RadianToVector2(angle) * currentAttackItem.impulse, currentAttackItem.effects, currentAttackItem.staminaDamage, currentAttackItem.manaDamage, ignoreDefense);
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
        public void GiveEffect(params Effect[] effects)
        {
            foreach (var effect in effects)
            {
                GiveEffect(new Effect(effect));
            }
        }
        public void GiveEffects(Effect[] effects)
        {
            foreach (var effect in effects)
            {
                GiveEffect(new Effect(effect));
            }
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
        public GameObject effectVFX;
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
        public int manaDamage;
        public int staminaDamage;
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
            manaDamage = 0;
            staminaDamage = 0;
        }
        public DamageData(GameObject sender, int damage, Vector2 impulse, bool ignoreDefense = false)
        {
            this.sender = sender;
            this.damage = damage;
            this.impulse = impulse;
            effects = new List<Effect>();
            this.ignoreDefense = ignoreDefense;
            manaDamage = 0;
            staminaDamage = 0;
        }
        public DamageData(int damage, Vector2 impulse, bool ignoreDefense = false)
        {
            this.sender = null;
            this.damage = damage;
            this.impulse = impulse;
            this.effects = new List<Effect>();
            this.ignoreDefense = ignoreDefense;
            manaDamage = 0;
            staminaDamage = 0;
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
        public DamageData(GameObject sender, int damage, Vector2 impulse, List<Effect> effects, int staminaDamage = 0, int manaDamage = 0, bool ignoreDefense = false) : this(sender, damage, impulse)
        {
            this.effects = effects;
            this.ignoreDefense = ignoreDefense;
            this.staminaDamage = staminaDamage;
            this.manaDamage = manaDamage;
        }
    }
    /// <summary>
    /// Evento de dano, é chamado quando a entidade leva dano, ele devolve a entityData da entidade que levou dano e o gameObject que deu o dano, as informações do dano dado e o total de dano causado.
    /// </summary>
    [Serializable]
    public class DamageEvent : UnityEvent<EntityData, GameObject, DamageData, int> { }
    /// <summary>
    /// Evento de morte, é chamado quando a entidade morre, ele devolve a entityData da entidade que morreu e o gameObject que o matou.
    /// </summary>
    [Serializable]
    public class DeathEvent : UnityEvent<EntityData, GameObject> { }
    public interface IEntity
    {
        public const float DEFAULT_SHOT_Y_POSITION = 1f;
        public EntityData EntityData { get; set; }
        public DeathEvent OnDeathEvent { get; set; }
        public DamageEvent OnDamageEvent { get; set; }
        public void Damage(DamageData damageData);
        public void Attack();
        public void Die(GameObject killer);
        public void SetItem(Item item = null);
        public void CalculateStatusRegen();
    }
    public abstract class EntityBehaviour : MonoBehaviour
    {
        public Vector3 gizmoPos { get; set; }
        public bool RayCastTargetIsBehindWall(GameObject target)
        {
            if (target == null) return false;
            RaycastHit hit;
            Vector3 direction = MathEx.AngleVectors(target.transform.position, transform.position);
            direction = new Vector3(direction.x, 0, direction.y);
            Ray ray = new(transform.position, direction);
            Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Wall"), QueryTriggerInteraction.Ignore);
            gizmoPos = hit.point;
            return Vector3.Distance(hit.point, transform.position) < Vector3.Distance(transform.position, target.transform.position);
        }
        public Vector3 RayCastWall(GameObject target)
        {
            if (target == null) return Vector3.zero;
            else if (target.layer != 6) return Vector3.zero;
            RaycastHit hit;
            Vector3 direction = MathEx.AngleVectors(target.transform.position, transform.position);
            direction = new Vector3(direction.x, 0, direction.y);
            Ray ray = new(transform.position, direction);
            Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Wall"), QueryTriggerInteraction.Ignore);
            return hit.point;
        }
    }
    public abstract class BaseEntityBehaviour : EntityBehaviour, IEntity
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

        public Rigidbody RB => GetComponent<Rigidbody>();
        public Collider Collid => GetComponent<Collider>();
        public NavMeshAgent Agent => gameObject.GetComponent<NavMeshAgent>();
        public GameObject SpriteObj => GameObjectGeneral.GetGameObject(gameObject, "SpriteObject");
        public GameObject ItemSpriteOffset => GameObjectGeneral.GetGameObject(gameObject, "SpriteObject\\ItemOffset");
        public GameObject ItemSpriteObj => GameObjectGeneral.GetGameObject(gameObject, "SpriteObject\\ItemOffset\\Item");
        public SpriteRenderer SpriteRenderer => SpriteObj.GetComponent<SpriteRenderer>();
        public SpriteRenderer ItemSpriteRenderer => ItemSpriteObj.GetComponent<SpriteRenderer>();
        public Animator ItemSpriteAnimator => ItemSpriteObj.GetComponent<Animator>();
        public abstract void Damage(DamageData damageData);
        public abstract void Attack();
        public abstract void Die(GameObject killer);
        public abstract void SetItem(Item item = null);
        public abstract void CalculateStatusRegen();
    }
    public abstract class BasicEntityBehaviour : BaseEntityBehaviour, IEntity
    {
        public GameObject AttackArea => transform.Find("AttackArea").gameObject;

        public virtual void OnValidate()
        {
            EntityData.gameObject = gameObject;
        }
        public virtual void StartEntity(string targetName)
        {
            EntityData.attackReloaded = true;

            MainCameraControl.spriteRenderers.Add(SpriteObj);
            EntityData.name = gameObject.name;

            EntityData.gameObject = gameObject;
            EntityData.target = GameObject.Find(targetName);

            EntityData.CalculateStatus();
            EntityData.ResetStatus();

            SetItem(EntityData.currentAttackItem);
            AttackTimer();
        }
        public override void SetItem(Item item = null)
        {
            if (item == null)
            {
                EntityData.currentAttackItem = null;
                ItemSpriteRenderer.sprite = null;
                ItemSpriteAnimator.runtimeAnimatorController = null;
                ItemSpriteOffset.transform.localPosition = new Vector3(0, 0, 0);
            }
            else
            {
                EntityData.currentAttackItem = item;
                ItemSpriteRenderer.sprite = EntityData.currentAttackItem.itemSprite;
                ItemSpriteAnimator.runtimeAnimatorController = item.animatorController;
                ItemSpriteOffset.transform.localPosition = (Vector3)item.positionOffSet + new Vector3(0, 0, -0.01f);
            }
        }
        public void Awake()
        {
            StartEntity("Player");
        }
        public void Update()
        {
            Agent.isStopped = !EntityData.dead;
            Agent.speed = 0;

            if (EntityData.dead)
                return;
            AttackArea.transform.localScale = new Vector3(EntityData.currentAttackItem.attackwidth, EntityData.currentAttackItem.attackDistance, 1);

            FieldOfView();

            if (Agent.velocity.x > 0.1f || Agent.velocity.x < -0.1f)
                SpriteObj.transform.localScale = Agent.destination.x < transform.position.x ? new Vector3(-1, 1, 1) : Vector3.one;

            if (Agent.isOnNavMesh && EntityData.target != null)
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
            
            if (EntityData.target != null)
            {
                var atkRotation = MathEx.AngleRadian(AttackArea.transform.position, EntityData.target.transform.position);
                AttackArea.transform.rotation = Quaternion.Euler(-90, 0, (-atkRotation * Mathf.Rad2Deg) + 90);

                switch (EntityData.currentAttackItem.type)
                {
                    case ItemType.MeleeWeapon:
                        AttackArea.transform.rotation = Quaternion.Euler(-90, 0, (-atkRotation * Mathf.Rad2Deg) + 90);
                        AttackArea.SetActive(true);
                        break;
                    case ItemType.RangedWeapon:
                        AttackArea.SetActive(false);
                        break;
                    case ItemType.CustomWeapon:

                        break;
                }
            }

            /*Vector3 direction = EntityData.target.transform.position - transform.position;
            SpriteRenderer.flipX = direction.x < 0;*/
        }
        public void FixedUpdate()
        {
            if (EntityData.dead)
                return;
            CalculateStatusRegen();
            Attack();
            EntityData.currentImpulse -= new Vector2(EntityData.currentImpulse.x * Time.fixedDeltaTime * 5f, EntityData.currentImpulse.y * Time.fixedDeltaTime * 5f);
            RB.velocity = -new Vector3(EntityData.currentImpulse.x, 0, EntityData.currentImpulse.y);
        }
        public override void Damage(DamageData damageData)
        {
            if (EntityData.target == null && (damageData.sender.layer == 8 || damageData.sender.layer == 9))
                EntityData.target = damageData.sender;

            int total = !damageData.ignoreDefense ? Mathf.Max(damageData.damage - EntityData.CalculateDefense(), 0) : damageData.damage;

            EntityData.currentHealth -= total;
            EntityData.WasteMana(damageData.manaDamage);
            EntityData.WasteStamina(damageData.staminaDamage);
            EntityData.currentImpulse += damageData.impulse;

            EntityData.GiveEffects(damageData.effects);

            StopCoroutine(SetDamageColor());
            StartCoroutine(SetDamageColor());
            OnDamageEvent.Invoke(EntityData, damageData.sender, damageData, total);

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

        public override void Attack()
        {
            if (!EntityData.attackReloaded || !EntityData.inRange || RayCastTargetIsBehindWall(EntityData.target))
                return;
            if (EntityData.currentAttackItem != null)
            {
                if (EntityData.currentAttackItem.muzzlePrefab != null)
                    Instantiate(EntityData.currentAttackItem.muzzlePrefab, transform.position, Quaternion.Euler(0, (-MathEx.AngleRadian(transform.position, EntityData.target.transform.position) * Mathf.Rad2Deg) - 90, 0));

                switch (EntityData.currentAttackItem.type)
                {
                    case ItemType.None:

                        break;
                    case ItemType.MeleeWeapon:

                        foreach (var entity in AttackArea.GetComponent<ColliderNutshell>().triggerList.ToList())
                        {
                            if ((entity.layer == 6 || entity.layer == 8) && entity.GetComponent<IEntity>() != null)
                                entity.GetComponent<IEntity>().Damage(EntityData.AttackWithItem(MathEx.AngleRadian(transform.position, entity.transform.position)));
                        }

                        /*var colliders = Physics.OverlapSphere(transform.position, (EntityData.attackDistance + EntityData.currentAttackItem.attackDistance + 0.2f), LayerMask.GetMask("Player"));
                        if (colliders.Length == 0)
                            break;

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
                        }*/

                        break;
                    case ItemType.RangedWeapon:
                        var bullet = Instantiate(EntityData.currentAttackItem.bulletPrefab, new Vector3(transform.position.x, IEntity.DEFAULT_SHOT_Y_POSITION, transform.position.z), Quaternion.Euler(0, (-MathEx.AngleRadian(transform.position, EntityData.target.transform.position) * Mathf.Rad2Deg) - 90, 0));
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
            StartCoroutine(AttackAnimC());
            IEnumerator AttackAnimC()
            {
                if (ItemSpriteAnimator == null || ItemSpriteAnimator.runtimeAnimatorController == null)
                    yield break;
                ItemSpriteAnimator.Play("Attack");
                yield return new WaitForSeconds(ItemSpriteAnimator.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);
                ItemSpriteAnimator.Play("Default");
            }
        }
        private IEnumerator AttackTimer()
        {
            EntityData.attackReloaded = false;
            var currentTime = 0f;
            while (currentTime < EntityData.currentAttackItem.reloadTime + EntityData.attackDelay)
            {
                if (EntityData.currentAttackItem != null)
                {
                    currentTime += Time.deltaTime * EntityData.attackSpeedMultipliyer;
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    currentTime += Time.deltaTime * EntityData.attackSpeedMultipliyer;
                    yield return new WaitForEndOfFrame();
                }
            }
            EntityData.attackReloaded = true;
        }
        public void FieldOfView()
        {
            Agent.speed = EntityData.currentSpeed;

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

            Agent.stoppingDistance = EntityData.currentAttackItem.attackDistance;
            EntityData.inRange = Vector3.Distance(transform.position, EntityData.target.transform.position) <= EntityData.currentAttackItem.attackDistance + 0.3f;

            Agent.isStopped = !EntityData.canMove;
        }
        public override void Die(GameObject killer)
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
            RB.constraints = RigidbodyConstraints.FreezeAll;
            StopCoroutine(DieAnimC());
            StartCoroutine(DieAnimC());
            IEnumerator DieAnimC()
            {
                SpriteObj.GetComponent<Animator>().SetBool("Dead", true);
                SpriteObj.GetComponent<Animator>().Play("Dead");
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(SpriteObj.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);
                Destroy(gameObject);
            }
        }
        public void OnDestroy()
        {
            MainCameraControl.spriteRenderers.Remove(SpriteObj);
        }
        public void OnMouseDown()
        {
            player.EntityData.target = gameObject;
        }
        private float manaTimer;
        private float staminaTimer;
        public override void CalculateStatusRegen()
        {
            manaTimer += Time.fixedDeltaTime;
            staminaTimer += Time.fixedDeltaTime;

            if (manaTimer >= 20f / (EntityData.level * EntityData.currentIntelligence))
            {
                manaTimer = 0;
                EntityData.WasteMana(-EntityData.currentIntelligence);
            }
            if (staminaTimer >= 20f / (EntityData.level * EntityData.currentStrength))
            {
                staminaTimer = 0;
                EntityData.WasteStamina(-EntityData.currentStrength);
            }
        }
    }
    public interface IBullet
    {
        public GameObject sender { get; set; }
        public IEntity senderEntity => sender.GetComponent<IEntity>();
        public GameObject explosionPrefab { get; set; }
        public bool started { get; set; }
        public void SetBullet(GameObject sender, float? radAngles = null);
        public void DeathEffect();
        public DamageData damageData { get; set; }
        public bool entityDamageAdd { get; set; }
    }
    public abstract class BaseBulletBehaviour : EntityBehaviour, IBullet
    {
        [SerializeField]
        private GameObject _sender;
        [SerializeField]
        private GameObject _explosionPrefab;
        public IEntity senderEntity => sender.GetComponent<IEntity>();

        private bool _started = false;

        public int minDamage = 1;
        public int maxDamage = 1;
        public float speed = 10f;
        public float impulseForce;
        public List<Effect> effects = new List<Effect>();
        [SerializeField]
        private bool _entityDamageAdd;
        public bool entityDamageAdd { get => _entityDamageAdd; set => _entityDamageAdd = value; }

        public GameObject sender { get => _sender; set => _sender = value; }
        public GameObject explosionPrefab { get => _explosionPrefab; set => _explosionPrefab = value; }
        public bool started { get => _started; set => _started = value; }
        public DamageData damageData { get; set; }
        public bool destroyingProcess = false;

        public virtual void SetBullet(GameObject sender, float? radAngles = null)
        {
            //Debug.Log((float)radAngles);
            this.sender = sender;
            gameObject.layer = sender.layer == 8 ? 12 : sender.layer == 10 ? 11 : 0;
            if (radAngles != null)
                transform.rotation = Quaternion.Euler(0, (float)radAngles * Mathf.Rad2Deg, 0);
            started = true;
        }
        public virtual void DeathEffect()
        {
            var expObj = Instantiate(explosionPrefab);
            expObj.transform.position = transform.position;
        }
        protected virtual IEnumerator AutoDestructionCoroutine(float timer = 0)
        {
            yield return new WaitForSeconds(0.2f);
            Destroy(gameObject);
        }
        public virtual void AutoDestruction()
        {
            destroyingProcess = true;
            StartCoroutine(AutoDestructionCoroutine());
        }
    }
}
