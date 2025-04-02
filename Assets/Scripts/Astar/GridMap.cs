using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using System;
using CrashKonijn.Goap.Demos.Complex.Goap;
using UnityEditor.Experimental.GraphView;
using static GridMap;


public class Node
{
    public int2 index;
    public Node Parent;
    public Node(int2 index,Node parent)
    {
        this.index = index;
        this.Parent = parent;
    }
}

public class GridMap : MonoBehaviour
{

    public enum AstarDir
    {
        l,
        lf,
        f,
        rf,
        r,
        rd,
        d,
        ld,
    }

    public enum JPSPositiveDir
    {
        l,
        f,
        r,
        d,
    }

    public enum JPSDir
    {
        lf,
        rf,
        rd,
        ld,
    }

    public int Weight = 10;
    public int Height = 10;
    public GameObject Tile;

    public SquareData[,] Map;

    public SquareData Unit;
    public SquareData Target;

    public List<SquareData> Obs;

    private Camera _camera;

    private Node[,] m_Node;

    private int[,] m_Dis;

    private List<SquareData> m_SquareDataList = new List<SquareData>();

    private Dictionary<int, SquareData> m_SquareFindDict = new Dictionary<int, SquareData>();

    private bool Find = false;

    public float WaitTime = 0.05f;


