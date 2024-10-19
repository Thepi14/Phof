using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Security.Cryptography;
using UnityEngine.EventSystems;

namespace ObjectUtils
{
    public static class GameObjectGeneral
    {
        /// <summary>
        /// Acha um GameObject por hierarquia nos transforms como se fosse um arquivo em pastas.
        /// </summary>
        /// <param name="start">Gameobject de referência.</param>
        /// <param name="path">Caminho do GameObject separados por '\' ou '/'.</param>
        /// <returns>O GameObject no caminho inserido.</returns>
        public static GameObject GetGameObject(GameObject start, string path)
        {
            if (start == null)
                throw new System.ArgumentNullException("GameObject of name \'" + start.name + "\' is null", "start");
            //lista de nomes
            List<string> names = new List<string>();
            //lista de caracteres do path
            char[] chars = path.ToCharArray();
            //nome temporário em chars
            List<char> temp = new List<char>();
            //iterar sobre os chars
            for (int i = 0; i < path.Length; i++)
            {
                //se for uma separação
                if (chars[i] == '\\' || chars[i] == '/')
                {
                    //adiciona um nome
                    names.Add(new string(temp.ToArray()));
                    //renova a temp
                    temp = new List<char>();
                    //continua
                    continue;
                }
                //adiciona novo char se não for uma separação
                temp.Add(chars[i]);
            }
            //última iteração adicionada como último destino do caminho.
            names.Add(new string(temp.ToArray()));
            //go se torna referência inicial
            GameObject go = start;
            //iterar sobre cada nome
            foreach (string name in names)
            {
                //se o filho de go for nulo
                if (go.transform.Find(name) == null)
                    throw new System.Exception(name + " was not been found or it doesn't exist on the path: " + path);
                //go é igual ao gameobject achado como filho do seu transform
                go = go.transform.Find(name).gameObject;
            }
            if (go == null)
                throw new System.Exception("GameObject has not been found on the path.");
            //retornar go
            return go;
        }
        /// <summary>
        /// Acha o componente designado de um GameObject por hierarquia nos transforms como se fosse um arquivo em pastas.
        /// </summary>
        /// <typeparam name="T">O componente.</typeparam>
        /// <param name="start">Gameobject de referência.</param>
        /// <param name="path">Caminho do GameObject separados por '\' ou '/'.</param>
        /// <returns>O componente do GameObject no caminho inserido.</returns>
        public static T GetGameObjectComponent<T>(GameObject start, string path)
        {
            if (start == null)
                throw new System.ArgumentNullException("GameObject of name \'" + start.name + "\' is null", "start");
            //lista de nomes
            List<string> names = new List<string>();
            //lista de caracteres do path
            char[] chars = path.ToCharArray();
            //nome temporário em chars
            List<char> temp = new List<char>();
            //iterar sobre os chars
            for (int i = 0; i < path.Length; i++)
            {
                //se for uma separação
                if (chars[i] == '\\' || chars[i] == '/')
                {
                    //adiciona um nome
                    names.Add(new string(temp.ToArray()));
                    //renova a temp
                    temp = new List<char>();
                    //continua
                    continue;
                }
                //adiciona novo char se não for uma separação
                temp.Add(chars[i]);
            }
            //última iteração adicionada como último destino do caminho.
            names.Add(new string(temp.ToArray()));
            //go se torna referência inicial
            GameObject go = start;
            //iterar sobre cada nome
            foreach (string name in names)
            {
                //se o filho de go for nulo
                if (go.transform.Find(name) == null)
                    throw new System.Exception(name + " was not been found or it doesn't exist on the path: " + path);
                //go é igual ao gameobject achado como filho do seu transform
                go = go.transform.Find(name).gameObject;
            }
            if (go == null)
                throw new System.Exception("GameObject has not been found on the path.");
            if (go.GetComponent<T>() == null)
                throw new System.Exception("Component has not been found on " + go.name);
            //retornar go
            return go.GetComponent<T>();
        }
        /// <summary>
        /// Acha um componente em um filho de um GameObject parente.
        /// </summary>
        /// <typeparam name="T">O componente.</typeparam>
        /// <param name="parent">O GameObject parente.</param>
        /// <param name="childName">Nome do filho.</param>
        /// <returns>O componente do GameObject filho do parent escolhido com o nome inserido.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static T FindComponentInChild<T>(GameObject parent, string childName)
        {
            if (parent == null)
                throw new System.ArgumentNullException("Parent of name \'" + parent.name + "\' is null");
            if (parent.transform.Find(childName) == null)
                throw new System.ArgumentNullException("Child of name \'" + childName + "\' was not found on the \'" + parent.name + "\' GameObject");
            if (parent.transform.Find(childName).GetComponent<T>() == null)
                throw new System.ArgumentNullException("Component was not been found in child of name \'" + childName + "\' of parent \'" + parent.name + "\'");
            return parent.transform.Find(childName).GetComponent<T>();
        }
    }
    public static class UIGeneral
    {
        /// <returns>Retorna a posição do mouse com o fator de escala do canvas.</returns>
        public static Vector2 MousePositionScaled()
        {
            return Mouse.current.position.ReadValue() * FindCanvas().scaleFactor;
        }
        /// <returns>Retorna a posição do mouse.</returns>
        public static Vector2 MousePosition()
        {
            return Mouse.current.position.ReadValue();
        }
        /// <returns>Retorna o canvas pelo nome "Canvas" (deve ter somente 1 canvas).</returns>
        public static Canvas FindCanvas()
        {
            return GameObject.Find("Canvas").GetComponent<Canvas>();
        }
        /// <summary>
        /// Fator de escala do canvas.
        /// </summary>
        public static float CanvasScaleFactor => FindCanvas().scaleFactor;

        private const int UILayer = 5;
        //Returns 'true' if we touched or hovering on Unity UI element.
        public static bool IsPointerOverUIElement() => IsPointerOverUIElement(GetEventSystemRaycastResults());
        //Returns 'true' if we touched or hovering on Unity UI element.
        private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == UILayer)
                    return true;
            }
            return false;
        }
        //Gets all event system raycast results of current mouse or touch position.
        public static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }
    public static class MathEx
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }

        public static Vector2 AngleVectors(Vector2 a, Vector2 b)
        {
            return RadianToVector2(Mathf.Atan2(a.y - b.y, a.x - b.x));
        }
        public static Vector2 AngleVectors(Vector3 a, Vector3 b)
        {
            return RadianToVector2(Mathf.Atan2(a.z - b.z, a.x - b.x));
        }
        public static float AngleRadian(Vector3 a, Vector3 b)
        {
            return Mathf.Atan2(a.z - b.z, a.x - b.x);
        }
    }
}