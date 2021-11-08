using Needle.Timeline;
using UnityEngine;

[ExecuteAlways]
public class SpheresFromData : InstancesFromData<SphereLogic, SpheresFromData.SphereData>
{
    [ContextMenu("Reset Data")]
    internal void ResetStuff() => ResetData();

    public class SphereData
    {
        public Vector3 position = Vector3.zero;
        public float radius = 1f;
    }
    
    public override void ApplyDataToBehaviour(SphereData dat, SphereLogic inst)
    {
        inst.targetPosition = dat.position;
        inst.targetScale = dat.radius;
    }

    public override void ApplyPhysics(SphereLogic sph)
    {
        var pos = sph.transform.position;
        sph.rigid.AddForce((transform.position - pos).normalized * force);
        sph.rigid.AddForce((sph.targetPosition - pos).normalized * sph.force);
        sph.transform.localScale = Vector3.one * sph.targetScale;
    }

    public override void DrawGizmo(SphereLogic inst)
    {
        Gizmos.DrawWireSphere(inst.targetPosition, inst.targetScale);
        Gizmos.DrawLine(inst.targetPosition, inst.transform.localPosition);
    }
}
