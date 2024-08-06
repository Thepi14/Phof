using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GamePlayer;
using static TerrainGeneration;
using Pathfindingsystem;

public class Enemy : MonoBehaviour
{
    public EntityData entityData;
    private Rigidbody RB => GetComponent<Rigidbody>();
    private Pathfinding pathfinding;
    private List<PathNode> path;

    public bool freezed = false;

    void Start()
    {
        entityData ??= new EntityData();
        pathfinding = new Pathfinding(Instance.MapWidth, Instance.MapHeight);
    }
    void FixedUpdate()
    {
        if (!freezed)
            Move(player.transform.position, entityData.speed);
    }
    /// <summary>
    /// Move a entidade em direção a algo com uma velocidade usando pathfinding.
    /// </summary>
    /// <param name="target">Alvo</param>
    /// <param name="speed">Velocidade</param>
    public void Move(Vector3 target, float speed)
    {
        if (path == null || Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(path[1].x + 0.5f, path[1].y + 0.5f)) < 0.05f || Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(path[1].x + 0.5f, path[1].y + 0.5f)) > 1.75f)
        {
            path = pathfinding.FindPath((int)transform.position.x, (int)transform.position.z, (int)target.x, (int)target.z, (int x, int y) =>
            {
                return Instance.physicalMap.GetPixel(x, y) == Color.white;
            });
            if (path == null)
            {
                RB.velocity = new Vector3(0, 0, 0);
                return;
            }
        }
        Vector2 vec = MathEx.RadianToVector2(Mathf.Atan2(
                path[1].y + 0.5f - transform.position.z,
                path[1].x + 0.5f - transform.position.x)) * speed;
        RB.velocity = new Vector3(vec.x, 0, vec.y);
    }
}

namespace UnityEngine
{
    public struct MathEx
    {
        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }
    }
}