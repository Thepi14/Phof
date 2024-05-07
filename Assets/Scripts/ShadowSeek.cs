using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainGeneration;

public class ShadowSeek : MonoBehaviour
{
    public GameObject shadowCastObject = null;
    void Update()
    {
        var t = shadowCastObject.transform.position;

        if (!Instance.IsInsideArray((int)t.x, (int)t.z, Instance.blocks))
        {
            transform.localPosition = new Vector3(0, -1000, 0); // :O omagaaaaaa 
            return;
        }//:D America Ya

        if (shadowCastObject != null && Instance != null)
        {
            transform.localPosition = new Vector3(
                0,
                Instance.GetBlockObj((int)t.x, (int)t.z) != null ? 
                (Instance.GetBlockObj((int)t.x, (int)t.z).transform.position.y - shadowCastObject.transform.position.y) + 0.55f : -1000,
                0);
        }
    }
}
