using System.Collections;
using System.Collections.Generic;
using EntityDataSystem;
using HabilitySystem;
using UnityEngine;

public class BerserkHability : HabilityBehaviour
{
    void Start()
    {
        SetHability("Berserk", 30, HabilityType.basic);
    }
    void Update()
    {
        CountReload();
    }
    public override void ExecuteHability(GameObject target = null)
    {
        if (reloaded)
        {
            UnselectCards();
            cooldownTimer = 0;
            GetComponent<IEntity>().EntityData.GiveEffect(new Effect("Berserk", 15f, 0, false, 1));
        }
    }
}
