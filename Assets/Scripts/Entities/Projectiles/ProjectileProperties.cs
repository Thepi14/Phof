using System;
using System.Collections;
using System.Collections.Generic;
using EntityDataSystem;
using UnityEngine;

namespace ProjectileSystem
{
    public enum BulletMovType : byte
    {
        normal = 0,
        teleguiado = 1
    }

    [Serializable]
    [CreateAssetMenu(fileName = "New Projetile", menuName = "Nova bala", order = 1)]
    public class ProjectileProperties : ScriptableObject
    {
        public int minDamage = 1;
        public int maxDamage = 1;
        public float speed = 10f;

        public List<Effect> effects = new List<Effect>();

        public bool explosive = false;
        public float explosionRadius = 1f;
        public float impulseForce = 0f;
    }
}
