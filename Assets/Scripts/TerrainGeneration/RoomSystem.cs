using System;
using System.Collections;
using System.Collections.Generic;
using EntityDataSystem;
using GameManagerSystem;
using UnityEngine;
using UnityEngine.Events;
using static TerrainGeneration;
using static NavMeshUpdate;

namespace RoomSystem
{
    [Serializable]
    public class RoomNode : MonoBehaviour
    {
        public RoomInfo info;
        public class RoomCompletionEvent : UnityEvent { }
        public RoomCompletionEvent OnRoomCompletion = new RoomCompletionEvent();
        public class RoomStartEvent : UnityEvent { }
        public RoomStartEvent OnRoomStart = new RoomStartEvent();

        public byte id;

        public bool isFirstRoom => id == 1;
        public bool isLastRoom => id == TerrainGeneration.Instance.rooms.Count;

        public bool roomEntered = false;
        public bool roomCompleted = false;

        public Vector2Int position;

        public Vector2Int LeftDownCornerPosition;
        public Vector2Int RightUpCornerPosition;
        public Vector2Int LeftDownCornerPositionInternal => LeftDownCornerPosition + new Vector2Int(1, 1);
        public Vector2Int RightUpCornerPositionInternal => RightUpCornerPosition - new Vector2Int(1, 1);

        public int width;
        public int height;

        public List<GameObject> blocks = new List<GameObject>();

        public List<Door> doors;
        [HideInInspector]
        [SerializeReference]
        public List<RoomNode> roomChildren;

        public BoxCollider RoomArea => GetComponent<BoxCollider>();

        #region Room config
        public void NewRoomNode(byte id, Vector2Int position, int width, int height)
        {
            this.id = id;
            this.position = position;
            this.width = width;
            this.height = height;
            doors = new List<Door>();
            blocks = new List<GameObject>();
        }
        public void SetRoomInfo()
        {
            for (int x = 0; x <= width - 1; x++)
            {
                for (int y = 0; y <= height - 1; y++)
                {
                    var xP = x + LeftDownCornerPositionInternal.x;
                    var yP = y + LeftDownCornerPositionInternal.y;

                    Instance.spawnTiles[xP, yP] = info.grid[x, y].entityCanSpawn;

                    if (info.grid[x, y].id == 0)
                        continue;
                    var gInfo = info.grid[x, y];
                    var block = Instance.biome.generalBlocks[gInfo.id - 1];
                    Instance.PlaceBlock(new Vector3(xP, gInfo.height + 1f, yP), block, true, gInfo.rotation, gInfo.scale);
                }
            }
        }
        public void SetPosID(byte id, Vector2Int position)
        {
            this.id = id;
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
                    roomEntered = true;
                    if (isLastRoom)
                    {
                        GameManager.gameManagerInstance.NextStage();
                        return;
                    }
                    if (!roomCompleted)
                    {
                        if (!isFirstRoom)
                        {
                            foreach (Door door in doors)
                            {
                                door.doorBlock.GetComponent<Animator>().Play("DoorClose");
                            }
                            OnRoomStart.Invoke();
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
                    roomEntered = false;
                    break;
            }
        }
        private void RoomStartFunction()
        {
            var posList = new List<Vector2Int>();
            var x = 0;
            var y = 0;

            for (int i = 0; i < (width + height) * info.entityDensity; i++)
            {
            returnV:;
                x = UnityEngine.Random.Range(LeftDownCornerPositionInternal.x, RightUpCornerPositionInternal.x);
                y = UnityEngine.Random.Range(LeftDownCornerPositionInternal.y, RightUpCornerPositionInternal.y);

                if (posList.Contains(new Vector2Int(x, y)) || Instance.spawnTiles[x, y])
                    goto returnV;

                if (info.entities.Count > 0)
                    GameManager.gameManagerInstance.SpawnEntity(new Vector2(x + 0.5f, y + 0.5f), info.entities[UnityEngine.Random.Range(0, info.entities.Count)]);
                else
                    GameManager.gameManagerInstance.SpawnEntity(new Vector2(x + 0.5f, y + 0.5f), Instance.biome.defaultRoom.entities[UnityEngine.Random.Range(0, info.entities.Count)]);

                posList.Add(new Vector2Int(x, y));
            }

            foreach (var entity in GameManager.gameManagerInstance.enemies)
            {
                entity.GetComponent<IEntity>().OnDeathEvent.AddListener((a, b) => { if (GameManager.gameManagerInstance.enemies.Count == 0) CompleteRoom(); });
            }

            Instance.RoomOcclusion(id);
        }
        private void RoomCompletionFunction()
        {
            GameManager.gameManagerInstance.ClearEnemyBullets();
            foreach (Door door in doors)
            {
                door.doorBlock.GetComponent<Animator>().Play("DoorOpen");
            }

            Instance.RoomOcclusion(-1);
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