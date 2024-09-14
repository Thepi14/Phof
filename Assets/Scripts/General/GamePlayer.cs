using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CameraControl;
using static TerrainGeneration;
using GameManagerSystem;
using EntityDataSystem;

public class GamePlayer : MonoBehaviour, IEntity
{
    public DeathEvent OnDeathEvent { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public DamageEvent OnDamageEvent { get; set; } = new DamageEvent();
    [SerializeField]
    private EntityData _entityData;
    public EntityData EntityData { get => _entityData; set => _entityData = value; }

    #region Variáveis obrigatórias
    public static GamePlayer player;
    [Header("Variáveis Obrigatórias", order = 0)]
    private float maxJumpSpeed = 7f;
    [SerializeField]
    private float _speed = 1f;
    public float Speed => EntityData.speed * 100;
    [SerializeField]
    private float _jumpForce = 1f;
    public float JumpForce => Mathf.Sqrt(_jumpForce);
    #endregion

    #region Status
    [Header("Status", order = 1)]
    [SerializeField]
    private bool _onGround;
    public bool OnGround { get => _onGround; private set => _onGround = value; }

    [SerializeField]
    private Vector3 velocity;
    public Vector2 XZInput; //X vector (vertical) = Z input, Y vector (horizontal) = X inpu
    #endregion

    #region Variáveis pré-definidas
    public Rigidbody RB => GetComponent<Rigidbody>();
    public CapsuleCollider2D Collid => GetComponent<CapsuleCollider2D>();
    public GameObject GroundDetectorObj => transform.Find("GroundDetector").gameObject;
    public ColliderNutshell GroundDetector => GroundDetectorObj.GetComponent<ColliderNutshell>();
    public GameObject SpriteObj => transform.Find("SpriteObject").gameObject;
    public SpriteRenderer SpriteRenderer => SpriteObj.GetComponent<SpriteRenderer>();
    public GameObject AttackArea => transform.Find("AttackArea").gameObject;

    #endregion

    private float time;
    void Start()
    {
        EntityData.CalculateStatus();
        EntityData.ResetStatus();
        player = this;

        transform.Find("ShadowObject").GetComponent<ShadowSeek>().shadowCastObject = gameObject;
        EntityData.canMove = true;

        MainCameraControl.focusObject = gameObject;
        MainCameraControl.focusMode = FocusMode.moveToFocusXYZ;
    }
    void FixedUpdate()
    {
        XZInput = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

        if(EntityData.canMove && XZInput != new Vector2(0, 0))
        {
            RB.velocity = new Vector3(XZInput.y * Speed * Time.fixedDeltaTime, 0, XZInput.x * Speed * Time.fixedDeltaTime);
        }
        if (!(EntityData.currentImpulse == Vector2.zero))
        {
            RB.velocity += new Vector3(EntityData.currentImpulse.x, 0, EntityData.currentImpulse.y);
            EntityData.currentImpulse *= 0.9f;
            if (EntityData.currentImpulse.x <= 0.5f && EntityData.currentImpulse.x >= -0.5f)
                EntityData.currentImpulse.x = 0;
            if (EntityData.currentImpulse.y <= 0.5f && EntityData.currentImpulse.y >= -0.5f)
                EntityData.currentImpulse.y = 0;
        }

        OnGround = GroundDetector.TriggerIsActive && (GroundDetector.TriggerContact != null ? (GroundDetector.TriggerContact.layer == 7 ? true : false) : true);
        if (Input.GetButton("Jump") && OnGround)
        {

        }
        velocity = RB.velocity;
    }
    private void Update()
    {
        //Debug.Log(RB.velocity.magnitude);

        if (RB.velocity.x > 0.1f || RB.velocity.x < -0.1f)
            SpriteRenderer.flipX = XZInput.y < 0;

        if ((XZInput.y != 0 || XZInput.x != 0) && OnGround)
        {
            SpriteObj.GetComponent<Animator>().SetBool("Walking", true);
        }
        else if (OnGround)
        {
            SpriteObj.GetComponent<Animator>().SetBool("Walking", false);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            string a = "";
            var l = AttackArea.GetComponent<ColliderNutshell>().GetColliders();
            for (int i = 0; i < l.Length; i++)
            {
                if (l[i].tag == "GamePlayer")
                    continue;
                a += l[i].gameObject.ToString();
                Destroy(AttackArea.GetComponent<ColliderNutshell>().GetColliders()[i].gameObject);
            }
            Debug.Log(a);
        }
    }

    public void Die(GameObject killer)
    {
        Destroy(gameObject);
    }

    public void Attack()
    {
        throw new System.NotImplementedException();
    }
}
