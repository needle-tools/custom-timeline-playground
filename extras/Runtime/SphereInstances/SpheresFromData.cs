using System.Collections.Generic;
using System.Linq;
using Needle.Timeline;
using UnityEngine;

[ExecuteAlways]
public class SpheresFromData : MonoBehaviour, IAnimated
{
    [ContextMenu("Reset")]
    void Reset()
    {
        foreach (var inst in instances)
        {
            if(!inst) continue;
            if (Application.isPlaying) Destroy(inst.gameObject);
            else DestroyImmediate(inst.gameObject);
        }

        foreach (var inst in buffer)
        {
            if(!inst) continue;
            if (Application.isPlaying) Destroy(inst.gameObject);
            else DestroyImmediate(inst.gameObject);
        }
        
        instances.Clear();
        buffer.Clear();
    }
    
    public SphereLogic template;

    public class SphereData
    {
        public Vector3 position = Vector3.zero;
        public float radius = 1f;
    }

    [Animate]
    public List<SphereData> data = new List<SphereData>();
    
    internal List<SphereLogic> instances = new List<SphereLogic>();
    internal List<SphereLogic> buffer = new List<SphereLogic>();
    private void Update()
    {
        if (!template) return;
        if (instances == null) instances = new List<SphereLogic>();
        if (data == null) data = new List<SphereData>();
        if (buffer == null) buffer = new List<SphereLogic>();
        
        if (instances.Count != data.Count)
        {
            while (instances.Count > data.Count)
            {
                var last = instances.Last();
                instances.Remove(last);
                buffer.Add(last);
                if(last) last.gameObject.SetActive(false);
            }

            while (instances.Count < data.Count)
            {
                if (buffer.Count > 0)
                {
                    var last = buffer.Last();
                    instances.Add(last);
                    buffer.Remove(last);
                    if(last) last.gameObject.SetActive(true);
                }
                else
                {
                    var newInstance = Instantiate(template, transform);
                    newInstance.gameObject.hideFlags = HideFlags.DontSave;
                    instances.Add(newInstance);
                    newInstance.gameObject.SetActive(true);
                }
            }
        }

        for (int i = 0; i < data.Count; i++)
        {
            var dat = data[i];
            var inst = instances[i];
            inst.targetPosition = dat.position;
            inst.targetScale = dat.radius;
        }

        UpdatePhysics();
    }

    public float force = 5f;
    private double timer;
    void UpdatePhysics()
    {
        if (Physics.autoSimulation) return;
        
        timer += Time.deltaTime;
        while (timer >= Time.fixedDeltaTime)
        {
            timer -= Time.fixedDeltaTime;

            foreach (var sph in instances)
            {
                if(!sph) continue;
                var pos = sph.transform.position;
                sph.rigid.AddForce((transform.position - pos).normalized * force);
                sph.rigid.AddForce((sph.targetPosition - pos).normalized * sph.force);
                sph.transform.localScale = Vector3.one * sph.targetScale;
            }
            
            Physics.Simulate(Time.fixedDeltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(1, 1, 1, 0.1f);
        foreach (var inst in instances)
        {
            if(!inst) continue;
            Gizmos.DrawWireSphere(inst.targetPosition, inst.targetScale);
            Gizmos.DrawLine(inst.targetPosition, inst.transform.localPosition);
        }
    }

    private void OnEnable()
    {
        Physics.autoSimulation = false;
    }

    private void OnDisable()
    {
        Physics.autoSimulation = true;
    }
    
    
    public sbyte needleInspector;
}
