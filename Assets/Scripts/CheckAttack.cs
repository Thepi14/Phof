using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAttack : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("GamePlayer"))
        {
            gameObject.GetComponentInParent<Enemy>().entityData.inCombat = true;
            Debug.Log("Player detectado");
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("GamePlayer"))
        {
           gameObject.GetComponentInParent<Enemy>().entityData.inCombat = false;
            Debug.Log("Player não detectado");
        }
    }
}
