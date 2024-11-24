using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CameraControl;

public class DamageExbitionAngler : MonoBehaviour
{
    private Animator Animator => GetComponent<Animator>();
    private void Awake()
    {
        //Animator.Play("");
        MainCameraControl.spriteRenderers.Add(transform.parent.gameObject);
    }
    public void Destroy()
    {
        MainCameraControl.spriteRenderers.Remove(transform.parent.gameObject);
        Destroy(transform.parent.gameObject);
    }
}
