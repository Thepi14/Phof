using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[AddComponentMenu("Physics/Collider Nutshell", 0)]
[RequireComponent(typeof(Collider))]
public class ColliderNutshell : MonoBehaviour
{
    #region Variáveis

    //Mostrar status para depuração
    /// <summary>
    /// Mostra mensagens de depuração.
    /// </summary>
    [Header("Debug", order = 0)]
    public bool showDebugMessages = false;
    /// <summary>
    /// Mostra mensagens de depuração em tempo real.
    /// </summary>
    public bool showOnCollisionMessages = false;
    /// <summary>
    /// Mostra os colisores.
    /// </summary>
    public bool showDebugColliders = false;

    private bool _enteredCollision,
        _onCollision,
        _exitedCollision,
        _enteredTrigger,
        _onTrigger,
        _exitedTrigger;

    //Variáveis de info.
    public Collider ColliderComponent { get => GetComponent<Collider>(); }

    //Variáveis visíveis no editor
    [Header("Status", order = 1)]
    [SerializeField]
    private bool enteredCollision;
    [SerializeField]
    private bool onCollision;
    [SerializeField]
    private bool exitedCollision;
    [SerializeField]
    private bool enteredTrigger;
    [SerializeField]
    private bool onTrigger;
    [SerializeField]
    private bool exitedTrigger;

    //variáveis somente para leitura
    public bool EnteredCollision { get => enteredCollision; private set => enteredCollision = value; }
    public bool OnCollision { get => onCollision; private set => onCollision = value; }
    public bool ExitedCollision { get => exitedCollision; private set => exitedCollision = value; }
    public bool EnteredTrigger { get => enteredTrigger; private set => enteredTrigger = value; }
    public bool OnTrigger { get => onTrigger; private set => onTrigger = value; }
    public bool ExitedTrigger { get => exitedTrigger; private set => exitedTrigger = value; }

    //Variáveis visíveis no editor
    [SerializeField]
    private bool triggerIsActive;
    [SerializeField]
    private bool colliderIsActive;

    //variáveis somente leitura para colisões mais completas
    public bool TriggerIsActive { get => enteredTrigger || onTrigger || exitedTrigger; }
    public bool ColliderIsActive { get => exitedCollision || onCollision || exitedCollision; }

    //Variáveis visíveis no editor
    [SerializeField]
    private GameObject triggerContact;
    [SerializeField]
    private GameObject colliderContact;
    [SerializeField]
    private GameObject lastTriggerContact;
    [SerializeField]
    private GameObject lastColliderContact;

    //variáveis somente leitura para providenciar o bloco em colisão atual
    public GameObject TriggerContact { get => triggerContact; private set => triggerContact = value; }
    public GameObject ColliderContact { get => colliderContact; private set => colliderContact = value; }

    //variáveis somente leitura para providenciar o bloco da colisão passada
    public GameObject LastTriggerContact { get => lastTriggerContact; private set => lastTriggerContact = value; }
    public GameObject LastColliderContact { get => lastColliderContact; private set => lastColliderContact = value; }

    //variáveis privadas
    /*private GameObject afterActualCol;
    private GameObject afterActualTri;*/

    #endregion

    #region Resolução de variáveis
    public void Start()
    {
        lastColliderContact = null;
        lastTriggerContact = null;
        colliderContact = null;
        triggerContact = null;

        if (!showDebugMessages)
            return;

        string str = ((object)ColliderComponent).GetType().ToString();
        Debug.Log(str);
    }
    public void FixedUpdate()
    {
        EnteredCollision = _enteredCollision;
        OnCollision = _onCollision;
        ExitedCollision = _exitedCollision;

        EnteredTrigger = _enteredTrigger;
        OnTrigger = _onTrigger;
        ExitedTrigger = _exitedTrigger;

        _enteredCollision = false;
        _onCollision = false;
        _exitedCollision = false;

        _enteredTrigger = false;
        _onTrigger = false;
        _exitedTrigger = false;

        colliderIsActive = ColliderIsActive;
        triggerIsActive = TriggerIsActive;
    }
    #endregion

    #region colisões com Colliders 3D

