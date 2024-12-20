using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAttack : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GamePlayer"))
           gameObject.GetComponentInParent<Enemy>().EntityData.inCombat = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("GamePlayer"))
        {
           gameObject.GetComponentInParent<Enemy>().EntityData.inCombat = false;
        }
    }
}
