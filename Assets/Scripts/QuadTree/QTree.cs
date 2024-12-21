using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.QuadTree
{
    [Flags]
    public enum DirEnum
    {
        lf =1<<0,
        lb =1<<1,
        rf =1<<2,
        rb =1<<3,
    }

    public class QTree
    {
        public QtRect qtRect;


        private  int MAX_DEEPTH = 4;

        private  int MAX_Threshold = 4;

        public List<Vector2> ChildCount;

        public Dictionary<DirEnum, QTree> ChildMap;

        public bool isdivide = false;


        public QTree(QtRect rect, int MAX_DEEPTH =4)
        {
            qtRect = rect;

            isdivide = false;
            this.MAX_DEEPTH = MAX_DEEPTH;
            ChildCount = new List<Vector2>(MAX_DEEPTH);
        }



        public void InSert(Vector2 point)
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
            var subrectlf = new QtRect
            {
                Height = rect.Height / 2,
                Width = rect.Width / 2,
                X = rect.X - rect.Width / 4,
                Y = rect.Y + rect.Height / 4
            };
            var lf = new QTree(subrectlf);
            ChildMap.Add(DirEnum.lf, lf);

            var subrectlb = new QtRect
            {
                Height = rect.Height / 2,
                Width = rect.Width / 2,
                X = rect.X - rect.Width / 4,
                Y = rect.Y - rect.Height / 4
            };
            var lb = new QTree(subrectlb);
            ChildMap.Add(DirEnum.lb, lb);

            var subrectrb = new QtRect
            {
                Height = rect.Height / 2,
                Width = rect.Width / 2,
                X = rect.X + rect.Width / 4,
                Y = rect.Y - rect.Height / 4
            };
            var rb = new QTree(subrectrb);
            ChildMap.Add(DirEnum.rb, rb);

            var subrectrf = new QtRect
            {
                Height = rect.Height / 2,
                Width = rect.Width / 2,
                X = rect.X + rect.Width / 4,
                Y = rect.Y + rect.Height / 4
            };
            var rf = new QTree(subrectrf);
            ChildMap.Add(DirEnum.rf, rf);

            isdivide = true;
        }

        private bool Contain(Vector2 point, QtRect rect)
        {
            if (point.x <= rect.X + rect.Width / 2 &&
                point.x >= rect.X - rect.Width / 2 &&
                point.y <= rect.Y + rect.Height / 2 &&
                point.y >= rect.Y - rect.Height / 2)
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
                rect.Y + rect.Height / 2 < qtRect.Y - qtRect.Height / 2);
        }

        public void Query( QtRect queryRect, List<Vector2> points,ref int allCount)
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

        private bool DirInsert(Vector2 point)
        {
            foreach (DirEnum dir in Enum.GetValues(typeof(DirEnum)))
            {
                switch (dir)
                {
                    case DirEnum.lf:
                       
                        if (Contain(point,ChildMap[DirEnum.lf].qtRect))
                        {
                            ChildMap[DirEnum.lf].InSert(point);
                            return true;
                        }
                        break;
                    case DirEnum.lb:
                        if (Contain(point, ChildMap[DirEnum.lb].qtRect))
                        {
                            ChildMap[DirEnum.lb].InSert(point);
                            return true;
                        }
                        break;
                    case DirEnum.rf:
                        if (Contain(point, ChildMap[DirEnum.rf].qtRect))
                        {
                            ChildMap[DirEnum.rf].InSert(point);
                            return true;
                        }
                        break;
                    case DirEnum.rb:
                        if (Contain(point, ChildMap[DirEnum.rb].qtRect))
                        {
                            ChildMap[DirEnum.rb].InSert(point);
                            return true;
                        }
                        break;
                    default:
                        return false;
                }
            }
            return false;
           
        }


    }
}