using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

namespace InputManagement
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;
        public List<KeyBind> keyBindList;
        public static List<KeyBind> defaultList => new()
        {
            new KeyBind(KeyCode.Escape, KeyBindKey.Escape),
            new KeyBind(KeyCode.Mouse0, KeyBindKey.Attack),
            new KeyBind(KeyCode.P, KeyBindKey.Pause),
            new KeyBind(KeyCode.E, KeyBindKey.Inventory),
            new KeyBind(KeyCode.Q, KeyBindKey.ItemHotbar),
            /*new KeyBind(KeyCode.UpArrow, KeyBindKey.Up),
            new KeyBind(KeyCode.LeftArrow, KeyBindKey.Left),
            new KeyBind(KeyCode.DownArrow, KeyBindKey.Down),
            new KeyBind(KeyCode.RightArrow, KeyBindKey.Right),*/
            new KeyBind(KeyCode.W, KeyBindKey.Up),
            new KeyBind(KeyCode.A, KeyBindKey.Left),
            new KeyBind(KeyCode.S, KeyBindKey.Down),
            new KeyBind(KeyCode.D, KeyBindKey.Right),
        };

        private void Start()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            DontDestroyOnLoad(Instance);
            LoadKeyBinds();
        }
        public static bool GetKeyDown(KeyBindKey key)
        {
            Func<KeyBind, bool> predicate = new Func<KeyBind, bool>((keyBind) => { return keyBind.bind == key; });
            foreach (var keyBind in Instance.keyBindList.Where(predicate))
            {
                if (Input.GetKeyDown(keyBind.key)) return true;
            }
            return false;
        }
        public static bool GetKey(KeyBindKey key)
        {
            Func<KeyBind, bool> predicate = new Func<KeyBind, bool>((keyBind) => { return keyBind.bind == key; });
            foreach (var keyBind in Instance.keyBindList.Where(predicate))
            {
                if (Input.GetKey(keyBind.key)) return true;
            }
            return false;
        }
        public static bool GetKeyUp(KeyBindKey key)
        {
            Func<KeyBind, bool> predicate = new Func<KeyBind, bool>((keyBind) => { return keyBind.bind == key; });
            foreach (var keyBind in Instance.keyBindList.Where(predicate))
            {
                if (Input.GetKeyUp(keyBind.key)) return true;
            }
            return false;
        }
        public static KeyCode GetKeyCode()
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(key))
                    return key;
            }
            return KeyCode.None;
        }
        public static void SaveKeyBinds()
        {
            foreach (var keyBind in Instance.keyBindList)
            {
                PlayerPrefs.SetInt(keyBind.bind.ToString(), (int)keyBind.key);
            }
            PlayerPrefs.Save();
        }
        public static void LoadKeyBinds()
        {
            Instance.keyBindList = new List<KeyBind>();

            foreach (KeyBindKey keyBind in Enum.GetValues(typeof(KeyBindKey)))
            {
                Instance.keyBindList.Add(new KeyBind((KeyCode)PlayerPrefs.GetInt(keyBind.ToString(), 0), keyBind));
            }
        }
        public static void AddKeyBind(KeyBind newBind)
        {
            Instance.keyBindList.Add(newBind);
        }
        public static void ReplaceKeyBind(KeyBind newBind)
        {
            RemoveKeyBind(newBind);
            Instance.keyBindList.Add(newBind);
        }
        public static void RemoveKeyBind(KeyBind keyBind) => RemoveKeyBind(keyBind.bind);
        public static void RemoveKeyBind(KeyBindKey keyBindKey)
        {
            foreach (var keyBind in Instance.keyBindList)
            {
                if (keyBind.bind == keyBindKey)
                    Instance.keyBindList.Remove(keyBind);
            }
        }
        public static void ResetKeyBindToDefault()
        {
            Instance.keyBindList = defaultList;
            SaveKeyBinds();
        }
        [SerializeField]
        private float axisChangeSpeed = 3f;
        [SerializeField]
        private Vector2 currentAxis;
        public static Vector2 GetAxis()
        {
            float h = (GetKey(KeyBindKey.Right) ? 1 : GetKey(KeyBindKey.Left) ? -1 : 0);
            float v = (GetKey(KeyBindKey.Up) ? 1 : GetKey(KeyBindKey.Down) ? -1 : 0);
            Instance.currentAxis = SmoothInput(h, v);
            return Instance.currentAxis;
        }
        private static float slidingH;
        private static float slidingV;
        private static Vector2 SmoothInput(float targetH, float targetV)
        {
            float deadZone = 0.001f;

            slidingH = Mathf.Lerp(slidingH,
                          targetH, Instance.axisChangeSpeed * Time.deltaTime);

            slidingV = Mathf.Lerp(slidingV,
                          targetV, Instance.axisChangeSpeed * Time.deltaTime);
            return new Vector2(
                   (Mathf.Abs(slidingH) < deadZone) ? 0f : slidingH,
                   (Mathf.Abs(slidingV) < deadZone) ? 0f : slidingV);
        }
    }
    public enum KeyBindKey : short
    {
        None = 0,
        Escape = 1,
        Attack = 2,
        Pause = 3,
        Inventory = 4,
        ItemHotbar = 5,
        Up = 6,
        Down = 7,
        Left = 8,
        Right = 9,
    }
    [Serializable]
    public class KeyBind
    {
        //public string name => key.ToString();
        public KeyCode key;
        public KeyBindKey bind;

        public KeyBind(KeyCode key, KeyBindKey bind)
        {
            this.key = key;
            this.bind = bind;
        }
    }
}
