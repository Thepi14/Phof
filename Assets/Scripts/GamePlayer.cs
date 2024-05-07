using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CameraControl;

public class GamePlayer : MonoBehaviour
{
    #region Variáveis obrigatórias
    public static GamePlayer player;
    [Header("Variáveis Obrigatórias", order = 0)]
    public float speed = 1f;
    public float jumpForce = 1f;
    public Animater animater;

    public Sprite[] idleFrames;
    public Sprite[] walkFrames;
    public Sprite[] jumpFrames;
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
        animater = new Animater(SpriteRenderer);
        player = this;
        transform.Find("ShadowObject").GetComponent<ShadowSeek>().shadowCastObject = gameObject;

        MainCameraControl.focusObject = gameObject;
        MainCameraControl.moveToFocus = true;
    }
    void FixedUpdate()
    {

        XZInput = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        RB.velocity = new Vector3(XZInput.y * speed * Time.fixedDeltaTime, RB.velocity.y, XZInput.x * speed * Time.fixedDeltaTime);

        OnGround = GroundDetector.TriggerIsActive && (GroundDetector.TriggerContact != null ? (GroundDetector.TriggerContact.layer == 6 ? true : false) : true);
        if (Input.GetButton("Jump") && OnGround)
        {
            RB.AddForce(0, jumpForce * 0.1f, 0, ForceMode.Impulse);
        }
        velocity = RB.velocity;
    }
    private void Update()
    {
        transform.rotation = XZInput.y < 0 ? Quaternion.Euler(transform.rotation.x, 0, 
            transform.rotation.z) :
            Quaternion.Euler(transform.rotation.x, 180, 
            transform.rotation.z);
        //SpriteObj.transform.localRotation = Quaternion.Euler((Mathf.Atan2(Cam.transform.position.z - SpriteObj.transform.position.z, Cam.transform.position.y - SpriteObj.transform.position.y) * (180 / Mathf.PI)) + 90, SpriteObj.transform.localRotation.y, SpriteObj.transform.localRotation.z);
        if (!OnGround)
        {
            animater.Animate(this, new Anime("Jump", jumpFrames, false, 12));
            goto jumpGroundAnimations;
        }
        if (XZInput.y != 0 || XZInput.x != 0 && OnGround)
        {
            animater.Animate(this, new Anime("Walk", walkFrames, true, 12));
        }
        else if (OnGround)
        {
            animater.Animate(this, new Anime("Idle", idleFrames, true, 12));
        }
        jumpGroundAnimations:
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
}
