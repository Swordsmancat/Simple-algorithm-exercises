using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.QctTree
{
    [Flags]
    public enum DirEnum
    {
        /// <summary>
        /// 左前上
        /// </summary>
        lfu =1<<0,

        /// <summary>
        /// 左后上
        /// </summary>
        lbu = 1<<1,

        /// <summary>
        /// 左前下
        /// </summary>
        lfd = 1 << 2,

        /// <summary>
        /// 左后下
        /// </summary>
        lbd = 1 << 3,

        /// <summary>
        /// 右前上
        /// </summary>
        rfu = 1 << 4,

        /// <summary>
        /// 右后上
        /// </summary>
        rbu = 1 << 5,

        /// <summary>
        /// 右前下
        /// </summary>
        rfd = 1 << 6,

        /// <summary>
        /// 右后下
        /// </summary>
        rbd = 1 << 7,


    }

    public class QTree
    {
        public QtRect qtRect;


        private  int MAX_DEEPTH = 8;

       // private  int MAX_Threshold = 4;

        public List<Vector3> ChildCount;

        public Dictionary<DirEnum, QTree> ChildMap;

        public bool isdivide = false;


        public QTree(QtRect rect, int MAX_DEEPTH =8)
        {
            qtRect = rect;

            isdivide = false;
            this.MAX_DEEPTH = MAX_DEEPTH;
            ChildCount = new List<Vector3>(MAX_DEEPTH);
        }



        public void InSert(Vector3 point)
        {
            if (ChildCount.Count < MAX_DEEPTH)
            {
                ChildCount.Add(point);
            }
            else
            {
                if (!isdivide)
                {
                    Subdivide(qtRect);
                }

                DirInsert(point);

            }
        }

        private void Subdivide(QtRect rect)
        {
            ChildMap =new Dictionary<DirEnum, QTree>();
            var subrectlfu = new QtRect
            {
                Height = rect.Height / 2,
                Width = rect.Width / 2,
                Lenght = rect.Lenght/2,
                X = rect.X - rect.Width / 4,
                Y = rect.Y + rect.Height / 4,
                Z =rect.Z +rect.Lenght / 4,
                
            };
            var lfu = new QTree(subrectlfu);
            ChildMap.Add(DirEnum.lfu, lfu);

            var subrectlbu = new QtRect
            {
                Height = rect.Height / 2,
                Width = rect.Width / 2,
                Lenght = rect.Lenght / 2,
                X = rect.X - rect.Width / 4,
                Y = rect.Y - rect.Height / 4,
                Z = rect.Z + rect.Lenght / 4,
            };
            var lbu = new QTree(subrectlbu);
            ChildMap.Add(DirEnum.lbu, lbu);

            var subrectlfd = new QtRect
            {
                Height = rect.Height / 2,
                Width = rect.Width / 2,
                Lenght = rect.Lenght / 2,
                X = rect.X - rect.Width / 4,
                Y = rect.Y + rect.Height / 4,
                Z = rect.Z - rect.Lenght / 4,
            };
            var lfd = new QTree(subrectlfd);
            ChildMap.Add(DirEnum.lfd, lfd);

            var subrectlbd = new QtRect
            {
                Height = rect.Height / 2,
                Width = rect.Width / 2,
                Lenght = rect.Lenght / 2,
                X = rect.X - rect.Width / 4,
                Y = rect.Y - rect.Height / 4,
                Z = rect.Z - rect.Lenght / 4,
            };
            var lbd = new QTree(subrectlbd);
            ChildMap.Add(DirEnum.lbd, lbd);


            var subrectrbu = new QtRect
            {
                Height = rect.Height / 2,
                Width = rect.Width / 2,
                Lenght = rect.Lenght / 2,
                X = rect.X + rect.Width / 4,
                Y = rect.Y - rect.Height / 4,
                Z =rect.Z + rect.Lenght / 4,
            };
            var rbu = new QTree(subrectrbu);
            ChildMap.Add(DirEnum.rbu, rbu);

            var subrectrfu = new QtRect
            {
                Height = rect.Height / 2,
                Width = rect.Width / 2,
                Lenght = rect.Lenght / 2,
                X = rect.X + rect.Width / 4,
                Y = rect.Y + rect.Height / 4,
                Z =rect.Z +rect.Lenght / 4,
            };
            var rfu = new QTree(subrectrfu);
            ChildMap.Add(DirEnum.rfu, rfu);

            var subrectrbd = new QtRect
            {
                Height = rect.Height / 2,
                Width = rect.Width / 2,
                Lenght = rect.Lenght / 2,
                X = rect.X + rect.Width / 4,
                Y = rect.Y - rect.Height / 4,
                Z = rect.Z - rect.Lenght / 4,
            };
            var rbd = new QTree(subrectrbd);
            ChildMap.Add(DirEnum.rbd, rbd);

            var subrectrfd = new QtRect
            {
                Height = rect.Height / 2,
                Width = rect.Width / 2,
                Lenght = rect.Lenght / 2,
                X = rect.X + rect.Width / 4,
                Y = rect.Y + rect.Height / 4,
                Z = rect.Z - rect.Lenght / 4,
            };
            var rfd = new QTree(subrectrfd);
            ChildMap.Add(DirEnum.rfd, rfd);

            isdivide = true;
        }

        private bool Contain(Vector3 point, QtRect rect)
        {
            if (point.x <= rect.X + rect.Width / 2 &&
                point.x >= rect.X - rect.Width / 2 &&
                point.y <= rect.Y + rect.Height / 2 &&
                point.y >= rect.Y - rect.Height / 2 &&
                point.z <= rect.X +rect.Lenght /2&&
                point.z >=rect.Z-rect.Lenght/2)
            {
                return true;
            }
            return false;
        }

        private bool NoContainRact(QtRect rect)
        {
            return (rect.X - rect.Width / 2 > qtRect.X + qtRect.Width / 2 ||
                rect.X + rect.Width / 2 < qtRect.X - qtRect.Width / 2 ||
                rect.Y - rect.Height / 2 > qtRect.Y + qtRect.Height / 2 ||
                rect.Y + rect.Height / 2 < qtRect.Y - qtRect.Height / 2||
                rect.Z - rect.Lenght / 2 > qtRect.Z + qtRect.Lenght / 2||
                  rect.Z + rect.Lenght / 2 < qtRect.Z - qtRect.Lenght / 2);
        }

        public void Query( QtRect queryRect, List<Vector3> points,ref int allCount)
        {
            if (NoContainRact(queryRect))
            {
                return;
            }
            foreach (var item in ChildCount)
            {
                allCount++;
                if (Contain(item, queryRect))
                {
                    points.Add(item);
                    Debug.Log($"{item}");
                }
            }
            if(isdivide)
            {
                foreach (var item in ChildMap.Keys)
                {
                    ChildMap[item].Query(queryRect, points,ref allCount);
                }
            }
        }

        private bool DirInsert(Vector3 point)
        {
            foreach (DirEnum dir in Enum.GetValues(typeof(DirEnum)))
            {
                if (Contain(point, ChildMap[dir].qtRect))
                {
                    ChildMap[dir].InSert(point);
                    return true;
                }
            }
            return false;
           
        }


    }
}