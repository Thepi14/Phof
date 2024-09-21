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
        public List<GameObject> enemies = new List<GameObject>();
        public List<GameObject> allies = new List<GameObject>();

        public const float EFFECT_CHANGER = 0.9f;

        [SerializeField]
        private bool Tick;

        private void OnValidate()
        {
            gameManagerInstance = this;
        }
        private void Start()
        {
            gameManagerInstance = this;
        }
        private void Update()
        {
            foreach (var entity in entities.ToList())
            {
                switch (entity.layer)
                {
                    case 8 or 10:
                        VerifyEffects(entity.GetComponent<IEntity>());
                        break;
                    default:
                        entities.Remove(entity);
                        break;
                }
            }
        }
        public void AddEntity(GameObject entity)
        {
            if (entity.GetComponent<IEntity>() == null)
                throw new Exception("Added entity" + entity.name + "doesn't has a entity script.");
            entities.Add(entity);
            switch (entity.layer)
            {
                case 8 when entity.GetComponent<GamePlayer>() != null:

                    break;
                case 8:
                    allies.Add(entity);
                    entity.GetComponent<IEntity>().OnDeathEvent.AddListener((a, obj) => { entities.Remove(entity); allies.Remove(entity); });
                    break;
                case 10:
                    enemies.Add(entity);
                    entity.GetComponent<IEntity>().OnDeathEvent.AddListener((a, obj) => { entities.Remove(entity); enemies.Remove(entity); });
                    break;
            }
        }
        public void VerifyEffects(IEntity entity)
        {
            entity.EntityData.ResetAttributesStatus();
            foreach (Effect effect in entity.EntityData.currentEffects.ToList())
            {
                bool ticked = false;
                effect.duration -= Time.deltaTime;
                effect.currentTick += Time.deltaTime; 
                if (effect.duration <= 0 || effect.level == 0)
                {
                    if (effect.level == 0)
                        Debug.LogWarning("Efeito com nível 0 não tem efeito.");
                    entity.EntityData.currentEffects.Remove(effect);
                    continue;
                }
                if (effect.currentTick >= effect.tickDelay)
                {
                    effect.currentTick = 0;
                    ticked = true;
                }
                switch (effect.name.ToLower())
                {
                    case "slowness":
                        effect.currentTick = 0;
                        entity.EntityData.currentSpeed = entity.EntityData.currentSpeed * (EFFECT_CHANGER / effect.level);
                        break;
                    case "agility":
                        effect.currentTick = 0;
                        entity.EntityData.currentSpeed = entity.EntityData.currentSpeed + ((entity.EntityData.currentSpeed / (EFFECT_CHANGER + 1)) * effect.level);
                        break;
                    case "poison":
                        if (ticked)
                            entity.Damage(new DamageData(effect.level * 2, true));
                        break;
                    case "on fire":
                        if (ticked)
                            entity.Damage(new DamageData(effect.level * 3));
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