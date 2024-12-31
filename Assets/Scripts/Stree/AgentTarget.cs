using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Stree
{
    public class AgentTarget : MonoBehaviour
    {
        public Rigidbody _rigidbody;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            _rigidbody.AddForce(new Vector3(Random.Range(-1,1f),0,Random.Range(-1, 1.2f)));
        }
    }
}