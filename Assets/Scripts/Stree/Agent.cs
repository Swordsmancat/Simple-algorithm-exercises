using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Stree
{
    public class Agent : MonoBehaviour
    {
        public float MaxSpeed = 4;
        public Rigidbody _rigidbody;

        public GameObject Target;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Seek(Target.transform.position);
        }
        public void Seek(Vector3 target) 
        {
            var dir = Vector3.Normalize(target -transform.position);
            var desiredVel =dir*MaxSpeed;
            var  steering = desiredVel - _rigidbody.linearVelocity;
            _rigidbody.AddForce(steering);
            transform.LookAt(target);

        }
    }
}