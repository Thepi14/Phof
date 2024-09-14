using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using EntityDataSystem;
using static GamePlayer;

namespace GameManagerSystem
{
    /// <summary>
    /// Gerenciador do jogo agora
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager gameManagerInstance;

        public List<GameObject> enemyBullets = new List<GameObject>();
        public List<GameObject> playerBullets = new List<GameObject>();

        public List<GameObject> entities = new List<GameObject>();

        [SerializeField]
        private bool Tick;

        private void OnValidate()
        {
            gameManagerInstance = this;
        }
        private void Update()
        {
            foreach (var entity in entities.ToList())
            {
                switch (entity.layer)
                {
                    //PLAYER
                    case 8:

                        break;
                    //PLAYER MINION
                    case 9:

                        break;
                    //ENEMY
                    case 10:
                        VerifyEffects(entity.GetComponent<IEntity>().EntityData);
                        break;
                    default:
                        entities.Remove(entity);
                        break;
                }
            }
        }
        public void VerifyEffects(EntityData entityData)
        {
            foreach (Effect effect in entityData.currentEffects)
            {
                bool ticked = false;
                effect.duration -= Time.deltaTime;
                effect.currentTick += Time.deltaTime;
                if (effect.duration <= 0)
                {
                    entityData.currentEffects.Remove(effect);
                    continue;
                }
                if (effect.currentTick >= effect.tickDelay)
                {
                    effect.currentTick = 0;
                    ticked = true;
                }
                switch (effect.name)
                {
                    case "Poison":
                        if (ticked)
                        {

                        }
                        break;
                    default:
                        throw new Exception("Efeito desconhecido, nome: " + effect.name);
                }
            }
        }
        public void ClearEnemyBullets()
        {
            foreach (var bullet in enemyBullets.ToList())
            {
                Destroy(bullet);
            }
        }
        public void ClearPlayerBullets()
        {
            foreach (var bullet in playerBullets.ToList())
            {
                Destroy(bullet);
            }
        }
        public void ClearAllBullets()
        {
            ClearEnemyBullets();
            ClearPlayerBullets();
        }
    }
}
namespace UnityEngine
{
    public struct MathEx
    {
        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }

        public static Vector2 AngleVectors(Vector2 a, Vector2 b)
        {
            return RadianToVector2(Mathf.Atan2(a.y - b.y, a.x - b.x));
        }
        public static Vector2 AngleVectors(Vector3 a, Vector3 b)
        {
            return RadianToVector2(Mathf.Atan2(a.z - b.z, a.x - b.x));
        }
        public static float AngleRadian(Vector3 a, Vector3 b)
        {
            return Mathf.Atan2(a.z - b.z, a.x - b.x);
        }
    }
}