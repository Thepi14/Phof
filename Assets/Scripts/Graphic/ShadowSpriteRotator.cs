using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GamePlayer;

public class ShadowSpriteRotator : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, Mathf.Atan2(transform.position.x - player.gameObject.transform.position.x, transform.position.z - player.gameObject.transform.position.z) * Mathf.Rad2Deg, 0);
    }
}
