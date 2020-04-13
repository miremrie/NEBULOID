using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MarchingSquares
{
    public class Rotate : MonoBehaviour
    {
        public float speed;

        void Update()
        {
            transform.Rotate(Vector3.forward, speed * Time.deltaTime);
        }
    }
}
