using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;
using UnityEditor.Experimental.GraphView;
using static UnityEditor.Progress;
using System.Linq;
using UnityEngine.XR;


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
                data.Dis =i+j;
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

    public async void StartAstarFind()
    {
        await StartAstar(Unit.Point.x, Unit.Point.y,Target.Point, Unit.Point);
    }

    private async UniTask StartAstar(int x, int y,int2 targetPoint,int2 startPoint)
    {
        var openList =new List<SquareData>();

        var targetQueue =new Queue<int2>();



        targetQueue.Enqueue(new int2(x, y));
       // var tempList =new List<int2>();
        var node = new Node(new int2(x, y), null);
        while (targetQueue.Count > 0)
        {
            var target = targetQueue.Dequeue();
            node = m_Node[target.x, target.y];
            await FindAstarNestDis(target.x - 1, target.y, target, openList, node);
            await FindAstarNestDis(target.x + 1, target.y, target, openList, node);
            await FindAstarNestDis(target.x, target.y - 1, target, openList, node);
            await FindAstarNestDis(target.x, target.y + 1, target, openList, node);

            var closest = openList.FindAll(p => p._gridType ==GridType.Search).OrderBy(a=>a.F).First();
            openList.Remove(closest);
            if (closest.Point.x == targetPoint.x && closest.Point.y == targetPoint.y)
            {
                break;
            }

            targetQueue.Enqueue(closest.Point);
        }

        node = m_Node[targetPoint.x, targetPoint.y];
        while (node != null)
        {
            await UniTask.WaitForSeconds(0.2f);
            Map[node.index.x, node.index.y].ChangeGrid(GridType.path);
            Debug.Log($"path :{Map[node.index.x, node.index.y].name}");
            node = node.Parent;
        }
    }

    private float GetAstarDistance(int2 form, int2 targer)
    {
        var x = Mathf.Abs(targer.x - form.x);
        var y = Mathf.Abs(targer.y - form.y);
        return  x*x+y*y ;
    }

    private async UniTask FindAstarNestDis(int x, int y,int2 lastPoint, List<SquareData> tempList,Node node)
    {
        if (x >= 0 && x < Weight && y >= 0 && y < Height)
        {
            var target = Map[x, y];
            if (target._gridType != GridType.Search)
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
