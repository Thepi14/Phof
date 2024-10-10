using System;
using System.Collections;
using System.Collections.Generic;
using GameManagerSystem;
using UnityEngine;
using UnityEngine.Events;

namespace RoomSystem
{
    [Serializable]
    public class RoomNode : MonoBehaviour
    {
        public class RoomCompletionEvent : UnityEvent { }
        public RoomCompletionEvent OnRoomCompletion = new RoomCompletionEvent();
        public class RoomStartEvent : UnityEvent { }
        public RoomStartEvent OnRoomStart = new RoomStartEvent();

        public int id;

        public bool isFirstRoom => id == 1;
        public bool roomEntered = false;
        public bool roomCompleted = false;

        public Vector2Int position;

        public Vector2Int LeftDownCornerPosition;
        public Vector2Int RightUpCornerPosition;

        public int width;
        public int height;

        public List<Door> doors;
        [HideInInspector]
        [SerializeReference]
        public List<RoomNode> roomChildren;

        public BoxCollider RoomArea => GetComponent<BoxCollider>();

        #region Room config
        public void NewRoomNode(int id, Vector2Int position, int width, int height)
        {
            this.id = id;
            this.position = position;
            this.width = width;
            this.height = height;
            doors = new List<Door>();
        }
        public void SetPosID(int ID, Vector2Int position)
        {
            this.id = ID;
            this.position = position;
            doors = new List<Door>();
        }
        public void SetSize(int width, int height)
        {
            this.width = width;
            this.height = height;
            doors = new List<Door>();
        }
        #endregion

        public void Start()
        {
            OnRoomCompletion.AddListener(() => RoomCompletionFunction());
            OnRoomStart.AddListener(() => RoomStartFunction());
        }
        public void OnTriggerEnter(Collider other)
        {
            switch (other.gameObject.layer)
            {
                case 8:
                    Debug.Log("Player entered room number " + id);
                    if (!roomCompleted && !isFirstRoom)
                    {
                        roomEntered = true;
                        foreach (Door door in doors)
                        {
                            door.doorBlock.GetComponent<Animator>().Play("DoorClose");
                        }
                        GameManager.gameManagerInstance.currentRoom = this;
                    }
                    break;
            }
        }
        public void OnTriggerExit(Collider other)
        {
            switch (other.gameObject.layer)
            {
                case 8:

                    break;
            }
        }
        private void RoomStartFunction()
        {

        }
        private void RoomCompletionFunction()
        {
            foreach (Door door in doors)
            {
                door.doorBlock.GetComponent<Animator>().Play("DoorOpen");
            }
        }
        public void CompleteRoom()
        {
            roomCompleted = true;
            OnRoomCompletion.Invoke();
        }
        public override string ToString()
        {
            return "Room ID: " + id + ", X: " + position.x + ", Y: " + position.y + ", W: " + width + ", H: " + height;
        }
    }
    [Serializable]
    public class Door
    {
        public GameObject doorBlock;
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
