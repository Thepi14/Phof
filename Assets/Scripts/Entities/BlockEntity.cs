using System.Collections;
using System.Collections.Generic;
using EntityDataSystem;
using ItemSystem;
using UnityEngine;
using static NavMeshUpdate;

public class BlockEntity : MonoBehaviour, IEntity
{
    [SerializeField]
    private EntityData _entityData;
    public EntityData EntityData { get => _entityData; set => _entityData = value; }

    [SerializeField]
    private DeathEvent _onDeathEvent;
    public DeathEvent OnDeathEvent { get => _onDeathEvent; set => _onDeathEvent = value; }

    [SerializeField]
    private DamageEvent _onDamageEvent;
    public DamageEvent OnDamageEvent { get => _onDamageEvent; set => _onDamageEvent = value; }

    public void Awake() 
    {
        EntityData = new EntityData
        {
            currentEffects = null,
            currentAttackItem = null,
            canAttack = false,
            canMove = false,
            attackDistance = 0,
            attackDelay = 0,
            attackReloaded = false,
            currentImpulse = Vector2.zero,
            attackSpeedMultipliyer = 0,

            intelligence = 0,
            strength = 0,
            speed = 0,
            maxMana = 0,
            maxStamina = 0,

            currentIntelligence = 0,
            currentStrength = 0,
            currentSpeed = 0,
            currentMana = 0,
            currentStamina = 0,

            defense = 1,
            resistence = 1,
            currentDefense = 1,
            currentResistence = 1
        };
    }

    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    public void CalculateStatusRegen()
    {
        throw new System.NotImplementedException();
    }

    public void Damage(DamageData damageData)
    {
        EntityData.currentHealth -= damageData.damage;
        if (EntityData.currentHealth < 0) Die(damageData.sender);
    }

    public void Die(GameObject killer)
    {
        navMeshUpdateInstance.BuildNavMesh(100);
        Destroy(gameObject);
    }

    public void SetItem(Item item)
    {
        throw new System.NotImplementedException();
    }
}