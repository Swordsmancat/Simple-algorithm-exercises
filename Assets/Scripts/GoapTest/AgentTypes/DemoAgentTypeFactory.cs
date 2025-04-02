using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using System.Collections;
using UnityEngine;

namespace CrashKonijn.Docs.GettingStarted
{
    public class DemoAgentTypeFactory : AgentTypeFactoryBase
    {
        public override IAgentTypeConfig Create()
        {
            var factory = new AgentTypeBuilder("ScriptDemoAgent");

            factory.AddCapability<IdleCapabilityFactory>();
            
            return factory.Build();
        }
    }
}