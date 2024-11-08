using System;
using System.Collections;
using System.Collections.Generic;
using EntityDataSystem;
using HabilitySystem;
using static LangSystem.Language;
using UnityEngine;
using static CanvasGameManager;

namespace HabilitySystem
{
    [Serializable]
    public enum HabilityType : byte
    {
        basic = 0,
        special = 1,
        ultimate = 2,
    }
    /// <summary>
    /// Classe base para execução de habilidades.
    /// </summary>
    [Serializable]
    public abstract class HabilityBehaviour : MonoBehaviour, IHability
    {
        public string habilityID;
        public Sprite habilitySprite;
        public HabilityInfo habilityInfo;
        public bool reloaded => cooldownTimer >= cooldown;
        public HabilityType type;
        public float cooldown = 5f;
        public float cooldownTimer = 0f;
        public GameObject card;

        public abstract void ExecuteHability(GameObject target = null);
        public virtual void OnDestroy()
        {
            gameObject.GetComponent<IEntity>().EntityData.habilities.Remove(habilityID);
        }
        public virtual void OnDisable()
        {
            gameObject.GetComponent<IEntity>().EntityData.habilities.Remove(habilityID);
            Destroy(this);
        }
        /// <summary>
        /// Define os valores base da habilidade, pode ser trocado por outro método por override.
        /// </summary>
        /// <param name="ID">ID da habilidade.</param>
        /// <param name="cooldown">Tempo de recarga.</param>
        /// <param name="type">Tipo da habilidade.</param>
        public virtual void SetHability(string ID, int cooldown, HabilityType type)
        {
            habilityID = ID;
            habilitySprite = Resources.Load<Sprite>("Cards/" + habilityID);
            if (currentLanguage.habilityInfos.ContainsKey(habilityID))
                habilityInfo = currentLanguage.habilityInfos[habilityID];
            cooldownTimer = cooldown;
            this.cooldown = cooldown;
            this.type = type;
            canvasInstance.AddCard(this);
        }
        public virtual void OnValidate()
        {
            /*foreach (var lang in Language.languages)
            {
                foreach (var ID in lang.habilityInfos)
                {
                    if (ID.Key == habilityID)
                    {
                        goto langExit;
                    }
                }
                lang.AddSerializableInfo(habilityID, habilityInfo);
            langExit:;
            }*/
        }
        public virtual void CountReload()
        {
            if (type == HabilityType.basic)
                cooldownTimer = Mathf.Min(cooldown, cooldownTimer + Time.deltaTime);
        }
    }
    /// <summary>
    /// Você sabe né Marcos
    /// </summary>
    public interface IHability
    {
        /// <summary>
        /// Função para a execução da habilidade com script customizado, para o player poder usar o script deve ser adaptado para ele também.
        /// </summary>
        /// <param name="target">Alvo do ataque.</param>
        public void ExecuteHability(GameObject target = null);
    }
    [Serializable]
    public struct HabilityInfo
    {
        public string name;
        public string description;

        public HabilityInfo (string name, string description)
        {
            this.name = name;
            this.description = description;
        }
        /// <summary>
        /// Hability info serializable vira hability info implicitamente para a conversão de línguas.
        /// </summary>
        /// <param name="info"></param>
        public static implicit operator HabilityInfo(HabilityInfoSerializable info)
        {
            return new HabilityInfo(info.name, info.description);
        }
    }
    /// <summary>
    /// Feito para guardar info de forma exclusiva para o editor
    /// </summary>
    [Serializable]
    public struct HabilityInfoSerializable
    {
        public string ID;
        public string name;
        public string description;

        public HabilityInfoSerializable(string ID, string name, string description)
        {
            this.ID = ID;
            this.name = name;
            this.description = description;
        }
    }
}