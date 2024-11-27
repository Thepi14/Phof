using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem
{
    public class ItemDrop : MonoBehaviour
    {
        public Item item;
        public const float MAX_TIME_ALIVE = 300f;
        public float timer;
        public SpriteRenderer SpriteRenderer => GetComponent<SpriteRenderer>();

        void Start()
        {
            CameraControl.MainCameraControl.spriteRenderers.Add(gameObject);
        }
        void Update()
        {
            timer += Time.deltaTime;
            if (timer > MAX_TIME_ALIVE)
            {
                timer = 0;
                Destroy(transform.parent.gameObject);
            }
        }
        public void StartItem(Item item)
        {
            this.item = item;
            if (item != null)
            {
                if (item.itemSprite)
                    SpriteRenderer.sprite = item.itemSprite;
            }
        }
    }
}
