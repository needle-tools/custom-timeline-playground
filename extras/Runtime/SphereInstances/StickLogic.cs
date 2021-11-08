using UnityEngine;

namespace Needle.Timeline
{
    [ExecuteAlways]
    public class StickLogic : MonoBehaviour
    {
        internal StickData stickData;
        internal Rigidbody rigid;
        internal MeshRenderer rend;
        internal Material materialInstance;
        internal Material originalMaterial;
        
        private void OnEnable()
        {
            if (!rigid) rigid = GetComponent<Rigidbody>();
            if (!rend) rend = GetComponent<MeshRenderer>();
            if (!materialInstance)
            {
                originalMaterial = rend.sharedMaterial;
                materialInstance = Instantiate(originalMaterial);
                materialInstance.hideFlags = HideFlags.DontSave;
            }
        }

        private void OnDisable()
        {
            if (materialInstance) DestroyImmediate(materialInstance);
            rend.sharedMaterial = originalMaterial;
        }

        public class StickData
        {
            public Vector3 from;
            public Vector3 to;
            public float thickness = 0.1f;
            public Color color = Color.white;
        }
    }
}
