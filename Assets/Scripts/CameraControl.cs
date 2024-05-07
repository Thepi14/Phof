using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Camera cam;
    public GameObject focusObject;
    public static CameraControl MainCameraControl;
    private float _defaultCameraFieldView;
    public float DefaultCameraFieldView { get => _defaultCameraFieldView; private set => _defaultCameraFieldView = value; }
    public bool moveToFocus = false;

    void Start()
    {
        MainCameraControl = this;
        cam = GetComponent<Camera>();
        DefaultCameraFieldView = cam.fieldOfView;
        moveToFocus = false;
        Debug.Log(MainCameraControl);
    }
    private void FixedUpdate()
    {
        if (moveToFocus)
        {
            transform.position = focusObject.transform.position + new Vector3(0, 8.7f, -11);
        }
    }
}
