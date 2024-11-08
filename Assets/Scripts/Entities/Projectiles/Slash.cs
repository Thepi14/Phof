using System.Collections;
using System.Collections.Generic;
using EntityDataSystem;
using ObjectUtils;
using UnityEngine;

public class Slash : BaseBulletBehaviour
{
    private Rigidbody RB => GetComponent<Rigidbody>();
    private List<GameObject> entitiesAffected = new List<GameObject>();
    private Vector3 initialPos;
    private float timer = 10;
    private float timerTime = 0;

    private void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }
    private void Start()
    {
        initialPos = transform.position;
        started = true;
    }
    private void Update()
    {
        if (!started)
            return;
        RB.velocity = new Vector3((transform.forward * projectileProperties.speed).x, RB.velocity.y, (transform.forward * projectileProperties.speed).z);
        //transform.rotation = Quaternion.LookRotation(RB.velocity.normalized);
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == null || collider.GetComponent<IEntity>() == null) return;
        if (entitiesAffected.Contains(collider.gameObject)) return;

        if ((gameObject.layer == 11 && collider.gameObject.layer == 8) || //ENEMY ATTACKS
            (gameObject.layer == 12 && collider.gameObject.layer == 10) || //PLAYER ATTACKS
            (collider.gameObject.layer == 6))//WALL
        {
            entitiesAffected.Add(collider.gameObject);
            collider.gameObject.GetComponent<IEntity>().Damage(new DamageData(gameObject, Random.Range(projectileProperties.minDamage + damageAdd, projectileProperties.maxDamage + damageAdd), MathEx.AngleVectors(initialPos, transform.position) * projectileProperties.impulseForce, projectileProperties.effects, true));
        }
    }
    private void FixedUpdate()
    {
        timerTime += Time.fixedDeltaTime;
        if (timerTime > timer)
        {
            Destroy(gameObject);
        }
    }
}
