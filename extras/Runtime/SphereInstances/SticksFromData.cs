using Needle.Timeline;
using UnityEngine;

[ExecuteAlways]
public class SticksFromData : InstancesFromData<StickLogic, StickLogic.StickData>
{
    [ContextMenu("Reset Data")]
    internal void ResetStuff() => ResetData();
    
    public override void ApplyDataToBehaviour(StickLogic.StickData data, StickLogic behaviour)
    {
        behaviour.stickData = data;
    }

    public override void ApplyPhysics(StickLogic sph)
    {
        // var pos = sph.transform.position;
        // sph.rigid.AddForce((transform.position - pos).normalized * force);
        // sph.rigid.AddForce((sph.stickData.from - pos).normalized * force);
        // // sph.transform.localScale = Vector3.one * sph.targetScale;
        
        var tr = sph.transform;
        var dat = sph.stickData;
        // tr.localPosition = (dat.from + dat.to) / 2;
        
        // if (lerpRotation)
        //     tr.localRotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(dat.to - dat.from), Time.fixedDeltaTime * 10f);
        // else
        //     tr.localRotation = Quaternion.LookRotation(dat.to - dat.from);
        
        var len = Vector3.Distance(dat.from, dat.to);
        tr.localScale = new Vector3(dat.thickness, dat.thickness, len);

        var fromNow = sph.transform.position + sph.transform.forward * len * 0.5f;
        var toNow = sph.transform.position - sph.transform.forward * len * 0.5f;
        sph.rigid.AddForceAtPosition((dat.from - fromNow).normalized * force, fromNow);
        sph.rigid.AddForceAtPosition((dat.to - toNow).normalized * force, toNow);
        
        sph.materialInstance.color = dat.color;
    }

    public override void DrawGizmo(StickLogic behaviour)
    {
        if (!behaviour || behaviour.stickData == null) return;
        var c = behaviour.stickData.color;
        c.a = 0.1f;
        Gizmos.color = c;
        Gizmos.DrawLine(behaviour.stickData.from, behaviour.stickData.to);
    }
}
