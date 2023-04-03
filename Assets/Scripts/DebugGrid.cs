using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;


public enum CellType
{
    Empty = 0,
    RedShphere,
    BlueSphere,
    GreenSphere,
    PinkSphere,
    YellowSphere
}

public class DebugGrid : MonoBehaviour
{
    public const int SIZE = 9;
    [SerializeField]private List<GameObject> queue;
    [SerializeField] private List<Vector3> posQueue;
    [SerializeField] private List<CellType> cellTypeQueue;

    [SerializeField] private GameObject redSphere;
    [SerializeField] private GameObject blueSphere;
    [SerializeField] private GameObject greenSphere;
    [SerializeField] private GameObject pinkSphere;
    [SerializeField] private GameObject yellowSphere;
    [SerializeField] private UIController ui;
    

    private CellType[,] cells;

    public CellType[,] GetCells()
    {
        return cells;
    }
    
    
    //Take cell position in vector 3
    private Vector3 ToWorldCoords(Vector2Int coord)
    {
        return new Vector3(coord.y, 0.45f, coord.x);
    }
    //Take cell position in vector 2 
    Vector2Int ToLocalCoords(Vector3 coord)
    {
        return new Vector2Int(Mathf.RoundToInt(coord.z), Mathf.RoundToInt(coord.x));
    }
    