    public void OnCollisionEnter(Collision collision)
    {
        _enteredCollision = true;
        EnteredCollision = true;
        SetActualObjectColliding(collision.gameObject);
        if (showDebugMessages)
            Debug.Log("Entered Collision with " + collision.gameObject.name);
    }
    public void OnCollisionStay(Collision collision)
    {
        _onCollision = true;
        OnCollision = true;
        //ColliderContact = collision.gameObject;
        if (showDebugMessages && showOnCollisionMessages)
            Debug.Log("Exited Trigger with " + collision.gameObject.name);
    }
    public void OnCollisionExit(Collision collision)
    {
        _exitedCollision = true;
        ExitedCollision = true;
        SetActualObjectColliding(null);
        if (showDebugMessages)
            Debug.Log("Is on Collision with " + collision.gameObject.name);
    }
    #endregion

    #region colisões com Colliders 2D

    public void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        _enteredCollision = true;
        EnteredCollision = true;
        SetActualObjectColliding(collision.gameObject);
        if (showDebugMessages)
            Debug.Log("Entered Collision with " + collision.gameObject.name);
    }
    public void OnCollisionStay2D(UnityEngine.Collision2D collision)
    {
        _onCollision = true;
        OnCollision = true;
        //ColliderContact = collision.gameObject;
        if (showDebugMessages && showOnCollisionMessages)
            Debug.Log("Exited Trigger with " + collision.gameObject.name);
    }
    public void OnCollisionExit2D(UnityEngine.Collision2D collision)
    {
        _exitedCollision = true;
        ExitedCollision = true;
        SetActualObjectColliding(null);
        if (showDebugMessages)
            Debug.Log("Is on Collision with " + collision.gameObject.name);
    }
    #endregion

    #region colisões com Triggers 3D

    public void OnTriggerEnter(Collider collision)
    {
        _enteredTrigger = true;
        EnteredTrigger = true;
        SetActualObjectTriggering(collision.gameObject);
        if (showDebugMessages)
            Debug.Log("Entered Trigger with " + collision.gameObject.name);
    }
    public void OnTriggerStay(Collider collision)
    {
        _onTrigger = true;
        OnTrigger = true;
        //TriggerContact = collision.gameObject;
        if (showDebugMessages && showOnCollisionMessages)
            Debug.Log("Is on Trigger with " + collision.gameObject.name);
    }
    public void OnTriggerExit(Collider collision)
    {
        _exitedTrigger = true;
        ExitedTrigger = true;
        SetActualObjectTriggering(null);
        if (showDebugMessages)
            Debug.Log("Exited Trigger with " + collision.gameObject.name);
    }
    #endregion

    #region colisões com Triggers 2D

    public void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        _enteredTrigger = true;
        EnteredTrigger = true;
        SetActualObjectTriggering(collision.gameObject);
        if (showDebugMessages)
            Debug.Log("Entered Trigger with " + collision.gameObject.name);
    }
    public void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {
        _onTrigger = true;
        OnTrigger = true;
        //TriggerContact = collision.gameObject;
        if (showDebugMessages && showOnCollisionMessages)
            Debug.Log("Is on Trigger with " + collision.gameObject.name);
    }
    public void OnTriggerExit2D(UnityEngine.Collider2D collision)
    {
        _exitedTrigger = true;
        ExitedTrigger = true;
        SetActualObjectTriggering(null);
        if (showDebugMessages)
            Debug.Log("Exited Trigger with " + collision.gameObject.name);
    }
    #endregion

    #region Detecção do objeto

    private void SetActualObjectColliding(GameObject obj)
    {
        LastColliderContact = (ColliderContact != null && ColliderContact != LastColliderContact) || (LastColliderContact == null && ColliderContact != null) ? ColliderContact : LastColliderContact;
        ColliderContact = obj;
    }
    private void SetActualObjectTriggering(GameObject obj)
    {
        LastTriggerContact = (TriggerContact != null && TriggerContact != LastTriggerContact) || (LastColliderContact == null && TriggerContact != null) ? TriggerContact : LastTriggerContact;
        TriggerContact = obj;
    }

    #endregion

    #region Intersecções
    /// <summary>
    /// Usado para conseguir uma lista de colisores que interseccionem com o colisor criado, usando o colisor atual do <b>GameObject</b>.
    /// </summary>
    /// <param name="size">Tamanho opcional para colisores em formato de caixa. (BoxCollider)</param>
    /// <returns>Uma lista dos colisores detectados.</returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public Collider[] GetColliders(Vector3? size = null)
    {
        if (ColliderComponent is Collider)
        {
            Collider a = ColliderComponent as Collider;
            if (!a.isTrigger)
                return null;
        }
        if (ColliderComponent is BoxCollider)
        {
            BoxCollider collider = ColliderComponent as BoxCollider;
            if (showDebugColliders)
                GetCollidersDebug(size == null ? collider.size : (Vector3)size);
            return Physics.OverlapBox(collider.center + transform.position, size == null ? collider.size : (Vector3)size, transform.rotation);
        }
        else if (ColliderComponent is SphereCollider)
        {
            SphereCollider collider = ColliderComponent as SphereCollider;
            return Physics.OverlapSphere(collider.center + transform.position, collider.radius);
        }
        else if (ColliderComponent is CapsuleCollider)
        {
            CapsuleCollider collider = ColliderComponent as CapsuleCollider;
            Vector3 dir = 
                collider.direction == 0 ? new Vector3(((Vector3)size).x * (collider.height / 2), 0, 0) : 
                collider.direction == 1 ? new Vector3(0, ((Vector3)size).y * (collider.height / 2), 0) : 
                collider.direction == 2 ? new Vector3(0, 0, ((Vector3)size).z * (collider.height / 2)) : 
                Vector3.zero;
            return Physics.OverlapCapsule(dir + transform.position, (dir * -1) + transform.position, collider.radius);
        }

        throw new System.NullReferenceException("Não foi achado nenhum collider bro.");
        //return null;
    }
    /// <summary>
    /// Usado para conseguir uma lista de colisores que interseccionem com o colisor criado, usando o colisor atual do <b>GameObject</b>
    /// </summary>
    /// <param name="radius">Raio opcional para a esfera ou a cápsula.</param>
    /// <returns>Uma lista dos colisores detectados.</returns>
    /// <exception cref="System.NullReferenceException"></exception>
    public Collider[] GetColliders(int radius)
    {
        if (ColliderComponent is Collider)
        {
            Collider a = ColliderComponent as Collider;
            if (!a.isTrigger)
                return null;
        }
        if (showDebugColliders)
            GetCollidersDebug(Vector3.one * radius);
        if (ColliderComponent is BoxCollider)
        {
            BoxCollider collider = ColliderComponent as BoxCollider;
            return Physics.OverlapBox(collider.center + transform.position, Vector3.one * radius, transform.rotation);
        }
        else if (ColliderComponent is SphereCollider)
        {
            SphereCollider collider = ColliderComponent as SphereCollider;
            return Physics.OverlapSphere(collider.center + transform.position, radius);
        }
        else if (ColliderComponent is CapsuleCollider)
        {
            CapsuleCollider collider = ColliderComponent as CapsuleCollider;
            Vector3 dir =
                collider.direction == 0 ? new Vector3((collider.height / 2), 0, 0) :
                collider.direction == 1 ? new Vector3(0, (collider.height / 2), 0) :
                collider.direction == 2 ? new Vector3(0, 0, (collider.height / 2)) :
                Vector3.zero;
            return Physics.OverlapCapsule(dir + transform.position, (dir * -1) + transform.position, radius);
        }

        throw new System.NullReferenceException("Não foi achado nenhum collider bro.");
        //return null;
    }
    private void GetCollidersDebug(Vector3 vector)
    {
        if (ColliderComponent is Collider)
        {
            Collider a = ColliderComponent as Collider;
            if (!a.isTrigger)
                return;
        }
        if (ColliderComponent is BoxCollider)
        {
            BoxCollider collider = ColliderComponent as BoxCollider;
            Gizmos.color = Color.red;
            if (showDebugColliders)
                Gizmos.DrawWireCube(collider.center + transform.position, vector);
        }
        else if (ColliderComponent is SphereCollider)
        {
            SphereCollider collider = ColliderComponent as SphereCollider;
            Gizmos.color = Color.red;
            if (showDebugColliders)
                Gizmos.DrawSphere(collider.center + transform.position, vector.x);
        }
        else if (ColliderComponent is CapsuleCollider)
        {
            CapsuleCollider collider = ColliderComponent as CapsuleCollider;
            Gizmos.color = Color.yellow;
            if (showDebugColliders)
                Gizmos.DrawSphere(collider.center + transform.position, vector.x);
        }
    }
    #endregion
}
