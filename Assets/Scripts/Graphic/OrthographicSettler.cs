using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthographicSettler : MonoBehaviour
{
    private Camera Camera => GetComponent<Camera>();
    private void Update() => Camera.orthographic = PlayerPreferences.Orthographic;
}