      private void Start()
    {
        _camera = Camera.main;

    }
    public void GenMap()
    {
        Map = new SquareData[Weight, Height];
        m_Node = new Node[Weight, Height];
        m_Dis =new int[Weight, Height];
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Weight; j++)
            {
                var pos = new Vector3(j, i, 0);
                var entity = Instantiate(Tile, pos, Quaternion.identity, gameObject.transform);
                entity.name = $"({pos.x},{pos.y})";
                var data = entity.GetComponent<SquareData>();
                data.Index = i * Weight + j;
                data.Point = new int2(j, i);
               // data.Dis =i+j;
                data.Dis =1;
                data.F =float.MaxValue;
               // data.AstarDistance =float.MaxValue;
                data.SetText(pos);
                Map[j, i] = data;
                m_SquareDataList.Add(data);
                m_Dis[j, i] = int.MaxValue;
            }
        }
    }

    private void Update()
    {

        MouseInput();

    }

    private async UniTask StartFlow(int x, int y, int2 targetPoint, int2 startPoint)
    {
        var openList  =new List<SquareData>();
        var closeList =new List<SquareData>();
        openList.Add(Unit);
        while (openList.Count > 0)
        {
            var target = openList[openList.Count - 1];
            openList.RemoveAt(openList.Count - 1);
          await  FindNestAsync(target.Point.x - 1, target.Point.y, openList);
          await  FindNestAsync(target.Point.x , target.Point.y-1, openList);
          await  FindNestAsync(target.Point.x + 1, target.Point.y, openList);
          await  FindNestAsync(target.Point.x , target.Point.y+1, openList);
        }
    }



    private async UniTask FindNestAsync(int x, int y,List<SquareData> openList)
    {
        if (x >= 0 && x < Weight && y >= 0 && y < Height)
        {
           var target =Map[x, y];
            if (target._gridType != GridType.Search && target._gridType != GridType.Obs)
            {
                openList.Add(target);
                await UniTask.WaitForSeconds(0.02f);
            }
        }
    }

    public async void StartAstarFind()
    {
        await StartAstar(Unit.Point.x, Unit.Point.y,Target.Point, Unit.Point);
    }

    public async void StartJPSFind()
    {
        await StartJPS(Unit.Point.x, Unit.Point.y, Target.Point, Unit.Point);
    }

    private async UniTask StartJPS(int x, int y, int2 targetPoint, int2 startPoint)
    {
        var openList = new SortedSet<SquareData>(new NodeComparer());
        var closeList = new HashSet<SquareData>();
        var startNode = Map[x, y];
        startNode.G = 0;
        startNode.H = GetAstarDistance(startNode.Point, targetPoint);
        startNode.F = startNode.G + startNode.H;
        startNode.Parent = null;
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            var currentNode = openList.Min;
            openList.Remove(currentNode);
            if (currentNode.Point.Equals(targetPoint))
            {
                await ReconstructPath(currentNode);
                return;
            }
            closeList.Add(currentNode);
            Map[currentNode.Point.x, currentNode.Point.y].ChangeGrid(GridType.Jump);
            foreach (AstarDir dir in Enum.GetValues(typeof(AstarDir)))
            {
                var queue = new Queue<SquareData>();
               await Jump(currentNode, dir, targetPoint, queue);

                while (queue.Count>0)
                {
                    var jumpPoint= queue.Dequeue();
                    if (jumpPoint != null && !closeList.Contains(jumpPoint))
                    {
                        var tentativeG = currentNode.G + GetCost(dir) * jumpPoint.Dis;
                        if (!openList.Contains(jumpPoint) || tentativeG < jumpPoint.G)
                        {
                            if (openList.Contains(jumpPoint))
                                openList.Remove(jumpPoint);

                            jumpPoint.Parent = currentNode;
                            jumpPoint.G = tentativeG;
                            jumpPoint.H = GetAstarDistance(jumpPoint.Point, targetPoint);
                            jumpPoint.F = jumpPoint.G + jumpPoint.H;

                            openList.Add(jumpPoint);
                        }
                    }
                }

            }
            await UniTask.WaitForSeconds(0.02f);
        }
    }
    private async UniTask Jump(SquareData currentNode, AstarDir dir, int2 targetPoint, Queue<SquareData> jumplist)
    {
        int x = currentNode.Point.x;
        int y = currentNode.Point.y;
        int2 d = GetDirection(dir);

        while (true)
        {
            x += d.x;
            y += d.y;

            if (!IsInBounds(x, y) || Map[x, y]._gridType == GridType.Obs)
            {
                return;
            }

            if (Map[x, y].Point.Equals(targetPoint))
            {
                jumplist.Enqueue(Map[x, y]);
                return;
            }
            await UniTask.WaitForSeconds(WaitTime);
            Map[x,y].ChangeGrid(GridType.Search);
            if (HasForcedNeighbor(x, y, d))
            {
                jumplist.Enqueue(Map[x, y]);
            }

            if (d.x != 0 && d.y != 0)
            {
                if (IsInBounds(x - d.x, y) && Map[x - d.x, y]._gridType != GridType.Obs && IsInBounds(x - d.x, y + d.y) && Map[x - d.x, y + d.y]._gridType != GridType.Obs)
                {
                    jumplist.Enqueue(Map[x, y]);
                }
                if (IsInBounds(x, y - d.y) && Map[x, y - d.y]._gridType != GridType.Obs && IsInBounds(x + d.x, y - d.y) && Map[x + d.x, y - d.y]._gridType != GridType.Obs)
                {
                    jumplist.Enqueue(Map[x, y]);
                }
            }
            else
            {
                if (d.x != 0)
                {
                    if (IsInBounds(x, y + 1) && Map[x, y + 1]._gridType == GridType.Obs && IsInBounds(x + d.x, y + 1) && Map[x + d.x, y + 1]._gridType != GridType.Obs)
                    {
                        jumplist.Enqueue(Map[x, y]);
                    }
                    if (IsInBounds(x, y - 1) && Map[x, y - 1]._gridType == GridType.Obs && IsInBounds(x + d.x, y - 1) && Map[x + d.x, y - 1]._gridType != GridType.Obs)
                    {
                        jumplist.Enqueue(Map[x, y]);
                    }
                }
                else if (d.y != 0)
                {
                    if (IsInBounds(x + 1, y) && Map[x + 1, y]._gridType == GridType.Obs && IsInBounds(x + 1, y + d.y) && Map[x + 1, y + d.y]._gridType != GridType.Obs)
                    {
                        jumplist.Enqueue(Map[x, y]);
                    }
                    if (IsInBounds(x - 1, y) && Map[x - 1, y]._gridType == GridType.Obs && IsInBounds(x - 1, y + d.y) && Map[x - 1, y + d.y]._gridType != GridType.Obs)
                    {
                        jumplist.Enqueue(Map[x, y]);
                    }
                }
            }
        }
    }

    private SquareData FindJumpPoint(SquareData currentNode, AstarDir dir, int2 targetPoint)
    {
        int x = currentNode.Point.x;
        int y = currentNode.Point.y;
        int2 d = GetDirection(dir);


        while (true)
        {
            x += d.x;
            y += d.y;

            if (x < 0 || x >= Weight || y < 0 || y >= Height || Map[x, y]._gridType == GridType.Obs)
            {
                return null;
            }

            if (Map[x, y].Point.Equals(targetPoint))
            {
                return Map[x, y];
            }

            if (HasForcedNeighbor(x, y, d))
            {
                return Map[x, y];
            }
        }


    }
    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Weight && y >= 0 && y < Height;
    }
    private bool HasForcedNeighbor(int x, int y, int2 d)
    {
        if (d.x != 0 && d.y != 0)
        {
            return (IsInBounds(x - d.x, y) && Map[x - d.x, y]._gridType == GridType.Obs && IsInBounds(x - d.x, y + d.y) && Map[x - d.x, y + d.y]._gridType != GridType.Obs) ||
                   (IsInBounds(x, y - d.y) && Map[x, y - d.y]._gridType == GridType.Obs && IsInBounds(x + d.x, y - d.y) && Map[x + d.x, y - d.y]._gridType != GridType.Obs);
        }
        else if (d.x != 0)
        {
            return (IsInBounds(x, y + 1) && Map[x, y + 1]._gridType == GridType.Obs && IsInBounds(x + d.x, y + 1) && Map[x + d.x, y + 1]._gridType != GridType.Obs) ||
                   (IsInBounds(x, y - 1) && Map[x, y - 1]._gridType == GridType.Obs && IsInBounds(x + d.x, y - 1) && Map[x + d.x, y - 1]._gridType != GridType.Obs);
        }
        else if (d.y != 0)
        {
            return (IsInBounds(x + 1, y) && Map[x + 1, y]._gridType == GridType.Obs && IsInBounds(x + 1, y + d.y) && Map[x + 1, y + d.y]._gridType != GridType.Obs) ||
                   (IsInBounds(x - 1, y) && Map[x - 1, y]._gridType == GridType.Obs && IsInBounds(x - 1, y + d.y) && Map[x - 1, y + d.y]._gridType != GridType.Obs);
        }
        return false;
    }


    private int2 GetDirection(AstarDir dir)
    {
        return dir switch
        {
            AstarDir.l => new int2(-1, 0),
            AstarDir.lf => new int2(-1, 1),
            AstarDir.f => new int2(0, 1),
            AstarDir.rf => new int2(1, 1),
            AstarDir.r => new int2(1, 0),
            AstarDir.rd => new int2(1, -1),
            AstarDir.d => new int2(0, -1),
            AstarDir.ld => new int2(-1, -1),
            _ => new int2(0, 0),
        };
    }



    private async UniTask StartAstar(int x, int y,int2 targetPoint,int2 startPoint)
    {
        var openList =new SortedSet<SquareData>(new NodeComparer());

        var closeList = new HashSet<SquareData>();

        var startNode = Map[x, y];
        startNode.G = 0;
        startNode.H = GetAstarDistance(startNode.Point, targetPoint);
        startNode.F = startNode.G + startNode.H;
        startNode.Parent = null;
        openList.Add(startNode);
        while (openList.Count > 0)
        {
            var currentNode = openList.Min;
            openList.Remove(currentNode);
            if (currentNode.Point.Equals(targetPoint))
            {
                await ReconstructPath(currentNode);
                return;
            }
            closeList.Add(currentNode);

            foreach (AstarDir dir in Enum.GetValues(typeof(AstarDir)))
            {
                var neighbor = GetNeighbor(currentNode, dir);
                if (neighbor == null || closeList.Contains(neighbor) || neighbor._gridType == GridType.Obs)
                {
                    continue;
                }
                var tentativeG = currentNode.G + GetCost(dir)*neighbor.Dis;
                var tentativeH = GetAstarDistanceDir(neighbor.Point, targetPoint);
                var tentativeF = tentativeG + tentativeH;
                if (!openList.Contains(neighbor) || tentativeG < neighbor.G)
                {
                    if (openList.Contains(neighbor))
                        openList.Remove(neighbor);

                    neighbor.Parent = currentNode;
                    neighbor.G = tentativeG;
                    neighbor.H = tentativeH;
                    neighbor.F = tentativeF;

                    openList.Add(neighbor);
                    Map[neighbor.Point.x, neighbor.Point.y].ChangeGrid(GridType.Search);
                }


            }
            await UniTask.WaitForSeconds(0.02f);
        }
    }

    public class NodeComparer : IComparer<SquareData>
    {
        public int Compare(SquareData a, SquareData b)
        {
            int fCompare = a.F.CompareTo(b.F);
            if (fCompare != 0)
                return fCompare;
            return a.H.CompareTo(b.H);
        }
    }

    private float GetCost(AstarDir dir)
    {
        return dir switch
        {
            AstarDir.l => 1,
            AstarDir.lf => 1.4f,
            AstarDir.f => 1,
            AstarDir.rf => 1.4f,
            AstarDir.r => 1,
            AstarDir.rd => 1.4f,
            AstarDir.d => 1,
            AstarDir.ld => 1.4f,
            _ => 1f,
        };
    }

    private SquareData GetNeighbor(SquareData node, AstarDir dir)
    {
        int x = node.Point.x;
        int y = node.Point.y;

        switch (dir)
        {
            case AstarDir.l:
                x--;
                break;
            case AstarDir.lf:
                x--;
                y++;
                break;
            case AstarDir.f:
                y++;
                break;
            case AstarDir.rf:
                x++;
                y++;
                break;
            case AstarDir.r:
                x++;
                break;
            case AstarDir.rd:
                x++;
                y--;
                break;
            case AstarDir.d:
                y--;
                break;
            case AstarDir.ld:
                x--;
                y--;
                break;
        }

        if (x >= 0 && x < Weight && y >= 0 && y < Height)
        {
            return Map[x, y];
        }

        return null;
    }

    private async UniTask ReconstructPath(SquareData node)
    {
        while (node != null)
        {
            await UniTask.WaitForSeconds(0.2f);
            node.ChangeGrid(GridType.path);
            Debug.Log($"path :{node.name}");
            node = node.Parent;
        }
    }

    private async UniTask FindAstarNestbyDir(AstarDir dir, int x, int y, int2 lastPoint, List<SquareData> tempList, Node node)
    {
        switch (dir)
        {
            case AstarDir.l:
                x--;
                break;
            case AstarDir.lf:
                x--;
                y++;
                break;
            case AstarDir.f:
                y++;
                break;
            case AstarDir.rf:
                x++;
                y++;
                break;
            case AstarDir.r:
                x++;
                break;
            case AstarDir.rd:
                x++;
                y--;
                break;
            case AstarDir.d:
                y--;
                break;
            case AstarDir.ld:
                x--;
                y--;
                break;
            default:
                break;
        }
        if (x >= 0 && x < Weight && y >= 0 && y < Height)
        {
            var target = Map[x, y];
            if (target._gridType != GridType.Search)
            {
                target.G += Map[lastPoint.x, lastPoint.y].G + target.Dis;
                target.H = GetAstarDistanceDir(new int2(x, y), Target.Point);
                target.F = target.G + target.H;
                tempList.Add(target);
                Map[x, y].ChangeGrid(GridType.Search);
                m_Node[x, y] = new Node(new int2(x, y), node);
                await UniTask.WaitForSeconds(0.02f);
            }
            else
            {
                if (target.F < Map[lastPoint.x, lastPoint.y].F)
                {
                    if (!tempList.Contains(target))
                    {
                        tempList.Add(target);
                    }
                }
            }
        }

    }


    private float GetAstarDistanceDir(int2 form, int2 targer)
    {
        var x  =targer.x - form.x;
        var y = targer.y - form.y;
        return (float)Math.Sqrt(x * x + y * y);
    }

    private float GetAstarDistance(int2 form, int2 targer)
    {
        var x = Mathf.Abs(targer.x - form.x);
        var y = Mathf.Abs(targer.y - form.y);
        return x + y;
    }

    private async UniTask FindAstarNestDis(int x, int y,int2 lastPoint, List<SquareData> tempList,Node node)
    {
        if (x >= 0 && x < Weight && y >= 0 && y < Height)
        {
            var target = Map[x, y];
            if (target._gridType != GridType.Search && target._gridType != GridType.Obs)
            {
                target.G += Map[lastPoint.x, lastPoint.y].G + target.Dis;
                target.H = GetAstarDistance(new int2(x, y), Target.Point);
                target.F = target.G + target.H;
                tempList.Add(target);
                Map[x, y].ChangeGrid(GridType.Search);
                m_Node[x, y] = new Node(new int2(x, y), node);
                await UniTask.WaitForSeconds(0.02f);
            }
            else
            {
                if (target.F < Map[lastPoint.x, lastPoint.y].F)
                {
                    if (!tempList.Contains(target))
                    {
                        tempList.Add(target);
                    }
                }
            }
        }
    }


    public async void StartDijkkatraFind()
    {
        await StartDijkkatra(Unit.Point.x, Unit.Point.y);
    }

    private async UniTask StartDijkkatra(int x, int y)
    {
        var dijqueue = new Queue<int2>();

        dijqueue.Enqueue(new int2(x,y));
        m_Node[x, y] = new Node(new int2(x, y), null);
        var findindex = new int2(0,0);
        while (dijqueue.Count > 0)
        {
            var target = dijqueue.Dequeue();

             await FindNestDis(target.x-1, target.y, new int2(target.x, target.y),  m_Node[target.x, target.y]);
            await FindNestDis(target.x+1, target.y, new int2(target.x, target.y), m_Node[target.x, target.y]);
            await FindNestDis(target.x, target.y-1, new int2(target.x, target.y),  m_Node[target.x, target.y]);
            await FindNestDis(target.x, target.y+1,  new int2(target.x, target.y),  m_Node[target.x, target.y]);

            if (Map[target.x, target.y] == Target)
            {
                Debug.Log($"find :{Map[target.x, target.y].name}");
                Find = true;
                findindex = new int2(target.x, target.y);
                break;
            }
            var tempDis = int.MaxValue;
            bool ishasMin = false;
            int2 minIndex =new int2();

            for (int i = 0; i < Weight; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    var data = Map[i,j];
                    var disdaata = m_Dis[i, j];
                    if (data._gridType != GridType.Search)
                    {
                        if (tempDis > disdaata)
                        {
                            tempDis = disdaata;
                            ishasMin =true;
                            minIndex =new int2 (i,j);
                        }
                    }
                }

            }
            if (ishasMin)
            {
                Map[minIndex.x, minIndex.y].ChangeGrid(GridType.Search);
                Debug.Log($"search :{Map[minIndex.x, minIndex.y].name}");
                dijqueue.Enqueue(minIndex);

            }

        }

        var curNode = m_Node[findindex.x, findindex.y];
        while (curNode != null)
        {
            await UniTask.WaitForSeconds(0.2f);
            Map[curNode.index.x, curNode.index.y].ChangeGrid(GridType.path);
            Debug.Log($"path :{Map[curNode.index.x, curNode.index.y].name}");
            curNode = curNode.Parent;
        }
    }

    private async  UniTask FindNestDis(int x, int y, int2 currIndex,Node node)
    {
        if (x >= 0 && x < Weight && y >= 0 && y < Height)
        {
            var target =Map[x, y];
            if (target._gridType != GridType.Search)
            {
                m_Dis[x, y] = m_Dis[currIndex.x, currIndex.y]+ target.Dis;
                m_Node[x, y] = new Node(new int2(x, y), node);
            }
            else if(m_Dis[x, y] > m_Dis[currIndex.x, currIndex.y] + target.Dis)
            {

                m_Dis[x, y] = target.Dis+ m_Dis[currIndex.x, currIndex.y];
                m_Node[x, y].Parent = node;
            }
            await UniTask.WaitForSeconds(0.02f);
        }
    }

    public async void StartBFSFind()
    {
        await StartBFS(Unit.Point.x, Unit.Point.y);

    }

    private async UniTask StartBFS(int x, int y)
    {
         var queue = new Queue<SquareData>();
        queue.Enqueue(Map[x,y]);

        while (queue.Count > 0)
        {
            var currSq = queue.Dequeue();
            var currindexX =currSq.Point.x;
            var currindexY =currSq.Point.y;

            await AddBfsPoint(currindexX - 1, currindexY, queue);
            await AddBfsPoint(currindexX + 1, currindexY, queue);
            await AddBfsPoint(currindexX , currindexY-1, queue);
            await AddBfsPoint(currindexX , currindexY+1, queue);
        }

        return;
    }

    private async UniTask AddBfsPoint(int x ,int y ,Queue<SquareData> squares)
    {
        if (x >= 0 && x < Weight && y >= 0 && y < Height)
        {
            if (Find)
            {
                return;
            }
            var target = Map[x, y];
            if (target._gridType == GridType.path)
            {
                return;
            }
            if (target._gridType == GridType.Obs)
            {
                return;
            }

            target.ChangeGrid(GridType.path);
            Debug.Log($"path :{target.name}");
            if (target == Target)
            {
                Debug.Log($"Find {target.name}");
                Find = true;
                return;
            }
            squares.Enqueue(target);
            await UniTask.WaitForSeconds(0.2f);
        }
    }

    public async void StartDFSFind()
    {
        await StartDFS(Unit.Point.x, Unit.Point.y);

    }

    private async UniTask StartDFS(int x, int y)
    {

        if (x >= 0 && x < Weight && y >= 0 && y < Height)
        {
            if (Find)
            {
                return;
            }
            var target = Map[x, y];
            if (target._gridType == GridType.path)
            {
                return;
            }
            if (target._gridType == GridType.Obs)
            {
                return;
            }
            target.ChangeGrid(GridType.path);
            Debug.Log($"path :{target.name}");
            if (target == Target)
            {
                Debug.Log($"Find {target.name}");
                Find = true;
                return;
            }
            await UniTask.WaitForSeconds(0.2f);
            await StartDFS(x - 1, y);
            await StartDFS(x + 1, y);
            await StartDFS(x, y - 1);
            await StartDFS(x, y + 1);
        }

        return;
    }



    public void CleanAll()
    {
        foreach (var data in m_SquareDataList)
        {
            data.ChangeGrid(GridType.None);
        }
        Find =false;
    }

    private void MouseInput()
    {
        Vector2 mousePosition = Input.mousePosition;

        Vector3 worldPosition = _camera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0;
        var dir = (worldPosition - _camera.transform.position).normalized;


        Debug.DrawRay(_camera.transform.position, dir * 100, Color.red);
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_camera.transform.position, dir, out var hit, 100f))
            {
                if (hit.collider != null)
                {
                    var target = hit.collider.GetComponent<SquareData>();
                    if (Target != null)
                    {
                        Target.ChangeGrid(GridType.None);
                    }
                    Target = target;
                    target.ChangeGrid(GridType.Target);
                    Debug.Log($"{hit.collider.name}   {hit.collider.GetComponent<SquareData>().m_Text.text} ");
                }
            }

        }

        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(_camera.transform.position, dir, out var hit, 100f))
            {
                if (hit.collider != null)
                {
                    var target = hit.collider.GetComponent<SquareData>();
                    if (Unit != null)
                    {
                        Unit.ChangeGrid(GridType.None);
                    }
                    Unit = target;
                    target.ChangeGrid(GridType.Unit);
                    Debug.Log($"{hit.collider.name}   {hit.collider.GetComponent<SquareData>().m_Text.text} ");
                }
            }

        }

        if (Input.GetMouseButtonDown(2))
        {
            if (Physics.Raycast(_camera.transform.position, dir, out var hit, 100f))
            {
                if (hit.collider != null)
                {

                    var target = hit.collider.GetComponent<SquareData>();
                    target.ChangeGrid(GridType.Obs);
                    target.Dis =int.MaxValue;
                    Obs.Add(target);
                    Debug.Log($"{hit.collider.name}   {hit.collider.GetComponent<SquareData>().m_Text.text} ");
                }
            }

        }
    }
}
