using System;
using System.Collections;
using System.Collections.Generic;
using EntityDataSystem;
using GameManagerSystem;
using UnityEngine;
using UnityEngine.Events;
using static TerrainGeneration;
using static NavMeshUpdate;
using System.Linq;
using System.Threading.Tasks;

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
        [SerializeField]
        private int _maxPoints;
        public int maxPoints { set { _maxPoints = (int)(((size ^ 2) * (int)PlayerPreferences.GetDifficulty() * 100) * info.entityDensity); } get { return (int)(((size ^ 2) * (int)PlayerPreferences.GetDifficulty() * 100) * info.entityDensity); } }
        public int usedPoints;

        public bool isFirstRoom => id == 1;
        public bool isLastRoom => id == Instance.rooms.Count;

        public bool roomEntered = false;
        public bool roomCompleted = false;

        public Vector2Int position;

        public Vector2Int LeftDownCornerPosition;
        public Vector2Int RightUpCornerPosition;
        public Vector2Int LeftDownCornerPositionInternal => LeftDownCornerPosition + new Vector2Int(1, 1);
        public Vector2Int RightUpCornerPositionInternal => RightUpCornerPosition - new Vector2Int(1, 1);

        public int size;

        public List<GameObject> blocks = new List<GameObject>();

        public List<Door> doors;
        [HideInInspector]
        [SerializeReference]
        public List<RoomNode> roomChildren;

        public BoxCollider RoomArea => GetComponent<BoxCollider>();

        #region Room config
        public void NewRoomNode(byte id, Vector2Int position, int size)
        {
            this.id = id;
            this.position = position;
            this.size = size;
            doors = new List<Door>();
            blocks = new List<GameObject>();
        }
        public async void SetRoomInfo(RoomInfo info)
        {
            this.info = info;
            await Task.Delay(1);
            for (int x = 0; x <= size - 1; x++)
            {
                for (int y = 0; y <= size - 1; y++)
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
        public void SetSize(int size)
        {
            this.size = size;
            doors = new List<Door>();
        }
        #endregion

        public void Start()
        {
            usedPoints = 0;
            maxPoints = maxPoints;
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
            bool maxEntitiesReached = false;
            List<GameObject> entityList = new List<GameObject>();

            if (info.entities.Count > 0)
            {
                entityList = info.entities.ToList();
            }
            else
            {
                entityList = Instance.biome.defaultRoom.entities.ToList();
            }
            while (maxEntitiesReached == false)
            {
                if (entityList.Count == 0)
                    break;

                returnV:;
                x = UnityEngine.Random.Range(LeftDownCornerPositionInternal.x, RightUpCornerPositionInternal.x);
                y = UnityEngine.Random.Range(LeftDownCornerPositionInternal.y, RightUpCornerPositionInternal.y);
                if (posList.Contains(new Vector2Int(x, y)) || Instance.spawnTiles[x, y])
                    goto returnV;
                posList.Add(new Vector2Int(x, y));

                var random = UnityEngine.Random.Range(0, entityList.Count);
                if (entityList[random] != null)
                {
                    if (entityList[random].GetComponent<IEntity>().EntityData.currentKarma == 0)
                        throw new Exception("Entity has 0 karma, this is not allowed.");
                    if (usedPoints + Mathf.Abs(entityList[random].GetComponent<IEntity>().EntityData.currentKarma) < maxPoints)
                    {
                        usedPoints += entityList[random].GetComponent<IEntity>().EntityData.currentKarma;
                        GameManager.gameManagerInstance.SpawnEntity(new Vector2(x + 0.5f, y + 0.5f), entityList[random]);
                    }
                    else
                    {
                        entityList.RemoveAt(random);
                    }
                }
                else
                {
                    entityList.RemoveAt(random);
                }
            }

            foreach (Door door in doors)
            {
                door.doorBlock.GetComponent<Animator>().Play("DoorClose");
            }
            foreach (var entity in GameManager.gameManagerInstance.enemies)
            {
                entity.GetComponent<IEntity>().OnDeathEvent.AddListener((a, b) => { if (GameManager.gameManagerInstance.enemies.Count == 0) CompleteRoom(); });
            }

            SoundManager.PlayMusic("No_Mercy");
            Instance.RoomOcclusion(id);
        }
        private void RoomCompletionFunction()
        {
            GameManager.gameManagerInstance.ClearEnemyBullets();
            foreach (Door door in doors)
            {
                door.doorBlock.GetComponent<Animator>().Play("DoorOpen");
            }

            SoundManager.StopMusic();
            GameManager.UpdatePlayerMaxKarma(usedPoints);
            Instance.RoomOcclusion(-1);
        }
        public void CompleteRoom()
        {
            roomCompleted = true;
            OnRoomCompletion.Invoke();
        }
        public override string ToString()
        {
            return "Room ID: " + id + ", X: " + position.x + ", Y: " + position.y + ", Size: " + size;
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