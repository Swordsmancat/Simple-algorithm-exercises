using Assets.Scripts.Stree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.CollTest
{
    [Serializable]
    public class Point
    {

        [Range(0.01f, 2)]
        public float x;


        [Range(0.01f, 2)]
        public float y;
    }

    public class ColliderAgentManager : MonoBehaviour
    {

        public float Width = 10;
        public float Height = 10;

        public GameObject Agent;

        public Point VelocityRange;
        
        public Point RadiusRange;

        public Point MassRange;

        public List<ColliderAgentTest> _agents = new List<ColliderAgentTest>();
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            CheckAgentCollider();

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                var agent = Instantiate(Agent);
                var agentdata = agent.AddComponent<ColliderAgentTest>();
                Vector3 randomVelocity = new Vector3(0.05f, 0, 0);
                agentdata.Init(Width, Height, randomVelocity,1, 1);
                _agents.Add(agentdata);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                var agent = Instantiate(Agent);
                var agentdata = agent.AddComponent<ColliderAgentTest>();
                Vector3 randomVelocity = new Vector3(-0.05f, 0, 0);
                agentdata.Init(Width, Height, randomVelocity, 2, 2);
                _agents.Add(agentdata);
            }

        }


        //动能守恒公式
        // |v1'| =|v1| +(2*m2/(m1+m2))*(|v2|-|v1|,|x2|-|x1|)/(||x2|-|x1||^2)*(|x2|-|x1|)
        public void CheckAgentCollider()
        {
            for (int i = 0; i < _agents.Count; i++)
            {
                for (int j = i; j < _agents.Count; j++)
                {
                    var agent = _agents[i];
                    var other = _agents[j];
                    var dis = Vector3.Distance(agent.transform.position, other.transform.position);
                    if (dis <= agent.Radius + other.Radius)
                    {

                        var overlap = dis - (agent.Radius + other.Radius);
                        var dir = other.transform.position - agent.transform.position;
                        dir = Vector3.Normalize(dir) * (overlap * 0.5f);

                        agent.transform.position += dir;
                        other.transform.position -= dir;

                        var d = agent.Radius + other.Radius;
                        var agentVel = agent.Velocity;

                        var subDir = other.transform.position - agent.transform.position;
                        var subVel = other.Velocity - agent.Velocity;
                        var subMas = 2 * other.Mass / (agent.Mass + other.Mass);
                        var newVel = Vector3.Dot(subVel, subDir) / (d * d) * subDir;
                        newVel = agent.Velocity + subMas * newVel;
                        agent.Velocity = newVel;


                        //TODO:将原本的数据复制出来，使用初始值进行计算
                        subDir = agent.transform.position - other.transform.position;
                        subVel = agentVel - other.Velocity;
                        subMas = 2 * agent.Mass / (agent.Mass + other.Mass);
                        newVel = Vector3.Dot(subVel, subDir) / (d * d) * subDir;
                        newVel = other.Velocity + subMas * newVel;
                        other.Velocity = newVel;

                    }
                }
            }
            //foreach (var agent in _agents)
            //{
            //    foreach (var other in _agents)
            //    {
            //        if (agent.Equals(other))
            //        {
            //            continue;
            //        }



            //    }
            //}
        }

        public void CreateAgent()
        {
            var agent =Instantiate(Agent);
            var agentdata = agent.AddComponent<ColliderAgentTest>();
            Vector3 randomVelocity = new Vector3(Random.Range(VelocityRange.x, VelocityRange.y), Random.Range(VelocityRange.x, VelocityRange.y), 0);
            agentdata.Init(Width, Height, randomVelocity, Random.Range(RadiusRange.x, RadiusRange.y), Random.Range(MassRange.x, MassRange.y));
            _agents.Add(agentdata);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(new Vector3(-Width, -Height, 0), new Vector3(Width, -Height, 0));
            Gizmos.DrawLine(new Vector3(Width, -Height, 0), new Vector3(Width, Height, 0));

            Gizmos.DrawLine(new Vector3(Width, Height, 0), new Vector3(-Width, Height, 0));
            Gizmos.DrawLine(new Vector3(-Width, Height, 0), new Vector3(-Width, -Height, 0));
        }
    }
}