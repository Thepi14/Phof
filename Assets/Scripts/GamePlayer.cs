using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CameraControl;
using static TerrainGeneration;
using GameManagerSystem;

public class GamePlayer : MonoBehaviour
{
    public Player entity => GetComponent<Player>();
    #region Variáveis obrigatórias
    public static GamePlayer player;
    [Header("Variáveis Obrigatórias", order = 0)]
    private float maxJumpSpeed = 7f;
    [SerializeField]
    private float _speed = 1f;
    public float Speed => entity.player.speed * 100;
    [SerializeField]
    private float _jumpForce = 1f;
    public float JumpForce => Mathf.Sqrt(_jumpForce);
    public Animater animater;

    public Sprite[] idleFrames;
    public Sprite[] walkFrames;
    public Sprite[] jumpFrames;
    public Sprite[] landFrames;
    #endregion

    #region Status
    [Header("Status", order = 1)]
    [SerializeField]
    private bool _onGround;
    public bool OnGround { get => _onGround; private set => _onGround = value; }

    [SerializeField]
    private Vector3 velocity;
    public Vector2 XZInput; //X vector (vertical) = Z input, Y vector (horizontal) = X input.
    #endregion

    #region Variáveis pré-definidas
    public Rigidbody RB => GetComponent<Rigidbody>();
    public CapsuleCollider2D Collid => GetComponent<CapsuleCollider2D>();
    public SpriteRenderer SpriteRenderer => SpriteObj.GetComponent<SpriteRenderer>();
    public GameObject Cam => transform.parent.Find("Camera").gameObject;
    public GameObject GroundDetectorObj => transform.Find("GroundDetector").gameObject;
    public ColliderNutshell GroundDetector => GroundDetectorObj.GetComponent<ColliderNutshell>();
    public GameObject SpriteObj => transform.Find("SpriteObject").gameObject;
    public GameObject AttackArea => transform.Find("AttackArea").gameObject;
    #endregion

    void Start()
    {
        /*if(manager == null)
        {
            Debug.LogError("Anexa o game manager no player prr");
            return;
        }*/
        entity.player.maxHealth = GameManager.CalculateHealth(entity.player);
        entity.player.maxMana = GameManager.CalculateMana(entity.player);
        entity.player.maxStamina = GameManager.CalculateStamina(entity.player);

        entity.player.currentHealth = entity.player.maxHealth;
        entity.player.currentMana = entity.player.maxStamina;
        entity.player.currentStamina = entity.player.maxStamina;

        animater = new Animater(SpriteRenderer);
        animater.Animate(this, new Anime("Idle", idleFrames, true, 12));
        player = this;
        transform.Find("ShadowObject").GetComponent<ShadowSeek>().shadowCastObject = gameObject;

        MainCameraControl.focusObject = gameObject;
        MainCameraControl.focusMode = FocusMode.moveToFocusXYZ;
    }
    void FixedUpdate()
    {
        XZInput = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        RB.velocity = new Vector3(XZInput.y * Speed * Time.fixedDeltaTime, RB.velocity.y, XZInput.x * Speed * Time.fixedDeltaTime);

        OnGround = GroundDetector.TriggerIsActive && (GroundDetector.TriggerContact != null ? (GroundDetector.TriggerContact.layer == 6 ? true : false) : true);
        if (Input.GetButton("Jump") && OnGround)
        {
            RB.AddForce(0, JumpForce, 0, ForceMode.Impulse);
        }
        if (RB.velocity.y > maxJumpSpeed)
            RB.velocity = new Vector3(RB.velocity.x, maxJumpSpeed, RB.velocity.z);
        velocity = RB.velocity;
    }
    private void Update()
    {
        //Debug.Log(RB.velocity.magnitude);

        if (RB.velocity.x > 0.1f || RB.velocity.x < -0.1f)
            SpriteObj.transform.localScale = XZInput.y < 0 ?
                new Vector3(1, 1, 1) :
                new Vector3(-1, 1, 1);
        if (!OnGround && animater.currentAnimation.name != "Jump")
        {
            animater.Animate(this, new Anime("Jump", jumpFrames, false, 12, true));
        }
        else if (OnGround && animater.currentAnimation.name == "Jump")
        {
            animater.Animate(this, new Anime("Land", landFrames, false, 12, true));
        }
        if ((XZInput.y != 0 || XZInput.x != 0) && OnGround)
        {
            animater.Animate(this, new Anime("Walk", walkFrames, true, 12));
        }
        else if (OnGround)
        {
            animater.Animate(this, new Anime("Idle", idleFrames, true, 12));
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            animater.freezeAnimation = !animater.freezeAnimation;
        }
    }
}
