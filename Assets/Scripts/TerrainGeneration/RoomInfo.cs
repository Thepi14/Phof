using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EntityDataSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New room", menuName = "Bioma/Nova sala", order = 1)]
public class RoomInfo : ScriptableObject
{
    public List<GameObject> entities = new List<GameObject>();
    /// <summary>
    /// Calcula a densidade de entidades inimigas por quadrado.
    /// </summary>
    public float entityDensity = 0.2f;
    public Vector2Int size;
    public bool universal = false;

    private void OnValidate()
    {
        foreach (GameObject obj in entities.ToList())
        {
            if (obj != null && obj.GetComponent<IEntity>() == null)
            {
                entities.Remove(obj);
                continue;
            }
        }
        size.x = Mathf.Max(size.x, 0);
        size.y = Mathf.Max(size.y, 0);
        universal = name.ToLower() == "default";
    }
}
