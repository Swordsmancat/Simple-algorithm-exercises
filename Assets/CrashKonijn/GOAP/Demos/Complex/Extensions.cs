﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CrashKonijn.Goap.Demos.Complex
{
    public static class Extensions
    {
        public static GameObject Closest(this IEnumerable<GameObject> items, Vector3 position)
        {
            return items
                .OrderBy(x => Vector3.Distance(x.transform.position, position))
                .FirstOrDefault();
        }
        
        public static T Closest<T>(this IEnumerable<T> items, Vector3 position)
            where T : MonoBehaviour
        {
            return items
                .OrderBy(x => Vector3.Distance(x.transform.position, position))
                .FirstOrDefault();
        }
    }
}