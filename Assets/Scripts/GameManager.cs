using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagerSystem
{
    /// <summary>
    /// Gerenciador de funções genéricas.
    /// </summary>
    public struct GameManager
    {
        /// <summary>
        /// Calcula a vida de uma entidade baseado nas suas características.
        /// </summary>
        /// <param name="entity">Entidade.</param>
        /// <returns>Valor da vida.</returns>
        public static Int32 CalculateHealth(EntityData entity)
        {
            Int32 result = (entity.resistence * 10) + (entity.level * 4) + 10;
            return result;
        }
        /// <summary>
        /// Calcula a mana de uma entidade baseado nas suas características.
        /// </summary>
        /// <param name="entity">Entidade.</param>
        /// <returns>Valor da mana.</returns>
        public static Int32 CalculateMana(EntityData entity)
        {
            Int32 result = (entity.intelligence * 10) + (entity.level * 4) + 5;
            return result;
        }
        /// <summary>
        /// Calcula a stamina de uma entidade baseado nas suas características.
        /// </summary>
        /// <param name="entity">Entidade.</param>
        /// <returns>Valor da stamina.</returns>
        public static Int32 CalculateStamina(EntityData entity)
        {
            Int32 result = (entity.resistence + entity.strength) + (entity.level * 2) + 5;
            return result;
        }
        /// <summary>
        /// Calcula o dano que a entidade irá receber baseado nas características dela e no dano da arma.
        /// </summary>
        /// <param name="entity">Entidade.</param>
        /// <param name="weaponDamage">Dano da arma.</param>
        /// <returns>A quantidade de dano.</returns>
        public static Int32 CalculateDamage(EntityData entity, int weaponDamage = 1)
        {
            System.Random random = new System.Random();
            Int32 result = (entity.strength * 2) + (weaponDamage * 2) + (entity.level * 3) + random.Next(1, 20);
            return result;
        }
        /// <summary>
        /// Calcula a defesa baseado nas características da entidade e na defesa da armadura.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="armorDefense"></param>
        /// <returns>O valor da defesa.</returns>
        public static Int32 CalculateDefense(EntityData entity, int armorDefense = 1)
        {
            Int32 result = (entity.resistence * 2) + (entity.level * 3) + armorDefense;
            return result;
        }
    }
}