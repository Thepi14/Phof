using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectColliderDeactivator : MonoBehaviour
{
    public float timeAlive = 1;
    void Start()
    {
        StartCoroutine(DeactivatorTimer());
    }
    public IEnumerator DeactivatorTimer()
    {
        yield return new WaitForSeconds(timeAlive);
        foreach (var obj in ObjectUtils.GameObjectGeneral.GetGameObjectChildren(gameObject))
        {
            if (obj != null)
                if (obj.GetComponent<Collider>() != null)
                    obj.GetComponent<Collider>().isTrigger = true;
        }
    }
}
