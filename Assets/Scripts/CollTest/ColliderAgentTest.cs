using System.Collections;
using UnityEngine;

namespace Assets.Scripts.CollTest
{
    public class ColliderAgentTest : MonoBehaviour
    {
        public Vector3 Velocity;

        public float Radius = 1;

        private float _width;

        private float _height;

        public float Mass;

        public int Dir =40;

        public void Init(float width,float height, Vector3 velocity,float radius,float mass)
        {
            _width =width;
            _height =height;    
            Velocity =velocity;
            Radius =radius;
            Mass =mass;
            transform.localScale = new Vector3(2*radius, 2 * radius, 1);
        }

        public void Update()
        {
            transform.position += Velocity;
            if (transform.position.x > _width - Radius)
            {
                transform.position = new Vector3(_width - Radius, transform.position.y, 0);
                Velocity.x *= -1;
            }
            else if (transform.position.x < Radius- _width)
            {
                transform.position = new Vector3(Radius - _width, transform.position.y, 0);
                Velocity.x *= -1;
            }

            if (transform.position.y > _height - Radius)
            {
                transform.position = new Vector3(transform.position.x, _height - Radius, 0);
                Velocity.y *= -1;
            }
            else if (transform.position.y < Radius - _height)
            {
                transform.position = new Vector3(transform.position.x, Radius - _height, 0);
                Velocity.y *= -1;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position+Velocity* Dir);
        }




    }
}