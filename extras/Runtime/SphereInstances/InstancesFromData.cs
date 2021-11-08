using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Needle.Timeline
{
    public abstract class InstancesFromData<TBehaviour, TData> : MonoBehaviour, IAnimated where TBehaviour : Component
    {
        internal void ResetData()
        {
            foreach (var inst in instances)
            {
                if (!inst) continue;
                if (Application.isPlaying) Destroy(inst.gameObject);
                else DestroyImmediate(inst.gameObject);
            }

            foreach (var inst in buffer)
            {
                if (!inst) continue;
                if (Application.isPlaying) Destroy(inst.gameObject);
                else DestroyImmediate(inst.gameObject);
            }

            instances.Clear();
            buffer.Clear();
        }

        public TBehaviour template;

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

            if (instances.Count != data.Count)
            {
                while (instances.Count > data.Count)
                {
                    var last = instances.Last();
                    instances.Remove(last);
                    buffer.Add(last);
                    if (last) last.gameObject.SetActive(false);
                }

                while (instances.Count < data.Count)
                {
                    if (buffer.Count > 0)
                    {
                        var last = buffer.Last();
                        instances.Add(last);
                        buffer.Remove(last);
                        if (last) last.gameObject.SetActive(true);
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

        public abstract void ApplyPhysics(TBehaviour behaviour);

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

        public abstract void DrawGizmo(TBehaviour behaviour);

        private void OnEnable()
        {
            Physics.autoSimulation = false;
        }

        private void OnDisable()
        {
            Physics.autoSimulation = true;
        }
    }
}
