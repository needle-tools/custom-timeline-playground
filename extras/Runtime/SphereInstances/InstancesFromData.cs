using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Needle.Timeline
{
    [ExecuteAlways]
    public abstract class InstancesFromData<TBehaviour, TData> : MonoBehaviour, IAnimated where TBehaviour : Component
    {
        internal void ResetData()
        {
            foreach (var inst in instances)
            {
                if (!inst) continue;
                if (inst.gameObject == this.gameObject) continue;
                if (Application.isPlaying) Destroy(inst.gameObject);
                else DestroyImmediate(inst.gameObject);
            }

            foreach (var inst in buffer)
            {
                if (!inst) continue;
                if (inst.gameObject == this.gameObject) continue;
                if (Application.isPlaying) Destroy(inst.gameObject);
                else DestroyImmediate(inst.gameObject);
            }

            instances.Clear();
            buffer.Clear();
        }

        public TBehaviour template;
        public HideFlags instanceFlags = HideFlags.DontSave;

        [Animate]
        public List<TData> data = new List<TData>();

        internal List<TBehaviour> instances = new List<TBehaviour>();
        internal List<TBehaviour> buffer = new List<TBehaviour>();

        private void Update()
        {
            if (!template) return;
            if (instances == null) instances = new List<TBehaviour>();
            if (data == null) data = new List<TData>();
            if (buffer == null) buffer = new List<TBehaviour>();

            // if (instances.Count != data.Count)
            {
                while (instances.Count > data.Count)
                {
                    var last = instances.Last();
                    instances.Remove(last);
                    buffer.Add(last);
                    if (last)
                    {
                        if (last.gameObject == this.gameObject) continue;
                        last.gameObject.SetActive(false);
                        last.gameObject.transform.localScale = Vector3.zero;
                    }
                }

                while (instances.Count < data.Count)
                {
                    if (buffer.Count > 0)
                    {
                        var last = buffer.Last();
                        instances.Add(last);
                        buffer.Remove(last);
                        if (last)
                        {
                            last.gameObject.transform.localScale = Vector3.zero;
                            last.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        var newInstance = Instantiate(template, transform);
                        newInstance.name = template.name + "-" + newInstance.transform.GetSiblingIndex();
                        newInstance.gameObject.hideFlags = instanceFlags;
                        ApplyDataToBehaviour(data[instances.Count], newInstance);
                        instances.Add(newInstance);
                        newInstance.gameObject.SetActive(true);
                    }
                }
            }

            for (int i = 0; i < data.Count; i++)
            {
                var dat = data[i];
                var inst = instances[i];
                if (!inst)
                {
                    instances.RemoveAt(i);
                    data.RemoveAt(i);
                    i--;
                    continue;
                }
                inst.gameObject.SetActive(true); // TODO figure out why the buffer logic doesn't do this

                ApplyDataToBehaviour(dat, inst);
            }

            UpdatePhysics();
        }

        public abstract void ApplyDataToBehaviour(TData data, TBehaviour behaviour);

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
                    if (!sph) continue;

                    ApplyPhysics(sph);
                }

                Physics.Simulate(Time.fixedDeltaTime);
            }
        }

        public virtual void ApplyPhysics(TBehaviour behaviour) {}

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(1, 1, 1, 0.1f);
            foreach (var inst in instances)
            {
                if (!inst) continue;

                DrawGizmo(inst);
            }
        }

        public virtual void DrawGizmo(TBehaviour behaviour) {}

        private void OnEnable()
        {
            Physics.autoSimulation = false;
            buffer = GetComponentsInChildren<TBehaviour>(true).ToList();
        }

        private void OnDisable()
        {
            Physics.autoSimulation = true;
        }
    }
}
