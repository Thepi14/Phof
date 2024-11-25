using System;
using System.Collections;
using System.Collections.Generic;
using EntityDataSystem;
using UnityEngine;

namespace ItemSystem
{
    public delegate void CustomItemAttack(EntityData senderEntityData);
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
        /// <summary>
        /// Arma com comportamento customizado
        /// </summary>
        CustomWeapon = 3,
    }
    /// <summary>
    /// Classe scriptável para itens.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "new Item", menuName = "Novo item", order = 0)]
    public class Item : ScriptableObject
    {
        public string ID => name;
        [Header("Geral", order = 0)]
        public string itemName = "NULL";
        public Sprite itemSprite;
        public RuntimeAnimatorController animatorController;
        public Vector2 positionOffSet;
        public int manaUse;
        public int staminaUse;
        public ItemType type;

        [Header("Ataque", order = 1)]
        [Tooltip("Recarregamento do ataque em segundos")]
        public float reloadTime = 1f;
        public float attackDistance = 1f;
        [Tooltip("Largura do ataque, diferente do attackDistance pois faz a largura da área de ataque aumentar, podendo afetar mais entidades em ataques corpo à corpo")]
        public float attackwidth = 1f;
        [Tooltip("Quantidade máxima por slot")]
        public byte maxStack = 1;
        public int minDamage = 1;
        public int maxDamage = 1;
        [Tooltip("Dano de mana, diminui a mana do inimigo")]
        public int manaDamage = 0;
        [Tooltip("Dano de stamina, diminui a stamina do inimigo")]
        public int staminaDamage = 0;
        public float impulse = 0f;
        public List<Effect> effects;

        [Header("Ataque à distância", order = 2)]
        [SerializeReference]
        public GameObject bulletPrefab;

        [Header("Efeitos", order = 3)]
        [SerializeReference]
        public GameObject muzzlePrefab;

        [Header("Scripts customizados", order = 4)]
        [Tooltip("Script customizado para ataques, deve ser um MonoBehaviour com a interface ICustomItemAttack para funcionar")]
        public CustomAttackIndex customAttackIndex;
        public CustomItemAttack customAttack;

        public void OnValidate()
        {
            customAttack = customAttacks[customAttackIndex];

            switch (type)
            {
                case ItemType.None:
                    bulletPrefab = null;
                    break;
                case ItemType.MeleeWeapon or ItemType.RangedWeapon or ItemType.CustomWeapon:
                    maxStack = 1;
                    break;
                default:
                    throw new Exception("O tipo do item não foi achado.");
            }
        }
        public enum CustomAttackIndex : byte
        {
            None = 0,

        }
        private static Dictionary<CustomAttackIndex, CustomItemAttack> customAttacks = new Dictionary<CustomAttackIndex, CustomItemAttack>
        {
            {CustomAttackIndex.None, (senderEntityData) => { } }
        };
    }
}
