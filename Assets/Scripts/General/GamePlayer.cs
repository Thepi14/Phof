using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CameraControl;
using static TerrainGeneration;
using static GameManagerSystem.GameManager;
using EntityDataSystem;
using ObjectUtils;
using System.Linq;
using ItemSystem;
using System.Threading.Tasks;

public class GamePlayer : MonoBehaviour, IEntity
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

    #region Variáveis obrigatórias
    public static GamePlayer player;
    #endregion

    #region Status
    [Header("Status", order = 1)]

    [SerializeField]
    private Vector3 velocity;
    public Vector2 XZInput; //X vector (vertical) = Z input, Y vector (horizontal) = X inpu
    public Vector3 playerTarget;
    #endregion

    #region Variáveis pré-definidas
    public Rigidbody RB => GetComponent<Rigidbody>();
    public Collider Collid => GetComponent<Collider>();
    public GameObject GroundDetectorObj => transform.Find("GroundDetector").gameObject;
    public ColliderNutshell GroundDetector => GroundDetectorObj.GetComponent<ColliderNutshell>();
    public GameObject SpriteObj => GameObjectGeneral.GetGameObject(gameObject, "SpriteObject");
    public GameObject ItemSpriteOffset => GameObjectGeneral.GetGameObject(gameObject, "SpriteObject\\ItemOffset");
    public GameObject ItemSpriteObj => GameObjectGeneral.GetGameObject(gameObject, "SpriteObject\\ItemOffset\\Item");
    public SpriteRenderer SpriteRenderer => SpriteObj.GetComponent<SpriteRenderer>();
    public SpriteRenderer ItemSpriteRenderer => ItemSpriteObj.GetComponent<SpriteRenderer>();
    public Animator ItemSpriteAnimator => ItemSpriteObj.GetComponent<Animator>();
    public GameObject AttackArea => transform.Find("AttackArea").gameObject;

    #endregion

    public void OnValidate()
    {
        EntityData.gameObject = gameObject;
    }
    public void Awake()
    {
        EntityData.attackReloaded = true;

        player = this;
        EntityData.gameObject = gameObject;
        MainCameraControl.spriteRenderers.Add(SpriteObj);
        gameManagerInstance.entities.Add(gameObject);
        EntityData.CalculateStatus();
        EntityData.ResetStatus();

        transform.Find("ShadowObject").GetComponent<ShadowSeek>().shadowCastObject = gameObject;
        EntityData.canMove = true;

        MainCameraControl.focusObject = gameObject;

        ItemSpriteRenderer.sprite = EntityData.currentAttackItem.itemSprite;

        SetItem(EntityData.currentAttackItem);
        gameManagerInstance.allies.Add(gameObject);
    }
    public void FixedUpdate()
    {
        if (EntityData.dead)
            return;
        CalculateStatusRegen();

        XZInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * new Vector2(Mathf.Abs(Input.GetAxis("Horizontal")), Mathf.Abs(Input.GetAxis("Vertical")));

        if(EntityData.canMove && XZInput != new Vector2(0, 0))
            RB.velocity = new Vector3(XZInput.x * EntityData.currentSpeed, 0, XZInput.y * EntityData.currentSpeed);
        else
            RB.velocity = Vector3.zero;

        EntityData.currentImpulse -= new Vector2(EntityData.currentImpulse.x * Time.fixedDeltaTime * 5f, EntityData.currentImpulse.y * Time.fixedDeltaTime * 5f);
        RB.velocity -= new Vector3(EntityData.currentImpulse.x, 0, EntityData.currentImpulse.y);

        velocity = RB.velocity;
    }
    private bool attackHeldDown = false;
    private bool canAttackByHeld = true;
    public void Update()
    {
        if (EntityData.dead)
            return;

        if (RB.velocity.x > 0.1f || RB.velocity.x < -0.1f)
            SpriteObj.transform.localScale = XZInput.x < 0 ? new Vector3(-1, 1, 1) : Vector3.one;

        if ((XZInput.y != 0 || XZInput.x != 0))
        {
            SpriteObj.GetComponent<Animator>().SetBool("Walking", true);
        }
        else
        {
            SpriteObj.GetComponent<Animator>().SetBool("Walking", false);
        }

        AttackArea.transform.localScale = new Vector3(EntityData.currentAttackItem.attackwidth, EntityData.currentAttackItem.attackDistance, 1);

        attackHeldDown = Input.GetKey(KeyCode.Mouse0);
        if (attackHeldDown && !EntityData.canAttack)
        {
            canAttackByHeld = false;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && EntityData.canAttack)
        {
            canAttackByHeld = true;
            Attack();
        }
        else if (Input.GetKey(KeyCode.Mouse0) && EntityData.canAttack && canAttackByHeld)
        {
            Attack();
        }

        RaycastHit hit;
        Ray ray = MainCameraControl.cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, Vector3.Distance(MainCameraControl.gameObject.transform.position, transform.position) + 10f, LayerMask.GetMask("RaycastHit"), QueryTriggerInteraction.Ignore);
        playerTarget = hit.point;

        switch (EntityData.currentAttackItem.type)
        {
            case ItemType.MeleeWeapon:
                var atkRotation = MathEx.AngleRadian(AttackArea.transform.position, hit.point);
                AttackArea.transform.rotation = Quaternion.Euler(-90, 0, (-atkRotation * Mathf.Rad2Deg) + 90);
                AttackArea.SetActive(true);
                gameManagerInstance.targetObject.SetActive(false);
                break;
            case ItemType.RangedWeapon:
                gameManagerInstance.SetTargetPosition(new Vector2(hit.point.x, hit.point.z));
                AttackArea.SetActive(false);
                gameManagerInstance.targetObject.SetActive(true);
                break;
            case ItemType.CustomWeapon:

                break;
        }
    }
    public void Damage(DamageData damageData)
    {
        OnDamageEvent.Invoke(EntityData, damageData.sender);

        int total = !damageData.ignoreDefense ? Mathf.Max(damageData.damage - EntityData.CalculateDefense(), 0) : damageData.damage;

        EntityData.currentHealth -= total;
        EntityData.WasteMana(damageData.manaDamage);
        EntityData.WasteStamina(damageData.staminaDamage);
        EntityData.currentImpulse += damageData.impulse;

        StopCoroutine(SetDamageColor());
        StartCoroutine(SetDamageColor());

        EntityData.GiveEffects(damageData.effects);

        if (EntityData.currentHealth <= 0)
        {
            Die(damageData.sender);
        }
    }
    public void SetItem(Item item)
    {
        EntityData.currentAttackItem = item;
        ItemSpriteRenderer.sprite = EntityData.currentAttackItem.itemSprite;
        ItemSpriteAnimator.runtimeAnimatorController = item.animatorController;
        ItemSpriteOffset.transform.localPosition = (Vector3)item.positionOffSet + new Vector3(0, 0, -0.01f);
    }
    private IEnumerator SetDamageColor()
    {
        EntityData.damaged = true;
        SpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        EntityData.damaged = false;
        SpriteRenderer.color = Color.white;
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
            yield return new WaitForSeconds(SpriteObj.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);
            SpriteRenderer.enabled = false;
            ItemSpriteRenderer.color = Color.clear;
        }
    }
    public void Attack()
    {
        StartCoroutine(_Attack());
    }
    private IEnumerator _Attack()
    {
        if (!EntityData.attackReloaded ||
            UIGeneral.IsPointerOverUIElement() ||
            EntityData.currentAttackItem == null ||
            !EntityData.CanWasteMana(EntityData.currentAttackItem.manaUse) ||
            !EntityData.CanWasteStamina(EntityData.currentAttackItem.staminaUse))
            yield break;

        //ATAQUE EM ÁREA
        /*
        var list = AttackArea.GetComponent<ColliderNutshell>().GetColliders(EntityData.attackDistance + EntityData.currentAttackItem.attackDistance);
        foreach (var collider in list.ToList())
        {
            if (collider.tag == "GamePlayer" || collider.gameObject.layer != 10)
                continue;
            if (collider.GetComponent<IEntity>() != null)
            {
                collider.GetComponent<IEntity>().Damage(EntityData.AttackWithItem(MathEx.AngleRadian(transform.position, collider.transform.position)));
            }
        }*/

        var target = EntityData.target;
        var atkRotation = MathEx.AngleRadian(AttackArea.transform.position, playerTarget);

        AttackArea.transform.rotation = Quaternion.Euler(-90, 0, (-atkRotation * Mathf.Rad2Deg) + 90);
        EntityData.WasteMana(EntityData.currentAttackItem.manaUse);
        EntityData.WasteStamina(EntityData.currentAttackItem.staminaUse);

        StartCoroutine(AttackTimer());
        StartCoroutine(AttackAnimC());

        yield return new WaitForEndOfFrame();

        switch (EntityData.currentAttackItem.type)
        {
            case ItemType.MeleeWeapon:
                foreach (var entity in AttackArea.GetComponent<ColliderNutshell>().triggerList.ToList())
                {
                    if ((entity.layer == 6 || entity.layer == 10) && entity.GetComponent<IEntity>() != null)
                        entity.GetComponent<IEntity>().Damage(EntityData.AttackWithItem(MathEx.AngleRadian(transform.position, entity.transform.position)));
                }
                break;
            case ItemType.RangedWeapon:
                var bullet = Instantiate(EntityData.currentAttackItem.bulletPrefab, new Vector3(transform.position.x, IEntity.DEFAULT_SHOT_Y_POSITION, transform.position.z), Quaternion.Euler(0, (-MathEx.AngleRadian(transform.position, new Vector3(playerTarget.x, IEntity.DEFAULT_SHOT_Y_POSITION, playerTarget.z)) * Mathf.Rad2Deg) - 90, 0));
                bullet.GetComponent<IBullet>().SetBullet(gameObject);
                break;
            case ItemType.CustomWeapon:

                break;
        }
        if (EntityData.currentAttackItem.muzzlePrefab != null)
            Instantiate(EntityData.currentAttackItem.muzzlePrefab, transform.position, Quaternion.Euler(0, (-MathEx.AngleRadian(transform.position, playerTarget) * Mathf.Rad2Deg) - 90, 0));

        IEnumerator AttackAnimC()
        {
            ItemSpriteAnimator.Play("Attack");
            yield return new WaitForSeconds(ItemSpriteAnimator.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);
            ItemSpriteAnimator.Play("Default");
        }
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
    public void OnDestroy()
    {
        MainCameraControl.spriteRenderers.Remove(SpriteObj);
        gameManagerInstance.entities.Remove(gameObject);
    }
    private float manaTimer;
    private float staminaTimer;
    public void CalculateStatusRegen()
    {
        manaTimer += Time.fixedDeltaTime;
        staminaTimer += Time.fixedDeltaTime;

        if (manaTimer >= 6f / (EntityData.currentIntelligence))
        {
            manaTimer = 0;
            EntityData.WasteMana(-EntityData.currentIntelligence);
        }
        if (staminaTimer >= 6f / (EntityData.currentStrength))
        {
            staminaTimer = 0;
            EntityData.WasteStamina(-EntityData.currentStrength);
        }
    }
}
