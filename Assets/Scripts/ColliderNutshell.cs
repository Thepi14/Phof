using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderNutshell : MonoBehaviour
{
    #region Variáveis

    //Mostrar status para depuração
    [Header("Debug", order = 0)]
    public bool showDebugMessages = false;
    public bool showOnCollsionMessages = false;

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
    private GameObject lastTriggerObject;
    [SerializeField]
    private GameObject lastColliderObject;

    //variáveis somente leitura para providenciar o bloco em colisão atual
    public GameObject TriggerContact { get => triggerContact; private set => triggerContact = value; }
    public GameObject ColliderContact { get => colliderContact; private set => colliderContact = value; }

    //variáveis somente leitura para providenciar o bloco da colisão passada
    public GameObject LastTriggerObject { get => lastTriggerObject; private set => lastTriggerObject = value; }
    public GameObject LastColliderObject { get => lastColliderObject; private set => lastColliderObject = value; }

    //variáveis privadas
    /*private GameObject afterActualCol;
    private GameObject afterActualTri;*/

    #endregion

    #region Resolução de variáveis
    public void Start()
    {
        lastColliderObject = null;
        lastTriggerObject = null;
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
        if (showDebugMessages && showOnCollsionMessages)
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
        if (showDebugMessages && showOnCollsionMessages)
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
        if (showDebugMessages && showOnCollsionMessages)
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
        if (showDebugMessages && showOnCollsionMessages)
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
        LastColliderObject = (ColliderContact != null && ColliderContact != LastColliderObject) || (LastColliderObject == null && ColliderContact != null) ? ColliderContact : LastColliderObject;
        ColliderContact = obj;
    }
    private void SetActualObjectTriggering(GameObject obj)
    {
        LastTriggerObject = (TriggerContact != null && TriggerContact != LastTriggerObject) || (LastColliderObject == null && TriggerContact != null) ? TriggerContact : LastTriggerObject;
        TriggerContact = obj;
    }

    #endregion

    #region Intersecções

    public Collider[] GetColliders()
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
            return Physics.OverlapBox(collider.center + transform.position, collider.size, transform.rotation);
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
                collider.direction == 0 ? new Vector3((collider.height / 2), 0, 0) : 
                collider.direction == 1 ? new Vector3(0, (collider.height / 2), 0) : 
                collider.direction == 2 ? new Vector3(0, 0, (collider.height / 2)) : 
                Vector3.zero;
            return Physics.OverlapCapsule(dir + transform.position, (dir * -1) + transform.position, collider.radius);
        }

        return null;
    }
    #endregion
}
