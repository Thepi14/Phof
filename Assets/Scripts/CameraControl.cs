using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Camera cam;
    public GameObject focusObject;
    public static CameraControl MainCameraControl;
    public Vector3 Placement;
    public float EulerAngleYPlacement = 0;
    private float _defaultCameraFieldView;
    public float DefaultCameraFieldView { get => _defaultCameraFieldView; private set => _defaultCameraFieldView = value; }
    public const float PLAYER_GROUND_LEVEL = 2f;
    public const float CAMERA_DEFAULT_X_ANGLE = 35;
    public const float MAX_ANGLE_X = 55;

    public List<GameObject> spriteRenderers = new List<GameObject>();
    public enum FocusMode
    {
        moveToFocusXYZ = 1,
        angleToFocus = 2,
        rotateToFocus = 3,
        moveToFocusY = 4,
        moveToFocusXZ = 5,
        moveToFocusXZplusAngle = 6,
        fullRotate = 7,
        None = 15
    }
    public FocusMode focusMode;

    void Start()
    {
        MainCameraControl = this;
        cam = GetComponent<Camera>();
        DefaultCameraFieldView = cam.fieldOfView;
        Debug.Log(MainCameraControl);
    }
    private void Update()
    {
        SpriteRotater();
    }
    private void FixedUpdate()
    {
        switch ((int)focusMode)
        {
            case 1: //XYZ
                transform.position = focusObject.transform.position + Placement;
                transform.rotation = Quaternion.Euler(35, 0, 0);
                break;
            case 2: //Angle
                if (!((Mathf.Rad2Deg * Mathf.Atan2(transform.position.z - focusObject.transform.position.z, transform.position.y - focusObject.transform.position.y)) + 90 > MAX_ANGLE_X))
                transform.rotation = Quaternion.Euler((Mathf.Rad2Deg * Mathf.Atan2(transform.position.z - focusObject.transform.position.z, transform.position.y - focusObject.transform.position.y)) + 90, 0, 0);
                break;
            case 3: //Rotate
                transform.rotation = Quaternion.Euler(35, (Mathf.Rad2Deg * Mathf.Atan2(transform.position.x - focusObject.transform.position.x, transform.position.z - focusObject.transform.position.z)) + 180, 0);
                break;
            case 4: //Y
                transform.position = new Vector3(transform.position.x, focusObject.transform.position.y + Placement.y, transform.position.z);
                transform.rotation = Quaternion.Euler(35, 0, 0);
                break;
            case 5: //XZ
                transform.position = new Vector3(focusObject.transform.position.x + Placement.x, Placement.y + PLAYER_GROUND_LEVEL, focusObject.transform.position.z + Placement.z);
                transform.rotation = Quaternion.Euler(35, 0, 0);
                break;
            case 6: //XZ + ANGLE
                transform.position = new Vector3(focusObject.transform.position.x + Placement.x, Placement.y + PLAYER_GROUND_LEVEL, focusObject.transform.position.z + Placement.z);
                if (!((Mathf.Rad2Deg * Mathf.Atan2(transform.position.z - focusObject.transform.position.z, transform.position.y - focusObject.transform.position.y)) + 90 > MAX_ANGLE_X))
                    transform.rotation = Quaternion.Euler((Mathf.Rad2Deg * Mathf.Atan2(transform.position.z - focusObject.transform.position.z, transform.position.y - focusObject.transform.position.y)) + 90, 0, 0);
                break;
            case 7: //FULL ROTATE
                    transform.rotation = Quaternion.Euler((Mathf.Rad2Deg * Mathf.Atan2(transform.position.z - focusObject.transform.position.z, transform.position.y - focusObject.transform.position.y)) + 90, (Mathf.Rad2Deg * Mathf.Atan2(transform.position.x - focusObject.transform.position.x, transform.position.z - focusObject.transform.position.z)) + 180, transform.rotation.z);
                break;

            default: break;
        }
    }

    private void SpriteRotater()
    {
        foreach (GameObject obj in spriteRenderers.ToArray())
        {
            if (obj.GetComponent<SpriteRenderer>() == null)
                spriteRenderers.Remove(obj);
        }
        foreach (GameObject spriteObj in spriteRenderers.ToArray())
        {
            spriteObj.transform.localRotation = Quaternion.Euler((Mathf.Atan2( //X
                transform.position.z - spriteObj.transform.position.z, 
                transform.position.y - spriteObj.transform.position.y) * (180 / Mathf.PI)) + 90,
                (Mathf.Rad2Deg * Mathf.Atan2(transform.position.x - spriteObj.transform.position.x, transform.position.z - spriteObj.transform.position.z)) + 180,
                spriteObj.transform.localRotation.z);
        }
    }
}