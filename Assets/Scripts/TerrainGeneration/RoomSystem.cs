using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomSystem
{
    public class RoomNode
    {
        public int id;

        public Vector2Int position;

        public Vector2Int LeftDownCornerPosition;
        public Vector2Int RightUpCornerPosition;

        public int width;
        public int height;

        public List<Door> doors;
        public List<RoomNode> roomChildren;

        public RoomNode(int id, Vector2Int position, int width, int height)
        {
            this.id = id;
            this.position = position;
            this.width = width;
            this.height = height;
            doors = new List<Door>();
        }
        public override string ToString()
        {
            return "Room ID: " + id + ", X: " + position.x + ", Y: " + position.y + ", W: " + width + ", H: " + height;
        }
    }
    public class Door
    {
        public List<GameObject> doorBlocks;
        public Vector2Int position;
        /// <summary>
        /// Determina para onde a porta está virada.
        /// <para>X(1) = Direita, Y(1) = Cima, X(-1) = Esquerda, Y(-1) = Baixo.</para>
        /// </summary>
        public Vector2Int facing
        {
            get { return _facing; }
            set
            {
                if (value.x >= 1)
                {
                    _facing.x = Mathf.Min(value.x, 1);
                    _facing.y = 0;
                }
                else if (value.x <= -1)
                {
                    _facing.x = Mathf.Max(value.x, -1);
                    _facing.y = 0;
                }
                if (value.y >= 1)
                {
                    _facing.y = Mathf.Min(value.y, 1);
                    _facing.x = 0;
                }
                else if (value.y <= -1)
                {
                    _facing.y = Mathf.Max(value.y, -1);
                    _facing.x = 0;
                }
            }
        }
        private Vector2Int _facing;
        public Door(Vector2Int position, Vector2Int facing)
        {
            this.position = position;
            this.facing = facing;
        }
    }
}
