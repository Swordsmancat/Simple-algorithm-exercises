using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class TestFib : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log(Fib(3));
            }
        }

        private  int Fib(int n)
        {
            // 终止条件 f(1) = 0, f(2) = 1
            if (n == 1 || n == 2)
                return n - 1;
            // 递归调用 f(n) = f(n-1) + f(n-2)
            int res = Fib(n - 1) + Fib(n - 2);
            // 返回结果 f(n)
            return res;
        }
    }
}