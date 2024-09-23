using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDeleter : MonoBehaviour
{
    public float timeAlive = 1;
    void Start()
    {
        StartCoroutine(DeleterTimer());
    }
    public IEnumerator DeleterTimer()
    {
        yield return new WaitForSeconds(timeAlive);
        Destroy(gameObject);
    }
}
