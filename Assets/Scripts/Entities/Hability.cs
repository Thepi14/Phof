// --------------------------------------------------------------------------------------------------------------------
/// <copyright file="Hability.cs">
///   Copyright (c) 2024, Pi14, All rights reserved.
/// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using EntityDataSystem;
using HabilitySystem;
using static LangSystem.Language;
using UnityEngine;
using static CanvasGameManager;
using UnityEditor;
using System.Text.RegularExpressions;
using UnityEngine.Windows;

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
        public bool selected = false;
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
        public virtual string ReturnNotReloadedHabilityText() => currentLanguage.HabilityIsNotReloadedPlusName(currentLanguage.habilityInfos[habilityID].name);
        /// <summary>
        /// Define os valores base da habilidade, pode ser trocado por outro método por override.
        /// </summary>
        /// <param name="ID">ID da habilidade.</param>
        /// <param name="cooldown">Tempo de recarga.</param>
        /// <param name="type">Tipo da habilidade.</param>
        public virtual void SetHability(string ID, int cooldown, HabilityType type)
        {
            habilityID = ID;
            habilitySprite = Resources.Load<Sprite>("CardSprites/" + habilityID);
            if (currentLanguage.habilityInfos.ContainsKey(habilityID))
                habilityInfo = currentLanguage.habilityInfos[habilityID];
            cooldownTimer = cooldown;
            this.cooldown = cooldown;
            this.type = type;
            canvasInstance.AddCard(this);
        }
        public virtual void UnselectCards()
        {
            if (gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                foreach (var hability in GetComponent<IEntity>().EntityData.habilities)
                {
                    if (hability.Value == this)
                        continue;
                    hability.Value.selected = false;
                }
            }
            else if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                foreach (var hability in GetComponent<IEntity>().EntityData.habilities)
                {
                    if (hability.Value == this)
                        continue;
                    hability.Value.selected = false;
                }
            }
            this.selected = true;
        }
        public void OnValidate()
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
            return new HabilityInfo(info.name, info.TrueDescription);
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
        [Multiline(10, order = 1)]
        [SerializeField]
        private string description;
        public string TrueDescription => Regex.Replace(description, @"\r\n?|\n", " ");

        public HabilityInfoSerializable(string ID, string name, string textAsset)
        {
            this.ID = ID;
            this.name = name;
            description = textAsset;
#if UNITY_EDITOR
            OnGUI();
#endif
        }
#if UNITY_EDITOR
        readonly void OnGUI()
        {
            GUI.skin.button.wordWrap = true;
            EditorStyles.textField.wordWrap = true;
        }
#endif
    }
}