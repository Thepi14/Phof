using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class Grid<TGridObject>
{
    public event EventHandler<OnGridValueChangedEventArgs> onGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private TGridObject[,] gridArray;
    private Func<Grid<TGridObject>, int, int, TGridObject> CreateGridObjectFunc;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="CreateGridObject"></param>
    public Grid(int width, int height, Func<Grid<TGridObject>, int, int, TGridObject> CreateGridObject)
    {
        this.width = width;
        this.height = height;

        gridArray = new TGridObject[width, height];
        CreateGridObjectFunc = CreateGridObject;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridArray[x, y] = CreateGridObject(this, x, y);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public TGridObject[,] GetArray() => gridArray;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void GetXY(Vector2 position, out int x, out int y)
    {
        x = (int)position.x;
        y = (int)position.y;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="value"></param>
    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (IsInsideArray(x, y))
            gridArray[x, y] = value;
        if (onGridValueChanged != null)
            onGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="value"></param>
    public void SetGridObject(Vector2 position, TGridObject value)
    {
        if (IsInsideArray((int)position.x, (int)position.y))
            gridArray[(int)position.x, (int)position.y] = value;
        if (onGridValueChanged != null) onGridValueChanged(this, new OnGridValueChangedEventArgs { x = (int)position.x, y = (int)position.y });
    }
    /*public void SetGridObject(Vector3 position, TGridObject value)
    {
        if (CheckIfInsideArrayLimits((int)position.x, (int)position.y))
            gridArray[(int)position.x, (int)position.y] = value;
        if (onGridValueChanged != null)
            onGridValueChanged(this, new OnGridValueChangedEventArgs { x = (int)position.x, y = (int)position.y });
    }*/
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void TriggerGridObjectChanged(int x, int y)
    {
        if (onGridValueChanged != null) onGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public TGridObject GetGridObject(int x, int y)
    {
        if (IsInsideArray(x, y))
            return gridArray[x, y];
        else
            return default;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public TGridObject GetGridObject(Vector2 position)
    {
        GetXY(position, out int x, out int y);
        return GetGridObject(x, y);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetWidth() => width;
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetHeight() => height;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="array"></param>
    public void ArrayToGrid(TGridObject[,] array) => gridArray = array;
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<TGridObject> GridToList()
    {
        var list = new List<TGridObject>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                list.Add(GetGridObject(x, y));
            }
        }
        return list;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="width"></param>
    /// <exception cref="SystemException"></exception>
    public void ListToGrid(List<TGridObject> list, int width)
    {
        if (list.Count % width != 0)
            throw new SystemException("List is not divisible by width.");
        TGridObject[,] newArray = new TGridObject[this.width, this.height];
        this.width = width;
        height = list.Count / width;
        int index = 0;
        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                newArray[x, y] = list.ToArray()[index];
                index++;
            }
        }
        gridArray = newArray;
        Debug.Log(width + " " + height);
    }
    public void ListToGrid(List<TGridObject> list, int width, Action<int, int, int> action)
    {
        if (list.Count % width != 0)
            throw new SystemException("List is not divisible by width.");
        this.width = width;
        height = list.Count / width;
        TGridObject[,] newArray = new TGridObject[this.width, height];
        int index = 0;
        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                newArray[x, y] = list.ToArray()[index];
                index++;
            }
        }
        gridArray = newArray;
        index = 0;
        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                action(x, y, index);
                index++;
            }
        }
    }
    public void ResizeGrid(int width, int height)
    {
        TGridObject[,] newArray = new TGridObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x >= this.width || y >= this.height)
                {
                    newArray[x, y] = CreateGridObjectFunc(this, x, y);
                    continue;
                }
                newArray[x, y] = gridArray[x, y];
            }
        }
        this.width = width;
        this.height = height;
        gridArray = newArray;
    }
    /// <summary>
    /// Um comparador de cores de 32Bits, que é ausente por algum motivo, então criei o meu.
    /// </summary>
    /// <param name="col1">Cor 1.</param>
    /// <param name="col2">Cor 2.</param>
    /// <returns>Se as duas cores são iguais retorna true, se não false.</returns>
    public static bool Color32Equals(Color32 col1, Color32 col2)
    {
        if (col1.r == col2.r &&
            col1.g == col2.g &&
            col1.b == col2.b &&
            col1.a == col2.a)
            return true;
        return false;
    }
    /// <summary>
    /// Verifica se a posição fornecida está dentro dos limites do array 2D fornecido.
    /// </summary>
    /// <typeparam name="T">O tipo do array, pra suportar todos os tipos e classes, não é necessário colocar o tipo.</typeparam>
    /// <param name="x">Localização X.</param>
    /// <param name="y">Localização Y.</param>
    /// <param name="a">O array que será usado de medida.</param>
    /// <returns></returns>
    public bool IsInsideArray(int x, int y) => x >= 0 && y >= 0 && x < gridArray.GetLength(0) && y < gridArray.GetLength(1);
    public TGridObject this [int i]
    {
        get
        {
            return GridToList()[i];
        }
        set
        {
            var list = GridToList();
            list[i] = value;
            ListToGrid(list, width);
        }
    }
    public TGridObject this [int x, int y]
    {
        get => gridArray[x, y];
        set => gridArray[x, y] = value;
    }
    /*public static Grid operator =(Grid a, TGridObject[,] b)
    {
        for (int x = 0; x < b.GetLength(0); x++)
        {
            for (int y = 0; y < b.GetLength(1); y++)
            {

            }
        }
        return a;
    }*/
}