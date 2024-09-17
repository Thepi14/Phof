using System;
using System.Collections;
using System.Collections.Generic;
using EntityDataSystem;
using UnityEngine;

namespace ItemSystem
{
    public enum ItemType : byte
    {
        None = 0,
        /// <summary>
        /// Arma corpo a corpo.
        /// </summary>
        MeleeWeapon = 1,
        /// <summary>
        /// Arma à distância.
        /// </summary>
        RangedWeapon = 2,
    }
    [Serializable]
    [CreateAssetMenu(fileName = "new Item", menuName = "Novo item", order = 0)]
    public class Item : ScriptableObject
    {
        [Header("Geral", order = 0)]
        public string itemName = "NULL";
        public Sprite itemSprite;
        public ItemType type;

        [Header("Ataque", order = 1)]
        /// <summary>
        /// Recarregamento do ataque em segundos
        /// </summary>
        public float reloadTime = 1f;
        public float attackDistance = 1f;
        /// <summary>
        /// Quantidade máxima por slot
        /// </summary>
        public byte maxStack = 1;
        /// <summary>
        /// Dano mínimo
        /// </summary>
        public int minDamage = 1;
        /// <summary>
        /// Dano máximo
        /// </summary>
        public int maxDamage = 1;
        public float impulse = 0f;
        public List<Effect> effects;

        [Header("Ataque à distância", order = 2)]
        /// <summary>
        /// Prefab da bala
        /// </summary>
        [SerializeReference]
        public GameObject bulletPrefab;

        public void OnValidate()
        {
            switch (type)
            {
                case ItemType.None:
                    bulletPrefab = null;
                    break;
                case ItemType.MeleeWeapon:
                    maxStack = 1;
                    bulletPrefab = null;
                    break;
                case ItemType.RangedWeapon:
                    maxStack = 1;
                    break;
                default:
                    throw new Exception("O tipo do item não foi achado.");
            }
        }
    }
}
