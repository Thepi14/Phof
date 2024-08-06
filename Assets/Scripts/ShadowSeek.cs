using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainGeneration;
using static TextureFunction;

public class ShadowSeek : MonoBehaviour
{
    public GameObject shadowCastObject = null;
    public Vector3 Scale = Vector2.one;
    public Vector2 Displacement = Vector2.zero;

    void Update()
    {
        transform.localScale = Scale;
        var t = shadowCastObject.transform.position;

        if (!IsInside2DArray((int)t.x, (int)t.z, Instance.blocks))
        {
            transform.localPosition = new Vector3(0, -1000, 0); // :O omagaaaaaa 
            return;
        }//:D America Ya

        if (shadowCastObject != null && Instance != null)
        {
            transform.localPosition = new Vector3(
                0 + Displacement.x,
                Instance.GetBlockObj((int)t.x, (int)t.z) != null ? 
                (Instance.GetBlockObj((int)t.x, (int)t.z).transform.position.y - shadowCastObject.transform.position.y) + 0.55f : -1000,
                0 + Displacement.y);
        }
    }
}
