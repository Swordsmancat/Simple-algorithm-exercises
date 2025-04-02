using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Goap.Behaviors
{
    public class AgentMoveBehavior : MonoBehaviour
    {
        private AgentBehaviour _agentBehaviour;
        private ITarget _currentTarget;
        private Vector3 _movePosition;
        private Vector3 _lastPosition;

        [SerializeField] private float _MoveSpeed =5f;
        [SerializeField] private float _MinMoveDistance =0.25f;

        private void Awake()
        {
            _agentBehaviour = GetComponent<AgentBehaviour>();
        }

        private void OnEnable()
        {
            _agentBehaviour.Events.OnTargetInRange += EventsOnTargetInRange;
            _agentBehaviour.Events.OnTargetChanged += EventsOnTargetChange;
            _agentBehaviour.Events.OnTargetNotInRange += EventsOnTargetNotInRange;

        }

        private void OnDisable()
        {
            _agentBehaviour.Events.OnTargetInRange -= EventsOnTargetInRange;
            _agentBehaviour.Events.OnTargetChanged -= EventsOnTargetChange;
            _agentBehaviour.Events.OnTargetNotInRange -= EventsOnTargetNotInRange;
        }

        private void Update()
        {
            if (_currentTarget == null)
            {
                return;
            }
            if (_MinMoveDistance <= Vector3.Distance(_currentTarget.Position, _lastPosition))
            {
                _lastPosition = _currentTarget.Position;
                var dir = _movePosition - transform.position;
                transform.position += dir.normalized * Time.deltaTime * _MoveSpeed;
            }

        }

        private void EventsOnTargetNotInRange(ITarget target)
        {

        }

        private void EventsOnTargetChange(ITarget target, bool inRange)
        {
            _currentTarget =target;
            _lastPosition = _currentTarget.Position;
            _movePosition =target.Position;

        }

        private void EventsOnTargetInRange(ITarget target)
        {
            _currentTarget = target;
        }
    }
}