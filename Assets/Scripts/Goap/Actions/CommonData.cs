using CrashKonijn.Agent.Core;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Goap.Actions
{
    public class CommonData : IActionData
    {
        public ITarget Target
        {
            get;
            set;
        }

        public float Timer
        {
            get;
            set;
        }
    }
}