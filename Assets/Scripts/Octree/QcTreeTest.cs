using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.QctTree
{
    public class QcTreeTest : MonoBehaviour
    {
        public QTree m_QTree;

        public QtRect m_QueryQtRect =new QtRect();

        public int InsertCount = 50;

        private List<Vector3> m_AllPoint;

        public List<Vector3> m_QueryPoint;
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
            m_QTree = new QTree(new QtRect { X =0,Y=0,Z =0, Height=100,Width=100,Lenght=100});
            m_AllPoint =new List<Vector3>();
        }

        public void TestQuery()
        {
            m_QueryPoint =new List<Vector3>();
            var count = 0;
            m_QTree.Query(m_QueryQtRect, m_QueryPoint,ref count);
            Debug.Log($"总查询 {count}");
        }

        public void TestRandomQuery()
        {
            m_QueryPoint = new List<Vector3>();
            m_QueryQtRect = new QtRect()
            {
                X = Random.Range(-50, 50),
                Y = Random.Range(-50, 50),
                Z = Random.Range(-50, 50),
                Width = Random.Range(0, 100),
                Height = Random.Range(0, 100),
                Lenght = Random.Range(0, 100)
            };
            var count = 0;
            m_QTree.Query(m_QueryQtRect, m_QueryPoint,ref count);
            Debug.Log($"总查询 {count}");
        }
        

        public void RandomInsert()
        {
            var randomPoint = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50));
            m_QTree.InSert(randomPoint);
            m_AllPoint.Add(randomPoint);
        }
        public void RandomInsertCount()
        {
            for (int i = 0; i < InsertCount; i++)
            {
                var randomPoint = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50));
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
                var leftTopup = new Vector3(ract.X - ract.Width / 2, ract.Y + ract.Height / 2, ract.Z + ract.Lenght / 2);
                var rightTopup = new Vector3(ract.X + ract.Width / 2, ract.Y + ract.Height / 2, ract.Z + ract.Lenght / 2);
                var leftBackup = new Vector3(ract.X - ract.Width / 2, ract.Y - ract.Height / 2, ract.Z + ract.Lenght / 2);
                var rightBackup = new Vector3(ract.X + ract.Width / 2, ract.Y - ract.Height / 2, ract.Z + ract.Lenght / 2);

                var leftTopdown = new Vector3(ract.X - ract.Width / 2, ract.Y + ract.Height / 2, ract.Z -ract.Lenght / 2);
                var rightTopdown = new Vector3(ract.X + ract.Width / 2, ract.Y + ract.Height / 2, ract.Z - ract.Lenght / 2);
                var leftBackdown = new Vector3(ract.X - ract.Width / 2, ract.Y - ract.Height / 2, ract.Z -ract.Lenght / 2);
                var rightBackdown = new Vector3(ract.X + ract.Width / 2, ract.Y - ract.Height / 2, ract.Z - ract.Lenght / 2);


                Gizmos.color = Color.red;
                Gizmos.DrawLine(leftTopup, leftBackup);
                Gizmos.DrawLine(leftBackup, rightBackup);
                Gizmos.DrawLine(rightBackup, rightTopup);
                Gizmos.DrawLine(rightTopup, leftTopup);

                Gizmos.DrawLine(leftTopdown, leftBackdown);
                Gizmos.DrawLine(leftBackdown, rightBackdown);
                Gizmos.DrawLine(rightBackdown, rightTopdown);
                Gizmos.DrawLine(rightTopdown, leftTopdown);

                Gizmos.DrawLine(leftTopup, leftTopdown);
                Gizmos.DrawLine(rightTopup, rightTopdown);
                Gizmos.DrawLine(leftBackup, leftBackdown);
                Gizmos.DrawLine(rightBackup, rightBackdown);


            }
            if (m_QueryPoint != null)
            {
                foreach (var point in m_QueryPoint)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(new Vector3(point.x, point.y, point.z), 1f);
                }
            }

        }

        public void DrawQuadTree(QTree qTree)
        {
            if (qTree != null)
            {
                var ract = qTree.qtRect;
                var leftTopup = new Vector3(ract.X - ract.Width / 2, ract.Y + ract.Height / 2, ract.Z + ract.Lenght / 2);
                var leftBackup = new Vector3(ract.X - ract.Width / 2, ract.Y - ract.Height / 2, ract.Z + ract.Lenght / 2);
                var rightBackup = new Vector3(ract.X + ract.Width / 2, ract.Y - ract.Height / 2, ract.Z + ract.Lenght / 2);
                var rightTopup = new Vector3(ract.X + ract.Width / 2, ract.Y + ract.Height / 2, ract.Z + ract.Lenght / 2);

                var leftTopdown = new Vector3(ract.X - ract.Width / 2, ract.Y + ract.Height / 2, ract.Z - ract.Lenght / 2);
                var leftBackdown = new Vector3(ract.X - ract.Width / 2, ract.Y - ract.Height / 2, ract.Z - ract.Lenght / 2);
                var rightBackdown = new Vector3(ract.X + ract.Width / 2, ract.Y - ract.Height / 2, ract.Z - ract.Lenght / 2);
                var rightTopdown = new Vector3(ract.X + ract.Width / 2, ract.Y + ract.Height / 2, ract.Z - ract.Lenght / 2);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(leftTopup, leftBackup);
                Gizmos.DrawLine(leftBackup, rightBackup);
                Gizmos.DrawLine(rightBackup, rightTopup);
                Gizmos.DrawLine(rightTopup, leftTopup);

                Gizmos.DrawLine(leftTopdown, leftBackdown);
                Gizmos.DrawLine(leftBackdown, rightBackdown);
                Gizmos.DrawLine(rightBackdown, rightTopdown);
                Gizmos.DrawLine(rightTopdown, leftTopdown);

                Gizmos.DrawLine(leftTopup, leftTopdown);
                Gizmos.DrawLine(rightTopup, rightTopdown);
                Gizmos.DrawLine(leftBackup, leftBackdown);
                Gizmos.DrawLine(rightBackup, rightBackdown);
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