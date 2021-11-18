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

        public MaterialPropertyBlock block;
        public Renderer rend;
    }
}
