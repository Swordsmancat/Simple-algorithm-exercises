using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Stree
{
    public class Agent : MonoBehaviour
    {
        public float MaxSpeed = 4;

        public float MaxForce = 10;

        public float RotateSpeed = 5f;
        public float SlowingRadius = 4f;


        public float WanderRadius = 15f;

        public float WanderAngle = 45f;

        private float _wanderTime = 0;


        public float WanderDistance = 15f;
        public float WanderRate = 1f;
        public float ANGLE_CHANGE = 5f;

        public float PursuitMaxVelocity = 4;

        public Rigidbody _rigidbody;

        public AgentTarget Target;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var seekWeight = 1f;
            var wanderWeight = 1f;
            var seek= Seek(Target.transform.position)* seekWeight;
           var wander = Wander(Target.transform.position)* wanderWeight;
           var pursuit = Pursuit(Target.transform.position, Target._rigidbody.linearVelocity) * wanderWeight;
           var evade = Evade(Target.transform.position, Target._rigidbody.linearVelocity) * wanderWeight;
            var vel = evade;

            Debug.DrawLine(transform.position, transform.position + seek, Color.red);
            Debug.DrawLine(transform.position, transform.position + wander, Color.green);
          //  RotateDir(vel);
            _rigidbody.AddForce(vel);
        }

        public Vector3 Evade(Vector3 target, Vector3 velocity)
        {
            var distance = target - transform.position;
            var speed = distance.magnitude / PursuitMaxVelocity;
            var futurePosition = target + velocity * speed;
            return Flee(futurePosition);
        }

        public Vector3 Wander(Vector3 target)
        {
            _wanderTime += Time.deltaTime;
            if (_wanderTime > 4f)
            {
                _wanderTime = 0;
            }


            Vector3 circleCenter = _rigidbody.linearVelocity.normalized * WanderDistance;

            WanderAngle += Random.Range(-WanderRate, WanderRate);
            WanderAngle = Mathf.Repeat(WanderAngle, 360);


            float x = Mathf.Cos(WanderAngle * Mathf.Deg2Rad) * WanderRadius;
            float z = Mathf.Sin(WanderAngle * Mathf.Deg2Rad) * WanderRadius; 

            Vector3 wanderTarget = circleCenter + new Vector3(x, 0, z); 

            var dist =Vector3.Distance(target, transform.position);
            if (dist > WanderRadius)
            {
                Vector3 desiredVelocity = (target - transform.position).normalized * MaxSpeed;
                desiredVelocity -= _rigidbody.linearVelocity;
                wanderTarget += desiredVelocity;
            }


            return wanderTarget;

        }

        public Vector3 Pursuit(Vector3 target,Vector3 velocity)
        {
            var distance =target-transform.position;
            var speed = distance.magnitude / PursuitMaxVelocity;
            var futurePosition = target + velocity* speed;
            return Seek(futurePosition);
        }

        private Vector3 GetWanderTarget(Vector3 target)
        {
            float theta = WanderAngle * Mathf.Deg2Rad;
            var circleCenter = target + transform.forward * WanderDistance;
            var targetOffset = new Vector3(Mathf.Cos(theta) * WanderRadius, 0, Mathf.Sin(theta) * WanderRadius);

            WanderAngle += Random.Range(-WanderRate, WanderRate);

            return circleCenter + targetOffset;
        }

        public Vector3 SetAngle(Vector3 dir, float angle)
        {
            var len =dir.sqrMagnitude;
            var vector = new Vector3(Mathf.Cos(angle) * len, 0, Mathf.Sign(angle) * len);
            return vector;
        }



        public Vector3 Arrival(Vector3 target)
        {
            var desired_velocity = target - transform.position;
            var distance =Vector3.Distance(target, transform.position);
            if (distance < SlowingRadius)
            {
                desired_velocity = Vector3.Normalize(desired_velocity) * MaxSpeed * (distance / SlowingRadius);
            }
            else
            {
                desired_velocity = Vector3.Normalize(desired_velocity) * MaxSpeed;
            }
            return desired_velocity - _rigidbody.linearVelocity;
        }

        public Vector3 Flee(Vector3 target)
        {
            return -Seek(target);
            var dir = Vector3.Normalize(transform.position- target);
            var desiredVel = dir * MaxSpeed;
            transform.rotation =Quaternion.LookRotation(desiredVel);
            return desiredVel- _rigidbody.linearVelocity;
        }

        public Vector3 Seek(Vector3 target) 
        {
            var dir = Vector3.Normalize(target -transform.position);
            var desiredVel =dir*MaxSpeed;
            return desiredVel - _rigidbody.linearVelocity;
        }

        public void RotateDir(Vector3 dir)
        {
            if (dir == Vector3.zero)
            {
                return;
            }
            var lookRotation =Quaternion.LookRotation(dir);
            transform.rotation =Quaternion.Slerp(transform.rotation,lookRotation,Time.deltaTime* RotateSpeed);
        }
    }
}