using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Goap.Actions
{
    public class WanderAction : GoapActionBase<CommonData>
    {


        public override void Start(IMonoAgent agent, CommonData data)
        {
            base.Start(agent, data);
            data.Timer = Random.Range(1, 2);
        }

        public override IActionRunState Perform(IMonoAgent agent, CommonData data, IActionContext context)
        {
            data.Timer -= context.DeltaTime;
            if (data.Timer > 0)
            {
                return ActionRunState.Continue;
            }
            return ActionRunState.Stop;
        }

        public override void End(IMonoAgent agent, CommonData data)
        {
            base.End(agent, data);
        }

    }
}