using System.Collections;
using System.Collections.Generic;
using EntityDataSystem;
using UnityEngine;
using GameManagerSystem;
using ObjectUtils;
using System.Threading.Tasks;
using static ObjectUtils.UIGeneral;

namespace HabilitySystem
{
    public class GroundSlashHability : HabilityBehaviour
    {
        public GameObject slashPrefab;
        public bool selectedTarget = false;
        public bool selecting = false;

        public void Start()
        {
            SetHability("GroundSlash", 5, HabilityType.basic);
            slashPrefab = Resources.Load<GameObject>("VFX/Powers/G_Slash/GSlash_Prefab/VFXGraphs/VFX_GSlash");
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && selecting && !IsPointerOverUIElement())
            {
                selectedTarget = true;
                selecting = false;
                ExecuteHability();
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0) && selecting && IsPointerOverUIElement())
            {
                selecting = false;
                ExecuteHability();
            }

            CountReload();
        }
        public override async void ExecuteHability(GameObject target = null)
        {
            if (!reloaded)
                return;
            if (!selectedTarget && !selecting && IsPointerOverUIElement())
            {
                selecting = true;
                GetComponent<IEntity>().EntityData.canAttack = false;
                return;
            }
            else if (selecting && IsPointerOverUIElement())
            {
                selectedTarget = false;
                selecting = false;
                GetComponent<IEntity>().EntityData.canAttack = true;
                return;
            }
            cooldownTimer = 0;
            var eTarget = GetComponent<GamePlayer>().playerTarget;

            switch (gameObject.layer)
            {
                //ENEMY
                case 10:
                    eTarget = GetComponent<IEntity>().EntityData.target.transform.position;
                    var slash = Instantiate(slashPrefab, new Vector3(transform.position.x, IEntity.DEFAULT_SHOT_Y_POSITION, transform.position.z), Quaternion.Euler(0, (-MathEx.AngleRadian(transform.position, new Vector3(eTarget.x, IEntity.DEFAULT_SHOT_Y_POSITION, eTarget.z)) * Mathf.Rad2Deg) - 90, 0), GameManager.gameManagerInstance.gameObject.transform);
                    slashPrefab.layer = 11;
                    slashPrefab.GetComponent<IBullet>().damageAdd = GetComponent<IEntity>().EntityData.currentStrength * 2;
                    slashPrefab.GetComponent<IBullet>().sender = gameObject;
                    break;
                //PLAYER
                case 8:
                    slash = Instantiate(slashPrefab, new Vector3(transform.position.x, IEntity.DEFAULT_SHOT_Y_POSITION, transform.position.z), Quaternion.Euler(0, (-MathEx.AngleRadian(transform.position, new Vector3(eTarget.x, IEntity.DEFAULT_SHOT_Y_POSITION, eTarget.z)) * Mathf.Rad2Deg) - 90, 0), GameManager.gameManagerInstance.gameObject.transform);
                    slashPrefab.layer = 12;
                    slashPrefab.GetComponent<IBullet>().damageAdd = GetComponent<IEntity>().EntityData.currentStrength * 2;
                    slashPrefab.GetComponent<IBullet>().sender = gameObject;
                    break;
            }
            selecting = false;
            selectedTarget = false;
            await Task.Delay(100);
            GetComponent<IEntity>().EntityData.canAttack = true;
        }
    }
}