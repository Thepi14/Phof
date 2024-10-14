using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CameraControl;
using static TerrainGeneration;
using static GameManagerSystem.GameManager;
using EntityDataSystem;
using ObjectUtils;
using System.Linq;

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
    #endregion

    #region Variáveis pré-definidas
    public Rigidbody RB => GetComponent<Rigidbody>();
    public CapsuleCollider2D Collid => GetComponent<CapsuleCollider2D>();
    public GameObject GroundDetectorObj => transform.Find("GroundDetector").gameObject;
    public ColliderNutshell GroundDetector => GroundDetectorObj.GetComponent<ColliderNutshell>();
    public GameObject SpriteObj => GameObjectGeneral.GetGameObject(gameObject, "SpriteObject");
    public GameObject ItemSpriteObj => GameObjectGeneral.GetGameObject(gameObject, "SpriteObject\\Item");
    public SpriteRenderer SpriteRenderer => SpriteObj.GetComponent<SpriteRenderer>();
    public SpriteRenderer ItemSpriteRenderer => ItemSpriteObj.GetComponent<SpriteRenderer>();
    public GameObject AttackArea => transform.Find("AttackArea").gameObject;

    #endregion

    public void OnValidate()
    {
        EntityData.gameObject = gameObject;
    }
    public void Start()
    {
        player = this;
        EntityData.gameObject = gameObject;
        MainCameraControl.spriteRenderers.Add(SpriteObj);
        gameManagerInstance.entities.Add(gameObject);
        EntityData.CalculateStatus();
        EntityData.ResetStatus();

        transform.Find("ShadowObject").GetComponent<ShadowSeek>().shadowCastObject = gameObject;
        EntityData.canMove = true;

        MainCameraControl.focusObject = gameObject;
        MainCameraControl.focusMode = FocusMode.moveToFocusXYZ;

        ItemSpriteRenderer.sprite = EntityData.currentAttackItem.itemSprite;
    }
    public void FixedUpdate()
    {
        XZInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if(EntityData.canMove && XZInput != new Vector2(0, 0))
            RB.velocity = new Vector3(XZInput.x * EntityData.currentSpeed, 0, XZInput.y * EntityData.currentSpeed);
        else
            RB.velocity = Vector3.zero;

        EntityData.currentImpulse -= new Vector2(EntityData.currentImpulse.x * Time.fixedDeltaTime * 5f, EntityData.currentImpulse.y * Time.fixedDeltaTime * 5f);
        RB.velocity -= new Vector3(EntityData.currentImpulse.x, 0, EntityData.currentImpulse.y);
        //OnGround = GroundDetector.TriggerIsActive && (GroundDetector.TriggerContact != null ? (GroundDetector.TriggerContact.layer == 7 ? true : false) : true);
        velocity = RB.velocity;
    }
    public void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.L))
        {
            Attack();
        }
    }

    public void Damage(DamageData damageData)
    {
        OnDamageEvent.Invoke(EntityData, damageData.sender);

        int total = !damageData.ignoreDefense ? Mathf.Max(damageData.damage - EntityData.CalculateDefense(), 0) : damageData.damage;

        EntityData.currentHealth -= total;
        EntityData.currentImpulse += damageData.impulse;

        StopCoroutine(SetDamageColor());
        StartCoroutine(SetDamageColor());

        EntityData.GiveEffects(damageData.effects);

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

    public void Die(GameObject killer)
    {
        OnDeathEvent.Invoke(EntityData, killer);
        Destroy(gameObject);
    }

    public void Attack()
    {
        var list = AttackArea.GetComponent<ColliderNutshell>().GetColliders();
        foreach (var collider in list.ToList())
        {
            if (collider.tag == "GamePlayer" || collider.gameObject.layer != 10)
                continue;
            if (collider.GetComponent<IEntity>() != null)
            {
                collider.GetComponent<IEntity>().Damage(EntityData.AttackWithItem(MathEx.AngleRadian(transform.position, collider.transform.position)));
            }
        }
    }

    public void OnDestroy()
    {
        MainCameraControl.spriteRenderers.Remove(SpriteObj);
        gameManagerInstance.entities.Remove(gameObject);
    }
}
