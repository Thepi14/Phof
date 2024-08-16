using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntityData 
{
    [Header("Name")]
    public string name;
    public int level = 1;

    [Header("Health")]
    public float currentHealth;
    public int maxHealth;

    [Header("Stamina")]
    public float currentStamina;
    public int maxStamina;

    [Header("Mana")]
    public float currentMana;
    public int maxMana;

    [Header("Atributos")]
    public int strength = 1;
    public int resistence = 1;
    public int intelligence = 1;
    public int damage = 1;
    public int defense = 1;
    public float speed = 1f;

    [Header("Combate")]
    public float attackDistance = 0.5f;
    public float attackTimer = 1;
    public float multiAtaque;
    public float cooldown = 2;
    public bool inCombat = false;
    public GameObject target;
    public bool combatCoroutine = false;
    public bool dead = false;
}
