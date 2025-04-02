using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using System.Collections;
using UnityEngine;

namespace CrashKonijn.Docs.GettingStarted
{
    public class BrainBehaviour : MonoBehaviour
    {
        private AgentBehaviour _agentBehaviour;
        private GoapActionProvider _actionProvider;
        private GoapBehaviour _goapBehaviour;

        private void Awake()
        {
            _goapBehaviour =FindFirstObjectByType<GoapBehaviour>();
            _agentBehaviour = GetComponent<AgentBehaviour>();
            _actionProvider =GetComponent<GoapActionProvider>();

            if (_actionProvider.AgentTypeBehaviour == null)
            {
                _actionProvider.AgentType = _goapBehaviour.GetAgentType("ScriptDemoAgent");
            }
        }

        private void Start()
        {
            _actionProvider.RequestGoal<IdleGoal>();
        }
    }
}