    //List of empty cell in grid
    public List<Vector2Int> GetEmptyCoords()
    {
        var emptyCoords = new List<Vector2Int>();
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                if (cells[j, i] == CellType.Empty)
                {
                    emptyCoords.Add(new Vector2Int(j, i));
                }
            }
        }

        return emptyCoords;
    }

    //Check if cell in bounds
    bool InBounds(int x, int y)
    {
        return (x >= 0) && (x < SIZE) && (y >= 0) && (y < SIZE);
    }
    
    
    //Get path of ball in grid
    private List<Vector2Int> GetPath(Vector2Int from, Vector2Int to, int[,] wave)
    {
        //Get empty cell list
        List<Vector2Int> path = new List<Vector2Int>();
        if (wave[from.x, from.y] == -1) return path;
        // Array of direction
        Vector2Int[] dxdy = new Vector2Int[4]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };
        Vector2Int current = from;
        path.Add(current);
        //Add path to list vector 2 int
        while (current != to)
        {
            bool stop = true;

            for (int k = 0; k < dxdy.Length; k++)
            {
                int x = current.x + dxdy[k].x;
                int y = current.y + dxdy[k].y;

                if (InBounds(x, y) && (wave[x, y] == wave[current.x, current.y] - 1))
                {
                    current = new Vector2Int(x, y);
                    path.Insert(0,current);
                    stop = false;
                }
            }

            if (stop) break;
        }
        //Check destination
        if (path.Count > 0)
        {
            if(path[0]!=to)
                path.Clear();
        }
        return path;
    }

    
    int[,] Wave(Vector2Int from, Vector2Int to)
    {
        int[,]  wave = new int[SIZE, SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                wave[j, i] = cells[j, i] == CellType.Empty ? 0 : -1;
            }
        }

        Vector2Int[] dxdy = new Vector2Int[4]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };
        int d = 1;
        wave[from.x, from.y] = d;
        while (true)
        {
            bool stop = true;
            for (int i = 0; i < SIZE; i++)
            for (int j = 0; j < SIZE; j++)
                if (wave[j, i] == d)
                {
                    for (int k = 0; k < dxdy.Length; k++)
                    {
                        int x = j + dxdy[k].x;
                        int y = i + dxdy[k].y;

                        if (InBounds(x, y) && wave[x, y] == 0)
                        {
                            wave[x, y] = d + 1;
                            stop = false;
                        }
                    }
                }
            d++;
            if (wave[to.x, to.y] != 0) break;
            if (stop) break;
        }

        return wave;
    }

    //Generate ball when start game
    public void StartGenerate(int count)
    {
        var emptyCoords = GetEmptyCoords();
        if (emptyCoords.Count == 0)
            return;
        //init balls
        count = Mathf.Min(count, emptyCoords.Count);
        var cellTypes = Enum.GetValues(typeof(CellType)).Cast<CellType>();
        int max = (int) cellTypes.Max() + 1;
        int min = (int) cellTypes.Min() + 1;

        for (int i = 0; i < count; i++)
        {
            CellType cellType = (CellType) Random.Range(min, max);
            int index = Random.Range(0, emptyCoords.Count);
            cells[emptyCoords[index].x, emptyCoords[index].y] = cellType;

            GameObject prefabs=null;
            switch (cellType)
            {
                case CellType.RedShphere:
                    prefabs = redSphere;
                    break;
                case CellType.BlueSphere:
                    prefabs = blueSphere;
                    break;
                case CellType.GreenSphere:
                    prefabs = greenSphere;
                    break;
                case CellType.PinkSphere:
                    prefabs = pinkSphere;
                    break;
                case CellType.YellowSphere:
                    prefabs = yellowSphere;
                    break;
            }
            //Set position on grid
            Vector3 pos = ToWorldCoords(emptyCoords[index]);
            Instantiate(prefabs, pos, Quaternion.identity);
            emptyCoords.RemoveAt(index);
        }
        //Init Queue
        Generate(count);
    }
    
    public int Generate(int count)
    {
        var emptyCoords = GetEmptyCoords();
        if (emptyCoords.Count == 0)
            return 0;
        count = Mathf.Min(count, emptyCoords.Count);
        var cellTypes = Enum.GetValues(typeof(CellType)).Cast<CellType>();
        int max = (int) cellTypes.Max() + 1;
        int min = (int) cellTypes.Min() + 1;

        for (int i = 0; i < count; i++)
        {
            CellType cellType = (CellType) Random.Range(min, max);
            int index = Random.Range(0, emptyCoords.Count); 
            //cells[emptyCoords[index].x, emptyCoords[index].y] = cellType;
            
            GameObject prefabs=null;
            switch (cellType)
            {
                case CellType.RedShphere:
                    prefabs = redSphere;
                    break;
                case CellType.BlueSphere:
                    prefabs = blueSphere;
                    break;
                case CellType.GreenSphere:
                    prefabs = greenSphere;
                    break;
                case CellType.PinkSphere:
                    prefabs = pinkSphere;
                    break;
                case CellType.YellowSphere:
                    prefabs = yellowSphere;
                    break;
            }
            //Init Queue
            Vector3 pos = ToWorldCoords(emptyCoords[index]);
            if (!posQueue.Contains(pos))
            {
                posQueue.Add(pos);
                GameObject predictClone = Instantiate(prefabs, pos, Quaternion.identity);
                Destroy(predictClone.GetComponent<Sphere>());
                cellTypeQueue.Add(cellType);
                queue.Add(predictClone);
                predictClone.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                cells[emptyCoords[index].x, emptyCoords[index].y] = CellType.Empty;
            }
        }

        return count;
    }
    
    //Generate ball include queue
    public void GenerateBall(int count)
    {
        var emptyCoords = GetEmptyCoords();
        for (int i = 0; i < queue.Count; i++)
        {
            if (queue[i] != null)
            {
                GameObject ballClone = Instantiate(queue[i], posQueue[i], Quaternion.identity);
                ballClone.AddComponent<Sphere>();
                ballClone.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                Vector2Int pos = ToLocalCoords(posQueue[i]);
                cells[pos.x, pos.y] = cellTypeQueue[i];
                Destroy(queue[i].gameObject);
                emptyCoords.Remove(pos);
                ui.Points.Points += DestroyLines(posQueue[i]);
            }
        }
        //Reset queue
        queue.Clear();
        posQueue.Clear();
        cellTypeQueue.Clear();
        Generate(count);
    }
    
    //Change state of cell
    public void Move(Vector3 from, Vector3 to)
    {
        Vector2Int _from = ToLocalCoords(from);
        Vector2Int _to = ToLocalCoords(to);
        cells[_to.x, _to.y] = cells[_from.x, _from.y];
        cells[_from.x, _from.y] = CellType.Empty; 
    
    }
    
    //Check line if have same balls type have line
    private HashSet<Vector2Int> CheckLine(Vector2Int pos, int dx, int dy)
    {
        HashSet<Vector2Int> line = new HashSet<Vector2Int>();
        for (int i = 0; i < SIZE*SIZE; i++)
        {
            int x = pos.x + i * dx;
            int y = pos.y + i * dy;
            if (InBounds(x, y))
            {
                if (cells[x, y] == cells[pos.x, pos.y])
                {
                    line.Add(new Vector2Int(x, y));
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
        return line;
    }
    
    
    //Take point if have line and destroy it
    public int DestroyLines(Vector3 pos)
    {
        //Create List line in current cell
        Vector2Int _pos = ToLocalCoords(pos);
        HashSet<Vector2Int> destroyed = new HashSet<Vector2Int>();
        HashSet<Vector2Int> line = new HashSet<Vector2Int>();
        //Init list of direction
        List<Tuple<Vector2Int, Vector2Int>> dxdy = new List<Tuple<Vector2Int, Vector2Int>>()
        {
            new Tuple<Vector2Int, Vector2Int>(new Vector2Int(1, 0), new Vector2Int(-1, 0)),
            new Tuple<Vector2Int, Vector2Int>(new Vector2Int(0, 1), new Vector2Int(0, -1)),
            new Tuple<Vector2Int, Vector2Int>(new Vector2Int(1, 1), new Vector2Int(-1, -1)),
            new Tuple<Vector2Int, Vector2Int>(new Vector2Int(1, -1), new Vector2Int(-1, 1)),
        };
        //Check line and combine it
        for (int i = 0; i < dxdy.Count; i++)
        {
            line.UnionWith(CheckLine(_pos,dxdy[i].Item1.x,dxdy[i].Item1.y));
            line.UnionWith(CheckLine(_pos,dxdy[i].Item2.x,dxdy[i].Item2.y));
            
            if(line.Count>=5)
                destroyed.UnionWith(line);
            line.Clear();
        }
        //Destroy line that have more than 5 spheres same type
        List<Sphere> spheres = FindObjectsOfType<Sphere>().ToList();
        foreach (var sphere in spheres)
        {
            Vector2Int spherePos = ToLocalCoords(sphere.transform.position);
            if (destroyed.Contains(spherePos))
            {
                sphere.DestroySphere();
                cells[spherePos.x, spherePos.y] = CellType.Empty;
            }
        }
        return destroyed.Count;
    }
    
    //return list of path ball go
    public List<Vector3> GetPath(Vector3 from, Vector3 to)
    {
        Vector2Int _from = ToLocalCoords(from);
        Vector2Int _to = ToLocalCoords(to);
        var wave = Wave(_from, _to);
        var path = GetPath(_to, _from, wave);
        var result = path.Select(v => ToWorldCoords(v)).ToList();
        return result;
    }

    //reset grid
    public void Clear()
    {
        cells = new CellType[SIZE, SIZE];
        queue.Clear();
        posQueue.Clear();
        cellTypeQueue.Clear();
        var list = FindObjectsOfType<SphereCollider>().ToList();
        foreach (var item in list)
        {
            Destroy(item.gameObject);
        }
    }

    //load grid 
    public void SetState(GameState gameState)
    {
        Clear();
        cells = gameState.Cells;
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                CellType cellType = cells[j, i];
                if (cellType != CellType.Empty)
                {
                    GameObject prefabs=null;
                    switch (cellType)
                    {
                        case CellType.RedShphere:
                            prefabs = redSphere;
                            break;
                        case CellType.BlueSphere:
                            prefabs = blueSphere;
                            break;
                        case CellType.GreenSphere:
                            prefabs = greenSphere;
                            break;
                        case CellType.PinkSphere:
                            prefabs = pinkSphere;
                            break;
                        case CellType.YellowSphere:
                            prefabs = yellowSphere;
                            break;
                    }

                    Vector3 pos = ToWorldCoords(new Vector2Int(j, i));
                    Instantiate(prefabs, pos, Quaternion.identity);
                }
            }
        }
    }

    
}

[System.Serializable]
public class GameState
{
    public int Points;
    public CellType[,] Cells;

    public GameState(CellType[,] cells, int points)
    {
        Points = points;
        Cells = cells;
    }
    
    public GameState()
    {
        Points = 0;
        Cells = new CellType[DebugGrid.SIZE,DebugGrid.SIZE];
    }
}