using UnityEngine;

namespace Needle.Timeline
{
    [ExecuteAlways]
    public class TransformFromData : InstancesFromData<Transform, SpheresFromData.SphereData>
    {
        [ContextMenu("Reset Data")]
        internal void ResetStuff() => ResetData();
        
        public override void ApplyDataToBehaviour(SpheresFromData.SphereData data, Transform behaviour)
        {
            behaviour.position = data.position;
            behaviour.localScale = data.radius * Vector3.one;
        }

        public override void ApplyPhysics(Transform behaviour){}

        public override void DrawGizmo(Transform behaviour)
        {
            Gizmos.DrawWireSphere(behaviour.position, behaviour.lossyScale.x);
        }
    }
}
