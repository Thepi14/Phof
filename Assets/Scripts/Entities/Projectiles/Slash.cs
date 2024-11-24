using System.Collections;
using System.Collections.Generic;
using EntityDataSystem;
using ObjectUtils;
using UnityEngine;

public class Slash : BaseBulletBehaviour
{
    private Rigidbody RB => GetComponent<Rigidbody>();
    private List<GameObject> entitiesAffected = new();
    private Vector3 initialPos;
    private float timer = 2f;
    private float timerTime = 0;
    private List<GameObject> wallList = new();

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
        if (!started && !destroyingProcess)
            return;
        RB.velocity = new Vector3((transform.forward * speed).x, RB.velocity.y, (transform.forward * speed).z);
        //transform.rotation = Quaternion.LookRotation(RB.velocity.normalized);
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (destroyingProcess)
            return;
        if (collider.gameObject.layer == 6 && collider.GetComponent<IEntity>() == null)
            wallList.Add(collider.gameObject);
        if (collider.gameObject == null || collider.GetComponent<IEntity>() == null) return;
        if (entitiesAffected.Contains(collider.gameObject)) return;

        if ((gameObject.layer == 11 && collider.gameObject.layer == 8) || //ENEMY ATTACKS
            (gameObject.layer == 12 && collider.gameObject.layer == 10) || //PLAYER ATTACKS
            (collider.gameObject.layer == 6))//WALL
        {
            entitiesAffected.Add(collider.gameObject);
            collider.gameObject.GetComponent<IEntity>().Damage(new DamageData(sender, Random.Range(minDamage + (senderEntity.EntityData.currentStrength * 3), maxDamage + (senderEntity.EntityData.currentStrength * 3)), MathEx.AngleVectors(initialPos, transform.position) * impulseForce, effects, true));
        }
    }
    private void FixedUpdate()
    {
        timerTime += Time.fixedDeltaTime;
        foreach (var wall in wallList)
        {
            if (wall.gameObject == null) continue;
            if (Vector3.Distance(MathEx.SetZeroY(transform.position), MathEx.SetZeroY(wall.transform.position)) < GetComponent<SphereCollider>().radius / 2f)
            {
                Destroy(gameObject);
            }
        }
        if (timerTime > timer)
        {
            AutoDestruction();
        }
    }
    protected sealed override IEnumerator AutoDestructionCoroutine(float timer = 0)
    {
        yield return new WaitForSeconds(0.1f);
        if (gameObject == null)
            yield break;
        transform.Find("SlashBright").gameObject.SetActive(false);
        RB.velocity = Vector3.zero;
        RB.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(1f);
        if (gameObject == null)
            yield break;
        Destroy(gameObject);
    }
}
