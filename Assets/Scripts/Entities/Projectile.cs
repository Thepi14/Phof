using EntityDataSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static GameManagerSystem.GameManager;
using ObjectUtils;

namespace ProjectileSystem
{
    public class Projectile : MonoBehaviour
    {
        public ProjectileProperties projectileProperties;
        public Vector2 direction;

        private Rigidbody RB => GetComponent<Rigidbody>();

        private void Start()
        {

        }
        void Update()
        {
            RB.velocity = new Vector3(direction.x * projectileProperties.speed, RB.velocity.y, direction.y * projectileProperties.speed);
        }
        private void Explode()
        {
            var colliders = Physics.OverlapSphere(transform.position, projectileProperties.explosionRadius);
            foreach (var col in colliders)
            {
                var obj = col.gameObject;
                if (obj.layer == 8)
                {
                    obj.GetComponent<IEntity>().EntityData.currentHealth -= Random.Range(projectileProperties.minDamage, projectileProperties.maxDamage);
                }
            }
            EndBullet();
        }
        private void OnTriggerEnter(Collider collider)
        {
            RB.velocity = new Vector3(0f, 0f, 0f);
            switch (collider.gameObject.layer)
            {
                case 0 or 6 or 7:
                    return;
                    //PLAYER
                case 8 when gameObject.layer == 11:
                    collider.gameObject.GetComponent<IEntity>().EntityData.currentImpulse += MathEx.AngleVectors(transform.position, collider.transform.position);
                    break;
                    //PLAYER MINION
                case 9 when gameObject.layer == 11:

                    break;
                    //ENEMY
                case 10 when gameObject.layer == 12:

                    break;
                default:
                    throw new System.Exception("Acho que o layer " + gameObject.layer + " não deveria estar colidindo com a layer " + collider.gameObject.layer);
            }
            if (projectileProperties.explosive)
                Explode();
        }
        public void EndBullet()
        {
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