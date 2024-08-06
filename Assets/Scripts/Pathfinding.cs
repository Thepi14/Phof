using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfindingsystem
{

    /// <summary>
    /// Classe criada para achar os caminhos mais curtos entre dois pontos distintos.
    /// </summary>
    public class Pathfinding
    {
        /// <summary>
        /// 
        /// </summary>
        public int MOVE_STRAIGHT_COST = 10;
        /// <summary>
        /// 
        /// </summary>
        public int MOVE_DIAGONAL_COST = 12;

        private Grid<PathNode> grid;
        private List<PathNode> openList;
        private HashSet<PathNode> closedList;

        /// <summary>
        /// Construtor da classe Pathfinding.
        /// </summary>
        /// <param name="width">Largura da grade do pathfinding, mas também pode ser definido um tamanho customizado dentro da matriz que você estiver usando para economizar processamento.</param>
        /// <param name="height">Altura da grade do pathfinding, mas também pode ser definido um tamanho customizado dentro da matriz que você estiver usando para economizar processamento.</param>
        public Pathfinding(int width, int height)
        {
            grid = new Grid<PathNode>(width, height, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
        }
        /// <summary>
        /// Acha o caminho entre o ponto inicial e o ponto final.
        /// </summary>
        /// <param name="startx">Coordenada inicial X.</param>
        /// <param name="starty">Coordenada inicial Y.</param>
        /// <param name="endx">Coordenada final X.</param>
        /// <param name="endy">Coordenada final Y.</param>
        /// <returns></returns>
        public List<PathNode> FindPath(int startx, int starty, int endx, int endy, Func<int, int, bool> booleaner = null)
        {
            PathNode startNode = grid.GetGridObject(startx, starty);
            PathNode endNode = grid.GetGridObject(endx, endy);

            if (booleaner == null)
                SetGridWalkableNodes();
            else
                SetGridWalkableNodes(booleaner);

            if (endNode != null)
                if (endNode.isWalkable == false)
                {
                    List<List<PathNode>> paths = new List<List<PathNode>>();

                    if (grid.CheckIfInsideGrid(endx - 1, endy))
                        if (grid.GetGridObject(endx - 1, endy).isWalkable)
                            if (FindPathB(startx, starty, endx - 1, endy) != null)
                                paths.Add(FindPathB(startx, starty, endx - 1, endy));

                    if (grid.CheckIfInsideGrid(endx + 1, endy))
                        if (grid.GetGridObject(endx + 1, endy).isWalkable)
                            if (FindPathB(startx, starty, endx + 1, endy) != null)
                                paths.Add(FindPathB(startx, starty, endx + 1, endy));

                    if (grid.CheckIfInsideGrid(endx, endy - 1))
                        if (grid.GetGridObject(endx, endy - 1).isWalkable)
                            if (FindPathB(startx, starty, endx, endy - 1) != null)
                                paths.Add(FindPathB(startx, starty, endx, endy - 1));

                    if (grid.CheckIfInsideGrid(endx, endy + 1))
                        if (grid.GetGridObject(endx, endy + 1).isWalkable)
                            if (FindPathB(startx, starty, endx, endy + 1) != null)
                                paths.Add(FindPathB(startx, starty, endx, endy + 1));

                    bool allPathIsNull = true;
                    foreach (var path in paths)
                    {
                        if (path != null)
                        {
                            allPathIsNull = false;
                            break;
                        }
                    }
                    if (allPathIsNull)
                        goto exitNotWalkablePathDefinition;

                    List<PathNode> finalPath = paths[0];

                    foreach (var path in paths)
                    {
                        if (path != null)
                            if (path.Count < finalPath.Count && path.Count > 1)
                            {
                                finalPath = path;
                            }
                    }
                    return finalPath;
                }
            exitNotWalkablePathDefinition:

            openList = new List<PathNode> { startNode };
            closedList = new HashSet<PathNode>();

            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    PathNode pathNode = grid.GetGridObject(x, y);
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.cameFromNode = null;
                }
            }
            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList);
                if (currentNode == endNode)
                {
                    //Debug.Log("PathFound");
                    return CalculatePath(endNode);
                }
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
                {
                    //Debug.Log(neighbourNode);
                    if (closedList.Contains(neighbourNode)) continue;
                    if (!neighbourNode.isWalkable)
                    {
                        closedList.Add(neighbourNode);
                        continue;
                    }

                    if (currentNode != null && neighbourNode != null) { }
                    else
                        continue;
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();

                        if (!openList.Contains(neighbourNode))
                            openList.Add(neighbourNode);
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Encontra o caminho entre o ponto inicial e o ponto final, mas sem caminhos diagonais.
        /// </summary>
        /// <param name="startx"></param>
        /// <param name="starty"></param>
        /// <param name="endx"></param>
        /// <param name="endy"></param>
        /// <returns></returns>
        public List<PathNode> FindPathB(int startx, int starty, int endx, int endy, Func<int, int, bool> booleaner = null)
        {
            PathNode startNode = grid.GetGridObject(startx, starty);
            PathNode endNode = grid.GetGridObject(endx, endy);

            if (booleaner == null)
                SetGridWalkableNodes();
            else
                SetGridWalkableNodes(booleaner);

            openList = new List<PathNode> { startNode };
            closedList = new HashSet<PathNode>();

            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    PathNode pathNode = grid.GetGridObject(x, y);
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.cameFromNode = null;
                }
            }
            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList);
                if (currentNode == endNode)
                {
                    return CalculatePath(endNode);
                }
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
                {
                    //Debug.Log(neighbourNode);
                    if (closedList.Contains(neighbourNode)) continue;
                    if (!neighbourNode.isWalkable)
                    {
                        closedList.Add(neighbourNode);
                        continue;
                    }

                    if (currentNode != null && neighbourNode != null) { }
                    else
                        continue;
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();

                        if (!openList.Contains(neighbourNode))
                            openList.Add(neighbourNode);
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Retorna a grid de PathNode usada no pathfinding.</returns>
        public Grid<PathNode> GetGrid()
        {
            return grid;
        }
        /// <summary>
        /// Localiza e retorna o Node na coordenada fornecida.
        /// </summary>
        /// <param name="x">Coordenada X.</param>
        /// <param name="y">Coordenada Y.</param>
        /// <returns>O PathNode na coordenada.</returns>
        public PathNode GetNode(int x, int y)
        {
            return grid.GetGridObject(x, y);
        }
        /// <summary>
        /// Define se o node na coordenada será usado ou não no pathfinding, basicamente uma parede.
        /// </summary>
        /// <param name="x">Coordenada X.</param>
        /// <param name="y">Coordenada Y.</param>
        /// <param name="value">Se é passável (true) ou não. (false)</param>
        public void SetWalkable(int x, int y, bool value)
        {
            grid.GetGridObject(x, y).SetWalkableState(value);
        }
        /// <summary>
        /// Define quais nodes são andáveis usando uma função passada como parâmetro.
        /// </summary>
        /// <param name="booleaner">A função que define o "isWalkable" em cada coordenada X (int 0, ou T1) e Y (int 1, ou T2) usando o valor que é retornado do próprio delegate.</param>
        public void SetGridWalkableNodes(Func<int, int, bool> booleaner = null)
        {
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    if (booleaner != null)
                        SetWalkable(x, y, booleaner(x, y));
                    else
                        SetWalkable(x, y, true);
                }
            }
        }
        private List<PathNode> GetNeighbourList(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();
            /*for (int x = -1; x < 1; x++)
            {
                for (int y = -1; y < 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    //Debug.DrawLine(new Vector3 (pathNode.x + x, pathNode.y + y), new Vector3(pathNode.x + x, pathNode.y + y + 0.5f), Color.green, 5f, false);
                    if (grid.CheckIfInsideGrid(currentNode.x + x, currentNode.y + y))
                        neighbourList.Add(grid.GetGridObject(currentNode.x + x, currentNode.y + y));
                }
            }*/
            if (currentNode.x - 1 >= 0)
            {
                // Left
                neighbourList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y));
                if (currentNode.y - 1 >= 0 &&
                    grid.GetGridObject(currentNode.x, currentNode.y - 1).isWalkable &&
                    grid.GetGridObject(currentNode.x - 1, currentNode.y).isWalkable) neighbourList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y - 1));
                if (currentNode.y + 1 < grid.GetHeight() &&
                    grid.GetGridObject(currentNode.x, currentNode.y + 1).isWalkable &&
                    grid.GetGridObject(currentNode.x - 1, currentNode.y).isWalkable) neighbourList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y + 1));
            }

            if (currentNode.x + 1 < grid.GetWidth())
            {
                // Right
                neighbourList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y));
                if (currentNode.y - 1 >= 0 &&
                    grid.GetGridObject(currentNode.x, currentNode.y - 1).isWalkable &&
                    grid.GetGridObject(currentNode.x + 1, currentNode.y).isWalkable) neighbourList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y - 1));
                if (currentNode.y + 1 < grid.GetHeight() &&
                    grid.GetGridObject(currentNode.x, currentNode.y + 1).isWalkable &&
                    grid.GetGridObject(currentNode.x + 1, currentNode.y).isWalkable) neighbourList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y + 1));
            }
            // Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(grid.GetGridObject(currentNode.x, currentNode.y - 1));
            // Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(grid.GetGridObject(currentNode.x, currentNode.y + 1));
            return neighbourList;
        }
        private int CalculateDistanceCost(PathNode a, PathNode b)
        {
            if (a != null && b != null)
            {
                int xDistance = Mathf.Abs(a.x - b.x);
                int yDistance = Mathf.Abs(a.y - b.y);
                int remaining = Mathf.Abs(xDistance - yDistance);
                return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST + remaining;
            }
            return 0;
        }
        private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostNode = pathNodeList[0];
            for (int i = 1; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                {
                    lowestFCostNode = pathNodeList[i];
                }
            }
            return lowestFCostNode;
        }
        private List<PathNode> CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            PathNode currentNode = endNode;
            //Debug.Log(currentNode);
            //Debug.Log(currentNode.cameFromNode);
            int x = currentNode.cameFromNode.x - currentNode.x, prevX = currentNode.cameFromNode.x - currentNode.x,
                y = currentNode.cameFromNode.y - currentNode.y, prevY = currentNode.cameFromNode.y - currentNode.y,
                counter = 0;
            path.Add(endNode);
            while (currentNode.cameFromNode != null)
            {
                if (x != prevX || y != prevY)
                {
                    path.Add(currentNode.cameFromNode);
                }
                else if (counter >= 0)
                {
                    path.Add(currentNode.cameFromNode);
                    counter = 0;
                }
                else counter++;
                prevX = x;
                prevY = y;
                currentNode = currentNode.cameFromNode;
                if (currentNode.cameFromNode == null) break;
                x = currentNode.cameFromNode.x - currentNode.x;
                y = currentNode.cameFromNode.y - currentNode.y;
                //Debug.Log(prevX + " x " + x + " --- " + prevY + " y " + y);
            }
            path.Reverse();
            return path;
        }
    }
    /// <summary>
    /// Classe importante para a classe Pathfinding, tem os valores sobre a distancia de nodes importantes, sobre nodes parentes e sobre o estado do node atual.
    /// </summary>
    public class PathNode
    {
        private Grid<PathNode> grid;
        public int x, y;

        /// <summary>
        /// G cost é a distância entre esse node e o node alvo, H cost é a distância entre esse node e o node inicial, F cost é a soma dos dois anteriores.
        /// </summary>
        public int gCost, hCost, fCost;

        public bool isWalkable;
        /// <summary>
        /// Váriavel que devolve o pathnode do qual o pathnode atual veio, se for nulo esse pathnode será o primeiro pathnode do seu caminho.
        /// </summary>
        public PathNode cameFromNode;
        /// <summary>
        /// Construtor do pathnode.
        /// </summary>
        /// <param name="grid">A grade pai.</param>
        /// <param name="x">Localização X desse pathnode.</param>
        /// <param name="y">Localização Y desse pathnode.</param>
        public PathNode(Grid<PathNode> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            isWalkable = true;
        }
        /// <summary>
        /// Calcula o custo F desse pathnode somando o valor G e H.
        /// </summary>
        public void CalculateFCost() => fCost = gCost + hCost;
        /// <summary>
        /// Define se esse pathnode é passável.
        /// </summary>
        /// <param name="value">Se sim ou não.</param>
        public void SetWalkableState(bool value) => isWalkable = value;
    }
    /// <summary>
    /// Classe Grid que facilita a criação de grades de objetos sem usar vetores bidimencionais. ([,])
    /// </summary>
    /// <typeparam name="TGridObject">Tipo customizado que será usado para criar essa grid. A grid será baseada no tipo que for colocado no construtor da grid.</typeparam>
    public class Grid<TGridObject>
    {
        /// <summary>
        /// Classe do evento.
        /// </summary>
        public event EventHandler<OnGridValueChangedEventArgs> onGridValueChanged;
        /// <summary>
        /// Classe que armazena a localização X e Y do evento de onde foi chamado.
        /// </summary>
        public class OnGridValueChangedEventArgs : EventArgs
        {
            public int x;
            public int y;
        }

        private int width;
        private int height;
        private TGridObject[,] gridArray;
        /// <summary>
        /// Classe construtora da Grid.
        /// </summary>
        /// <param name="width">Largura da grade.</param>
        /// <param name="height">Altura da grade.</param>
        /// <param name="CreateGridObject">o tipo da grade.</param>
        public Grid(int width, int height, Func<Grid<TGridObject>, int, int, TGridObject> CreateGridObject)
        {
            this.width = width;
            this.height = height;

            gridArray = new TGridObject[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gridArray[x, y] = CreateGridObject(this, x, y);
                }
            }
        }
        /// <summary>
        /// Define X e Y para as posições convertidas em int dos componentes X e Y do Vector2.
        /// </summary>
        /// <param name="position">Posição do Vector2</param>
        /// <param name="x">Coordenada X que será alterada.</param>
        /// <param name="y">Coordenada Y que será alterada.</param>
        public void GetXY(Vector2 position, out int x, out int y)
        {
            x = (int)position.x;
            y = (int)position.y;
        }
        /// <summary>
        /// Define o objeto da classe definida em "TGridObject" na posição do Vector2 transformado em int na grade.
        /// </summary>
        /// <param name="x">Coordenada X.</param>
        /// <param name="y">Coordenada Y.</param>
        /// <param name="value">O valor que será escrito nessa localização.</param>
        public void SetGridObject(int x, int y, TGridObject value)
        {
            if (CheckIfInsideGrid(x, y))
                gridArray[x, y] = value;
            if (onGridValueChanged != null)
                onGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
        }
        /// <summary>
        /// Define o objeto classe definida em "TGridObject" na grade usando o value e cria um evento de mudança nesse bloco localizado na posição do Vector2 transformado em int na grade.
        /// </summary>
        /// <param name="position">A posição em números flutuantes do Vector2.</param>
        /// <param name="value">O valor que será escrito nessa localização.</param>
        public void SetGridObject(Vector2 position, TGridObject value)
        {
            if (CheckIfInsideGrid((int)position.x, (int)position.y))
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
        /// Cria um evento sobre a mudança do objeto da grade nas coordenadas X e Y.
        /// </summary>
        /// <param name="x">Coordenada X.</param>
        /// <param name="y">Coordenada Y.</param>
        public void TriggerGridObjectChanged(int x, int y)
        {
            if (onGridValueChanged != null) onGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
        }
        /// <summary>
        /// Devolve o objeto da classe definida em "TGridObject" na posição X e Y da grade.
        /// </summary>
        /// <param name="x">Coordenada X.</param>
        /// <param name="y">Coordenada Y.</param>
        /// <returns>O objeto.</returns>
        public TGridObject GetGridObject(int x, int y)
        {
            if (CheckIfInsideGrid(x, y))
                return gridArray[x, y];
            else
                return default;
        }
        /// <summary>
        /// Devolve o objeto da classe definida em "TGridObject" na posição do Vector2 transformado em int na grade.
        /// </summary>
        /// <param name="position">A posição em números flutuantes do Vector2.</param>
        /// <returns>O objeto.</returns>
        public TGridObject GetGridObject(Vector2 position)
        {
            GetXY(position, out int x, out int y);
            return GetGridObject(x, y);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>A largura.</returns>
        public int GetWidth() => width;
        /// <summary>
        /// 
        /// </summary>
        /// <returns>A altura.</returns>
        public int GetHeight() => height;
        /// <summary>
        /// Checa se a coordenada X e Y estão dentro da grade.
        /// </summary>
        /// <param name="x">Coordenada X.</param>
        /// <param name="y">Coordenada Y.</param>
        /// <returns>Se está ou não dentro da grade.</returns>
        public bool CheckIfInsideGrid(int x, int y) => TextureFunction.IsInside2DArray(x, y, gridArray);
    }
}
