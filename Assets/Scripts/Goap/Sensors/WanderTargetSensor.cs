using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Goap.Sensors
{
    public class WanderTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {

        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var position =GetRandomPosition(agent);
            return new PositionTarget(position);
        }

        public override void Update()
        {

        }

        private Vector3 GetRandomPosition(IActionReceiver agent)
        {
            var random = Random.insideUnitCircle * 5;
            var position = agent.Transform.position + new Vector3(random.x, 0, random.y);
            return position;
        }
    }
}