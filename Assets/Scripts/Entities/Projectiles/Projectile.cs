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
            if (!started)
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

            RB.velocity = new Vector3((transform.forward * projectileProperties.speed).x, RB.velocity.y, (transform.forward * projectileProperties.speed).z);
            //transform.rotation = Quaternion.LookRotation(RB.velocity.normalized);
        }
        private void Explode()
        {
            var colliders = Physics.OverlapSphere(transform.position, projectileProperties.explosionRadius);
            foreach (var col in colliders.ToList())
            {
                var obj = col.gameObject;
                damageData = new DamageData(sender, Random.Range(projectileProperties.minDamage, projectileProperties.maxDamage), MathEx.AngleVectors(transform.position, obj.transform.position) * projectileProperties.impulseForce, projectileProperties.effects, false);
                switch (obj.layer)
                {
                    case 6 or 7:
                        if (obj.GetComponent<IEntity>() != null)
                            obj.GetComponent<IEntity>().Damage(damageData);
                        break;
                    //PLAYER
                    case 8 when gameObject.layer == 11:
                        obj.GetComponent<IEntity>().Damage(damageData);
                        break;
                    //ENEMY
                    case 10 when gameObject.layer == 12:
                        obj.GetComponent<IEntity>().Damage(damageData);
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
            damageData = new DamageData(sender, Random.Range(projectileProperties.minDamage, projectileProperties.maxDamage), MathEx.AngleVectors(transform.position, collider.transform.position) * projectileProperties.impulseForce, projectileProperties.effects, false);
            switch (collider.gameObject.layer)
            {
                case 0:
                    return;
                case 6 or 7:
                    if (collider.GetComponent<IEntity>() != null)
                        if (!projectileProperties.explosive)
                            collider.GetComponent<IEntity>().Damage(damageData);
                    break;
                    //PLAYER
                case 8 when gameObject.layer == 11:
                    if (!projectileProperties.explosive)
                        collider.GetComponent<IEntity>().Damage(damageData);
                    break;
                    //ENEMY
                case 10 when gameObject.layer == 12:
                    if (!projectileProperties.explosive)
                        collider.GetComponent<IEntity>().Damage(damageData);
                    break;
                default:
                    EndBullet();
                    throw new System.Exception("Acho que o layer " + gameObject.layer + " não deveria estar colidindo com a layer " + collider.gameObject.layer);
            }
            if (projectileProperties.explosive)
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