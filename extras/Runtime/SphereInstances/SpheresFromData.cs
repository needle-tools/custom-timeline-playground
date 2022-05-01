using Needle.Timeline;
using UnityEngine;

[ExecuteAlways]
public class SpheresFromData : InstancesFromData<SphereLogic, SpheresFromData.SphereData>
{
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    [ContextMenu("Reset Data")]
    internal void ResetStuff() => ResetData();

    public class SphereData
    {
        public Vector3 position = Vector3.zero;
        public float radius = 1f;
        public float force = 50;
        // public float drag;
        public Color mainColor = Color.white;
    }
    
    public override void ApplyDataToBehaviour(SphereData dat, SphereLogic inst)
    {
        inst.targetPosition = dat.position; 
        inst.targetScale = dat.radius;
        inst.force = dat.force;
        // inst.rigid.drag = dat.drag;
        if (!inst.rend && inst.TryGetComponent(out Renderer r)) inst.rend = r;
        if (inst.rend )
        {
            // MaterialPropertyBlock
            // if (inst.block == null) inst.block = new MaterialPropertyBlock();
            // inst.block.SetColor(Color1, dat.mainColor);
            // inst.block.SetColor(BaseColor, dat.mainColor);
            
            // Enforce separate material instances for easier recording
            inst.rend.material.SetColor(Color1, dat.mainColor);
            inst.rend.material.SetColor(BaseColor, dat.mainColor);
            
            inst.rend.SetPropertyBlock(inst.block);
        }
    }

    public override void ApplyPhysics(SphereLogic sph)
    {
        if (sph.rigid.isKinematic)
        {
            sph.rigid.position = sph.targetPosition;
            sph.transform.localScale = Vector3.one * sph.targetScale;
            return;
        }
        var pos = sph.transform.position;
        // sph.rigid.AddForce((transform.position - pos).normalized * force);
        sph.rigid.AddForce((sph.targetPosition - pos).normalized * sph.force);
        sph.transform.localScale = Vector3.one * sph.targetScale;
    } 

    public override void DrawGizmo(SphereLogic inst)
    {
        Gizmos.color = Color.Lerp(Color.green, Color.red, inst.force / 100f);
        Gizmos.DrawWireSphere(inst.targetPosition, inst.targetScale);
        Gizmos.DrawLine(inst.targetPosition, inst.transform.localPosition);
    }
}
