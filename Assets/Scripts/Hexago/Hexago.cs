using System;
using System.Collections;
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
        private Vector3[] _hexPoints = new Vector3[6];

        public float HexSize = 1f;

        public int2 StartPoint = int2.zero;

        public Transform Target;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
          var hex=  PixelToHex(Target.position);
            hex.GetHexPoint();
           //Debug.Log($"Vector3 {HexToPixel(hex)}");
        }

        private Hex PixelToHex(Vector3 position)
        {
            var point =new Vector3((position.x-StartPoint.x)/HexSize,0, (position.y - StartPoint.y)/ HexSize);

            var q = (Mathf.Sqrt(3) / 3f * point.x +(- 1f / 3f) * point.z) / HexSize + StartPoint.x;
            var r = (0*point.x+ 2f / 3f * position.z);

            var frac = new Vector3(q, r, -q - r);
            var hex = HexRound(frac);
            return hex;
        }

        private Vector3 HexToPixel(Hex h)
        {
            var x = (float)(Math.Sqrt(3) / 3f * h.q + Math.Sqrt(3) / 2f * h.r) * HexSize;

            var z = (0 * h.q + 3f / 2f * h.r) * HexSize;

            return new Vector3(x, 0, z);
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

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var pointX = StartPoint.x + i * Mathf.Sqrt(3) * HexSize;
                    if (j % 2 == 0)
                    {
                        pointX = Mathf.Sqrt(3) / 2* HexSize + pointX;
                    }
                    var point = new Vector3(pointX, 0, j * (3f / 2f * HexSize)+StartPoint.y);
                    DrawHex(point);
                }
            }


        }

        private void DrawHex(Vector3 center)
        {
         
            for (int i = 0; i < 6; i++)
            {
                var angle_deg = 60 * i - 30;
                var angle_rad = Mathf.PI / 180f * angle_deg;
                _hexPoints[i] = new Vector3(center.x + HexSize * Mathf.Cos(angle_rad), 0, center.z + HexSize * Mathf.Sin(angle_rad));
            }
            for (int i = 0; i < _hexPoints.Length; i++)
            {
                var index = (i + 1) % (_hexPoints.Length);
                Gizmos.DrawLine(_hexPoints[i], _hexPoints[index]);
            }
        }
    }
}