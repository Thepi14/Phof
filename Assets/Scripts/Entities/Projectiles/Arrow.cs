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
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class Arrow : BaseBulletBehaviour
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
        public int penetration = 0;

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
               EndBullet();

            RB.velocity = new Vector3((transform.forward * speed).x, RB.velocity.y, (transform.forward * speed).z);
            //transform.rotation = Quaternion.LookRotation(RB.velocity.normalized);
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
                        collider.GetComponent<IEntity>().Damage(DamageData);
                    break;
                //PLAYER
                case 8 when gameObject.layer == 11:
                    collider.GetComponent<IEntity>().Damage(DamageData);
                    break;
                //ENEMY
                case 10 when gameObject.layer == 12:
                    collider.GetComponent<IEntity>().Damage(DamageData);
                    break;
                default:
                    EndBullet();
                    throw new System.Exception("Acho que o layer " + gameObject.layer + " não deveria estar colidindo com a layer " + collider.gameObject.layer);
            }
            penetration--;
            if (penetration == -1)
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