using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.QuadTree
{
    public class QTreeTest : MonoBehaviour
    {
        public QTree m_QTree;

        public QtRect m_QueryQtRect =new QtRect();

        public int InsertCount = 50;

        private List<Vector2> m_AllPoint;

        public List<Vector2> m_QueryPoint;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GenQtree()
        {
            m_QTree = new QTree(new QtRect { X =0,Y=0, Height=100,Width=100});
            m_AllPoint =new List<Vector2>();
        }

        public void TestQuery()
        {
            m_QueryPoint =new List<Vector2>();
            var count = 0;
            m_QTree.Query(m_QueryQtRect, m_QueryPoint,ref count);
            Debug.Log($"总查询 {count}");
        }

        public void TestRandomQuery()
        {
            m_QueryPoint = new List<Vector2>();
            m_QueryQtRect = new QtRect()
            {
                X = Random.Range(-50, 50),
                Y = Random.Range(-50, 50),
                Width = Random.Range(0, 100),
                Height = Random.Range(0, 100),
            };
            var count = 0;
            m_QTree.Query(m_QueryQtRect, m_QueryPoint,ref count);
            Debug.Log($"总查询 {count}");
        }
        

        public void RandomInsert()
        {
            var randomPoint = new Vector2(Random.Range(-50, 50), Random.Range(-50, 50));
            m_QTree.InSert(randomPoint);
            m_AllPoint.Add(randomPoint);
        }
        public void RandomInsertCount()
        {
            for (int i = 0; i < InsertCount; i++)
            {
                var randomPoint = new Vector2(Random.Range(-50, 50), Random.Range(-50, 50));
                m_QTree.InSert(randomPoint);
                m_AllPoint.Add(randomPoint);
            }

        }

        public void OnDrawGizmos()
        {
            if (m_QTree != null)
            {
                DrawQuadTree(m_QTree);
            }
            if (m_AllPoint != null)
            {
                foreach (var point in m_AllPoint)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(point, 1f);
                }
            }
            if (m_QueryQtRect != null)
            {
                var ract = m_QueryQtRect;
                var leftTop = new Vector2(ract.X - ract.Width / 2, ract.Y + ract.Height / 2);
                var rightTop = new Vector2(ract.X + ract.Width / 2, ract.Y + ract.Height / 2);
                var leftBack = new Vector2(ract.X - ract.Width / 2, ract.Y - ract.Height / 2);
                var rightBack = new Vector2(ract.X + ract.Width / 2, ract.Y - ract.Height / 2);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(leftTop, rightTop);
                Gizmos.DrawLine(rightTop, rightBack);
                Gizmos.DrawLine(rightBack, leftBack);
                Gizmos.DrawLine(leftBack, leftTop);
            }
            if (m_QueryPoint != null)
            {
                foreach (var point in m_QueryPoint)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(new Vector3(point.x, point.y, 1), 1f);
                }
            }

        }

        public void DrawQuadTree(QTree qTree)
        {
            if (qTree != null)
            {
                var ract = qTree.qtRect;
                var leftTop =new Vector2 (ract.X - ract.Width / 2, ract.Y + ract.Height / 2);
                var rightTop =new Vector2 (ract.X + ract.Width / 2, ract.Y + ract.Height / 2);
                var leftBack =new Vector2 (ract.X - ract.Width / 2, ract.Y - ract.Height / 2);
                var rightBack =new Vector2 (ract.X + ract.Width / 2, ract.Y - ract.Height / 2);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(leftTop, rightTop);
                Gizmos.DrawLine(rightTop, rightBack);
                Gizmos.DrawLine(rightBack, leftBack);
                Gizmos.DrawLine(leftBack, leftTop);
                if (qTree.isdivide)
                {
                    foreach (var item in qTree.ChildMap)
                    {
                        DrawQuadTree(item.Value);
                    }
                }
            }
        }
    }
}