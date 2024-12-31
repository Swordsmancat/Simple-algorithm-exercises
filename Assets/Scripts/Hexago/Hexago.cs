using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Hexago
{
    public class Hexago : MonoBehaviour
    {
        public class Hex
        {
            public Hex(int q, int r, int s)
            {
                this.q = q;
                this.r = r;
                this.s = s;
            }
            public int q;
            public int r;
            public int s;

            public void GetHexPoint()
            {
                Debug.Log($"({q},{r},{s})");
            }
        }

       public struct Orientation
        {
            public Orientation(double f0, double f1, double f2, double f3, double b0, double b1, double b2, double b3, double start_angle)
            {
                this.f0 = f0;
                this.f1 = f1;
                this.f2 = f2;
                this.f3 = f3;
                this.b0 = b0;
                this.b1 = b1;
                this.b2 = b2;
                this.b3 = b3;
                this.start_angle = start_angle;
            }
            public readonly double f0;
            public readonly double f1;
            public readonly double f2;
            public readonly double f3;
            public readonly double b0;
            public readonly double b1;
            public readonly double b2;
            public readonly double b3;
            public readonly double start_angle;
        }

        private Orientation pointy = new Orientation(Math.Sqrt(3.0), Math.Sqrt(3.0) / 2.0, 0.0, 3.0 / 2.0, Math.Sqrt(3.0) / 3.0, -1.0 / 3.0, 0.0, 2.0 / 3.0, 0.5);

        private Orientation flat = new Orientation(3.0 / 2.0, 0.0, Math.Sqrt(3.0) / 2.0, Math.Sqrt(3.0), 2.0 / 3.0, 0.0, -1.0 / 3.0, Math.Sqrt(3.0) / 3.0, 0.0);

        private Orientation _orientation;

        public float HexSize = 1f;

        public int2 StartPoint = int2.zero;

        public List<Transform> Targets;

        public Dictionary<int,List<Transform>> CubeDict = new Dictionary<int,List<Transform>>();
        // Use this for initialization
        void Start()
        {
            _orientation = flat;
        }

        private int GetIndexKeyFormPosition(Hex hex)
        {
            return hex.q+100*hex.r+10000*hex.s;
        }


        // Update is called once per frame
        void Update()
        {

            foreach (Transform t in Targets)
            {
                var hex = PixelToHex(t.position);
                hex.GetHexPoint();

                DebugDrawHexList(PolygonCorners(hex));
            }

        }

        private void IndexTest()
        {
            CubeDict.Clear();
            foreach (var item in Targets)
            {
                var hex = PixelToHex(item.position);
                var index = GetIndexKeyFormPosition(hex);
                if (!CubeDict.ContainsKey(index))
                {
                    CubeDict.Add(index, new List<Transform>());
                }
                CubeDict[index].Add(item);
            }

            foreach (var item in CubeDict)
            {
                foreach (var obj in item.Value)
                {
                    Debug.Log($"Index {item.Key} ,Name:{obj.name}");
                }
            }
        }

        private Hex PixelToHex(Vector3 position)
        {
            Orientation M = _orientation;
            var pt = new Vector3((position.x-StartPoint.x)/HexSize,0, (position.z - StartPoint.y)/ HexSize);
            var q = (float)(M.b0 * pt.x + M.b1 * pt.z);
            var r = (float)(M.b2 * pt.x + M.b3 * pt.z);

            var frac = new Vector3(q, r, -q - r);
            var hex = HexRound(frac);
            return hex;
        }

        private Vector3 HexToPixel(Hex h)
        {
            Orientation M = _orientation;
            var x =(float)(M.f0 * h.q + M.f1 * h.r) * HexSize;
            var z = (float)(M.f2 * h.q + M.f3 * h.r) * HexSize;
            return new Vector3(x + StartPoint.x,0, z + StartPoint.y);
        }



        private Hex HexRound(Vector3 frac)
        {
            var qi = (int)Mathf.Round(frac.x);
            var ri = (int)Mathf.Round(frac.y);
            var si = (int)Mathf.Round(frac.z);

            var q_diff = Mathf.Abs(qi - frac.x);
            var r_diff = Mathf.Abs(ri - frac.y);
            var s_diff = Mathf.Abs(si - frac.z);

            if (q_diff > r_diff && q_diff > s_diff)
            {
                qi = -ri - si;
            }
            else if (r_diff > s_diff)
            {
                ri = -qi - si;
            }
            else
            {
                si = -qi - ri;
            }
            return new Hex(qi, ri, si);
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;

            for (int i = -10; i < 10; i++)
            {
                for (int j = -10; j < 10; j++)
                {
                    var hex = new Hex(i, -i - j, j);
                    Gizmos.color = Color.black;
                    DrawHexList(PolygonCorners(hex));
                }
            }


        }

        private Vector3 HexCornerOffset(int corner)
        {
            Orientation M = _orientation;
            var angle =(float)(2f * Math.PI * (M.start_angle - corner) / 6f);
            return new Vector3(HexSize * Mathf.Cos(angle),0, HexSize * Mathf.Sin(angle));
        }

        public List<Vector3> PolygonCorners(Hex h)
        {
            List<Vector3> corners = new List<Vector3> { };
            Vector3 center = HexToPixel(h);
            for (int i = 0; i < 6; i++)
            {
                Vector3 offset = HexCornerOffset(i);
                corners.Add(new Vector3(center.x + offset.x,0, center.z + offset.z));
            }
            return corners;
        }

        private void DebugDrawHexList(List<Vector3> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var index = (i + 1) % (list.Count);
                Debug.DrawLine(list[i], list[index]);
            }
        }

        private void DrawHexList(List<Vector3> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var index = (i + 1) % (list.Count);
                Gizmos.DrawLine(list[i], list[index]);
            }
        }


    }
}