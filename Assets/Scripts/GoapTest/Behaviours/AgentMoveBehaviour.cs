using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using System.Collections;
using UnityEngine;

namespace CrashKonijn.Docs.GettingStarted
{
    public class AgentMoveBehaviour : MonoBehaviour
    {
        private AgentBehaviour _agent;
        private ITarget _CurrentTarget;
        private bool _shouldMove;

        private void Awake()
        {
            _agent = GetComponent<AgentBehaviour>();
        }

        private void OnEnable()
        {
            _agent.Events.OnTargetInRange += OnTargetInRange;
            _agent.Events.OnTargetChanged += OnTargetChanged;
            _agent.Events.OnTargetNotInRange += TargetNotInRange;
            _agent.Events.OnTargetLost += TargetLost;
        }

        private void OnDisable()
        {
            _agent.Events.OnTargetInRange -= OnTargetInRange;
            _agent.Events.OnTargetChanged -= OnTargetChanged;
            _agent.Events.OnTargetNotInRange -= TargetNotInRange;
            _agent.Events.OnTargetLost -= TargetLost;
        }

        private void Update()
        {
            if(!_shouldMove)
            {
                return;
            }

            if(_CurrentTarget == null)
            {
                return;
            }

            transform.position =Vector3.MoveTowards(transform.position, _CurrentTarget.Position, Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            if (_CurrentTarget == null)
            {
                return;
            }
            Gizmos.DrawLine(transform.position, _CurrentTarget.Position);

        }

        private void OnTargetInRange(ITarget target)
        {
            _shouldMove = true;
        }

        private void OnTargetChanged(ITarget target, bool inRange)
        {
            _CurrentTarget = target;
            _shouldMove = !inRange;
        }

        private void TargetNotInRange(ITarget target)
        {
           _shouldMove = true;
        }

        private void TargetLost()
        {
            _CurrentTarget = null;
            _shouldMove = false;
        }
    }
}