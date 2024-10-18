using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using EntityDataSystem;
using HabilitySystem;
using UnityEngine;

namespace HabilitySystem
{
    /// <summary>
    /// Classe base para execu��o de habilidades.
    /// </summary>
    public abstract class HabilityBehaviour : MonoBehaviour, IHability
    {
        public abstract void ExecuteHability(GameObject target = null);
        public void OnDestroy()
        {
            gameObject.GetComponent<IEntity>().EntityData.habilities.Remove(this);
        }
    }
    /// <summary>
    /// Voc� sabe n� Marcos
    /// </summary>
    public interface IHability
    {
        /// <summary>
        /// Fun��o para a execu��o da habilidade com script customizado, para o player poder usar o script deve ser adaptado para ele tamb�m.
        /// </summary>
        /// <param name="target">Alvo do ataque.</param>
        public void ExecuteHability(GameObject target = null);
    }
}