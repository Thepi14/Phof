using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using EntityDataSystem;
using static GamePlayer;
using ObjectUtils;

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
        public void RemoveEffectByName(IEntity entity, string name, int frames)
        {
            foreach (Effect effect in entity.EntityData.currentEffects.ToList())
            {
                if (effect.name.ToLower() == name.ToLower())
                {
                    if (effect.frameAdded < frames)
                    {
                        entity.EntityData.currentEffects.Remove(effect);
                    }
                    return;
                }
            }
        }
        public void VerifyEffects(IEntity entity)
        {
            entity.EntityData.ResetAttributesStatus();

            if (!entity.EntityData.damaged)
                GameObjectGeneral.GetGameObjectComponent<SpriteRenderer>(entity.EntityData.gameObject, "SpriteObject").color = Color.white;
            else
                GameObjectGeneral.GetGameObjectComponent<SpriteRenderer>(entity.EntityData.gameObject, "SpriteObject").color = Color.red;
            foreach (Effect effect in entity.EntityData.currentEffects.ToList())
            {
                bool ticked = false;
                effect.duration -= Time.deltaTime;
                    if (effect.tickDelay > 0)
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
                        RemoveEffectByName(entity, "agility", effect.frameAdded);
                        effect.currentTick = 0;
                        entity.EntityData.currentSpeed = entity.EntityData.currentSpeed * (EFFECT_CHANGER / effect.level);
                        break;
                    case "agility":
                        RemoveEffectByName(entity, "slowness", effect.frameAdded);
                        effect.currentTick = 0;
                        entity.EntityData.currentSpeed = entity.EntityData.currentSpeed + ((entity.EntityData.currentSpeed / (EFFECT_CHANGER + 1)) * effect.level);
                        break;
                    case "poison":
                        if (ticked)
                        {
                            if (entity.EntityData.currentHealth - (effect.level * 7) + (entity.EntityData.resistence) <= 0)
                                effect.currentTick = effect.tickDelay;
                            else
                                entity.Damage(new DamageData(effect.level * 2, true));
                        }
                        if (!entity.EntityData.damaged)
                            ChangeEntityColor(entity, (GameObjectGeneral.GetGameObjectComponent<SpriteRenderer>(entity.EntityData.gameObject, "SpriteObject").color / 2) + (Color.green / 2));
                        break;
                    case "on fire":
                        RemoveEffectByName(entity, "freezed", effect.frameAdded);
                        RemoveEffectByName(entity, "coldness", effect.frameAdded);
                        if (ticked)
                            entity.Damage(new DamageData((effect.level * 6) + (entity.EntityData.resistence) + ((effect.level / 2) * UnityEngine.Random.Range(0, 6))));
                        if (!entity.EntityData.damaged)
                            ChangeEntityColor(entity, (GameObjectGeneral.GetGameObjectComponent<SpriteRenderer>(entity.EntityData.gameObject, "SpriteObject").color / 2) + (new Color(1, 0.5f, 0) / 2));
                        break;
                    case "coldness":
                        RemoveEffectByName(entity, "on fire", effect.frameAdded);
                        entity.EntityData.currentSpeed = entity.EntityData.currentSpeed * (EFFECT_CHANGER / effect.level);
                        if (ticked)
                            entity.Damage(new DamageData((effect.level * 8) + (entity.EntityData.resistence)));
                        if (!entity.EntityData.damaged)
                            ChangeEntityColor(entity, (GameObjectGeneral.GetGameObjectComponent<SpriteRenderer>(entity.EntityData.gameObject, "SpriteObject").color / 2) + (Color.blue / 2));
                        break;
                    case "freeze":
                        RemoveEffectByName(entity, "on fire", effect.frameAdded);
                        entity.EntityData.currentSpeed = 0;
                        if (ticked)
                            entity.Damage(new DamageData((effect.level * 12) + (entity.EntityData.resistence)));
                        if (!entity.EntityData.damaged)
                            ChangeEntityColor(entity, (GameObjectGeneral.GetGameObjectComponent<SpriteRenderer>(entity.EntityData.gameObject, "SpriteObject").color / 3) + (Color.blue / 1.5f));
                        break;
                    case "stun":
                        effect.currentTick = 0;
                        entity.EntityData.currentSpeed = 0;
                        if (!entity.EntityData.damaged)
                            ChangeEntityColor(entity, (GameObjectGeneral.GetGameObjectComponent<SpriteRenderer>(entity.EntityData.gameObject, "SpriteObject").color / 2) + (Color.gray / 2));
                        break;
                    case "vulnerable":
                        effect.currentTick = 0;
                        entity.EntityData.currentDefense = Mathf.Max(0, entity.EntityData.currentDefense - effect.level);
                        break;
                    case "healing":
                        if (ticked)
                        {
                            entity.EntityData.currentHealth = Mathf.Min(entity.EntityData.maxHealth, entity.EntityData.currentHealth + effect.level);
                            ChangeEntityColor(entity, Color.green, 0.4f);
                        }
                        break;
                    default:
                        throw new Exception("Efeito desconhecido, nome: " + effect.name);
                }
            }
        }
        private void ChangeEntityColor(IEntity entity, Color color, float time = 0f)
        {
            StartCoroutine(_ChangeEntityColor(entity, color, time));
            IEnumerator _ChangeEntityColor(IEntity entity, Color color, float time)
            {
                float timePassed = 0f;
                if (!entity.EntityData.damaged)
                    GameObjectGeneral.GetGameObjectComponent<SpriteRenderer>(entity.EntityData.gameObject, "SpriteObject").color = color;
                while (timePassed <= time)
                {
                    if (!entity.EntityData.damaged)
                        GameObjectGeneral.GetGameObjectComponent<SpriteRenderer>(entity.EntityData.gameObject, "SpriteObject").color = color;
                    timePassed += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
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