using EntityDataSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameManagerSystem.GameManager;
using ObjectUtils;
using System.Linq;

namespace ProjectileSystem
{
    public enum BulletExclusionType
    {
        None = 0,
        ByTime = 1,
        ByDistance = 2
    }
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : BaseBulletBehaviour
    {
        [SerializeField]
        private BulletExclusionType bulletExclusionType;
        [SerializeField]
        private float maxTimeAlive = 5f;
        [SerializeField]
        private float maxDistance = 10f;
        private Rigidbody RB => GetComponent<Rigidbody>();
        public Vector3 startPosition;
        [SerializeField]
        private float condition = 0f;
        public bool explodeWithoutHitting = false;
        public bool explosive = false;
        public float explosionRadius = 1f;

        private void OnValidate()
        {
            GetComponent<Collider>().isTrigger = true;
        }
        public void Awake()
        {
            startPosition = transform.position;
        }
        void Update()
        {
            if (!Started)
                return;

            switch (bulletExclusionType)
            {
                case BulletExclusionType.ByTime:
                    condition += Time.deltaTime;
                    break;
                case BulletExclusionType.ByDistance:
                    condition = Vector3.Distance(startPosition, transform.position);
                    break;
            }

            if ((condition > maxTimeAlive && bulletExclusionType == BulletExclusionType.ByTime) || (condition > maxDistance && bulletExclusionType == BulletExclusionType.ByDistance))
                if (explodeWithoutHitting)
                    Explode();
                else
                    EndBullet();

            RB.velocity = new Vector3((transform.forward * speed).x, RB.velocity.y, (transform.forward * speed).z);
            //transform.rotation = Quaternion.LookRotation(RB.velocity.normalized);
        }
        private void Explode()
        {
            var colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var col in colliders.ToList())
            {
                var obj = col.gameObject;
                DamageData = new DamageData(Sender, Random.Range(minDamage, maxDamage), MathEx.AngleVectors(transform.position, obj.transform.position) * impulseForce, effects, false);
                switch (obj.layer)
                {
                    case 6 or 7:
                        if (obj.GetComponent<IEntity>() != null)
                            obj.GetComponent<IEntity>().Damage(DamageData);
                        break;
                    //PLAYER
                    case 8 when gameObject.layer == 11:
                        if (!RayCastTargetIsBehindWall(obj))
                            obj.GetComponent<IEntity>().Damage(DamageData);
                        break;
                    //ENEMY
                    case 10 when gameObject.layer == 12:
                        if (!RayCastTargetIsBehindWall(obj))
                            obj.GetComponent<IEntity>().Damage(DamageData);
                        break;
                    default:
                        break;
                }
            }
            DeathEffect();
            EndBullet();
        }
        private void OnTriggerEnter(Collider collider)
        {
            DamageData = new DamageData(Sender, Random.Range(minDamage, maxDamage), MathEx.AngleVectors(transform.position, collider.transform.position) * impulseForce, effects, false);
            switch (collider.gameObject.layer)
            {
                case 0:
                    return;
                case 6 or 7:
                    if (collider.GetComponent<IEntity>() != null)
                        if (!explosive)
                            collider.GetComponent<IEntity>().Damage(DamageData);
                    break;
                    //PLAYER
                case 8 when gameObject.layer == 11:
                    if (!explosive)
                        collider.GetComponent<IEntity>().Damage(DamageData);
                    break;
                    //ENEMY
                case 10 when gameObject.layer == 12:
                    if (!explosive)
                        collider.GetComponent<IEntity>().Damage(DamageData);
                    break;
                default:
                    EndBullet();
                    throw new System.Exception("Acho que o layer " + gameObject.layer + " não deveria estar colidindo com a layer " + collider.gameObject.layer);
            }
            if (explosive)
                Explode();
            else
                EndBullet();
        }
        public void EndBullet()
        {
            RB.velocity = new Vector3(0f, 0f, 0f);
            RB.constraints = RigidbodyConstraints.FreezeAll;
            if (gameObject.layer == 11)
            {
                gameManagerInstance.enemyBullets.Remove(gameObject);
            }
            else if (gameObject.layer == 12)
            {
                gameManagerInstance.playerBullets.Remove(gameObject);
            }
            Destroy(gameObject);
        }
    }
}