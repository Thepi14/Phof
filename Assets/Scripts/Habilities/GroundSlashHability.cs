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
            slashPrefab = Resources.Load<GameObject>("Projectiles/GSlash");
        }
        public void Update()
        {
            if (selected == false)
            {
                selectedTarget = false;
                selecting = false;
            }
            if (gameObject.layer == 8)
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
            }
            else
            {

            }

            CountReload();
        }
        public override async void ExecuteHability(GameObject target = null)
        {
            if (!reloaded)
            {
                WarningTextManager.ShowWarning(ReturnNotReloadedHabilityText(), 3f, 1f);
                return;
            }
            selected = true;
            UnselectCards();
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
            Vector3 eTarget = gameObject.layer == 10 ? eTarget = GetComponent<IEntity>().EntityData.target.transform.position : eTarget = GetComponent<GamePlayer>().playerTarget;
            var slash = Instantiate(slashPrefab, new Vector3(transform.position.x, IEntity.DEFAULT_SHOT_Y_POSITION, transform.position.z), Quaternion.Euler(0, (-MathEx.AngleRadian(transform.position, new Vector3(eTarget.x, IEntity.DEFAULT_SHOT_Y_POSITION, eTarget.z)) * Mathf.Rad2Deg) - 90, 0), GameManager.gameManagerInstance.gameObject.transform);
            slash.GetComponent<IBullet>().Sender = gameObject;
            slash.layer = gameObject.layer == 8 ? 12 : 11;
            selecting = false;
            selectedTarget = false;
            await Task.Delay(100);
            GetComponent<IEntity>().EntityData.canAttack = true;
        }
    }
}