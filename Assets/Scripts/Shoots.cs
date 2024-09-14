using EntityDataSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shoots : MonoBehaviour
{
    public int tipo;
    public Vector3 target;
    public Vector3 target2;
    private Vector3 dir;
    private bool explodir = false;
    private bool congelar = false;
    public float speed;
    public EntityData entity;
    public EntityData entityAlvo;
    public float count;

    private Rigidbody rbEntity;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 20;
        count = 3f;
    }
    void Update()
    {
        switch (tipo)
        {
            case 0:
                Explosivo();
            break;
            case 1:
                Freeze();
            break;
        }
    }
    private void Explosivo()//salvar
    {
        if (!explodir)
        {
            MoveToTarget();
            if (transform.position == target)
                explodir = true;
        }
        else
        {
            transform.localScale += new Vector3(1 * Time.fixedDeltaTime, 1 * Time.fixedDeltaTime, 1 * Time.fixedDeltaTime);
            StartCoroutine("ExplodBulletTime");
        }

    }

    private void Freeze()
    {
        GetComponent<Renderer>().material.color = Color.blue;
        if (!congelar)
        {
            MoveDir();
            Debug.Log(rb.velocity);
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
            if(count <= 0)
            {
                rbEntity.gameObject.GetComponentInChildren<GamePlayer>().SpriteRenderer.color = Color.white;
                rbEntity.gameObject.GetComponentInChildren<GamePlayer>().animater.freezeAnimation = false;
                entity.canMove = true;
                Destroy(gameObject);
            }
            else
            {
                rbEntity.gameObject.GetComponentInChildren<GamePlayer>().SpriteRenderer.color = new Color(49f / 255f, 152f / 255f, 156f / 255f, 255f / 255f);
                rbEntity.gameObject.GetComponentInChildren<GamePlayer>().animater.freezeAnimation = true;
                entity.canMove = false;
                count -= Time.deltaTime;
                rbEntity.velocity = Vector3.zero;
            }
               
        }
    }

    IEnumerator AttackBulletTime()
    {
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }
    IEnumerator ExplodBulletTime()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "GamePlayer")
        {
            explodir = true;
            congelar = true;
            rb.velocity = new Vector3(0f, 0f, 0f);
            rbEntity = collider.GetComponent<Rigidbody>();
            entity = collider.gameObject.GetComponent<GamePlayer>().entity;
        }else if (collider.gameObject.layer == 7)
        {

            switch (tipo)
            {
                case 0:
                    explodir = true;
                    break;
                case 1:
                    Destroy(gameObject);
                    break;
            }
        }


    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, this.target, speed * Time.deltaTime);
        StartCoroutine("AttackBulletTime");
    }

    private void MoveDir()
    {
        dir = (target - target2).normalized;
        rb.velocity = (dir * speed);
        StartCoroutine("AttackBulletTime");
    }

}
