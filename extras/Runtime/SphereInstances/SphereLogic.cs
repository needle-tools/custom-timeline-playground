using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Needle.Timeline
{
    public class SphereLogic : MonoBehaviour
    {
        public Vector3 targetPosition;
        public float targetScale;
        public float force;
        public Rigidbody rigid;
        
        private void OnEnable()
        {
            if(!rigid) rigid = GetComponent<Rigidbody>();
        }
    }
}
