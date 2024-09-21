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
    public class Projectile : MonoBehaviour
    {
        public GameObject sender;
        public ProjectileProperties projectileProperties;
        private Rigidbody RB => GetComponent<Rigidbody>();

        [SerializeField]
        private bool started = false;

        public void SetProjectile(GameObject sender, float? radAngles = null)
        {
            //Debug.Log((float)radAngles);
            this.sender = sender;
            gameObject.layer = sender.layer == 8 ? 12 : sender.layer == 10 ? 11 : 0;
            if (radAngles != null)
                transform.rotation = Quaternion.Euler(0, (float)radAngles * Mathf.Rad2Deg, 0);
            started = true;
        }
        private void OnValidate()
        {
            GetComponent<Collider>().isTrigger = true;
        }
        void Update()
        {
            if (!started)
                return;
            RB.velocity = new Vector3((transform.forward * projectileProperties.speed).x, RB.velocity.y, (transform.forward * projectileProperties.speed).z);
            //transform.rotation = Quaternion.LookRotation(RB.velocity.normalized);
        }
        private void Explode()
        {
            var colliders = Physics.OverlapSphere(transform.position, projectileProperties.explosionRadius);
            foreach (var col in colliders.ToList())
            {
                var obj = col.gameObject;
                switch (obj.layer)
                {
                    case 8 when gameObject.layer == 11:
                        obj.GetComponent<IEntity>().Damage(new DamageData(sender, Random.Range(projectileProperties.minDamage, projectileProperties.maxDamage), MathEx.AngleVectors(transform.position, obj.transform.position), projectileProperties.effects));
                        break;
                    case 10 when gameObject.layer == 12:
                        obj.GetComponent<IEntity>().Damage(new DamageData(sender, Random.Range(projectileProperties.minDamage, projectileProperties.maxDamage), MathEx.AngleVectors(transform.position, obj.transform.position), projectileProperties.effects));
                        break;
                    default:
                        break;
                }
            }
            EndBullet();
        }
        private void OnTriggerEnter(Collider collider)
        {
            switch (collider.gameObject.layer)
            {
                case 0 or 6 or 7:
                    if (projectileProperties.explosive)
                        Explode();
                    else
                        EndBullet();
                    return;
                    //PLAYER
                case 8 when gameObject.layer == 11:
                    collider.GetComponent<IEntity>().Damage(new DamageData(sender, Random.Range(projectileProperties.minDamage, projectileProperties.maxDamage), MathEx.AngleVectors(transform.position, collider.transform.position), projectileProperties.effects));
                    break;
                    //PLAYER MINION
                /*case 9 when gameObject.layer == 11:
                    collider.GetComponent<IEntity>().Damage(new DamageData(sender, Random.Range(projectileProperties.minDamage, projectileProperties.maxDamage), MathEx.AngleVectors(transform.position, collider.transform.position), projectileProperties.effects));
                    if (projectileProperties.explosive)
                        Explode();
                    break;*/
                    //ENEMY
                case 10 when gameObject.layer == 12:
                    collider.GetComponent<IEntity>().Damage(new DamageData(sender, Random.Range(projectileProperties.minDamage, projectileProperties.maxDamage), MathEx.AngleVectors(transform.position, collider.transform.position), projectileProperties.effects));
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