using System;
using System.Collections.Generic;
using System.Linq;
using GLTF.Schema;
using UnityEngine;
using UnityGLTF;

[ExecuteAlways]
public class GltfRecording : MonoBehaviour
{
	private void OnEnable()
	{
		if(Application.isPlaying)
		{
			Time.captureFramerate = 60;
			StartNow();
		}
	}

	private void OnDisable()
	{
		if (Application.isPlaying)
		{
			Time.captureFramerate = 0;
			StopNow();
		}
	}

	[ContextMenu("Start Recording")]
	void StartNow()
	{
		StartRecording(Time.realtimeSinceStartupAsDouble);
		recording = true;
	}

	[ContextMenu("Stop Recording")]
	void StopNow()
	{
		recording = false;
		EndRecording();
	}

	private bool recording = false;

	private void LateUpdate()
	{
		if (!recording) return;

		UpdateRecording(Time.realtimeSinceStartupAsDouble);
		Size = data.Sum(x => x.Value.Size + 4 + 4);
	}

	public int Size;

	public Transform root;

	private Dictionary<Transform, AnimationData> data = new Dictionary<Transform, AnimationData>();

	internal class AnimationData
	{
		public int Size => 4 + 4 + keys.Count * (4 + 4 + 40);
		private Transform tr;
		public TransformData lastData;
		public Dictionary<double, TransformData> keys = new Dictionary<double, TransformData>();

		public AnimationData(Transform tr, double time)
		{
			this.tr = tr;
			keys.Add(time, new TransformData(tr));
		}

		public void Update(double time)
		{
			var newTr = new TransformData(tr);
			if(!newTr.Equals(lastData))
				keys.Add(time, new TransformData(tr));
		}
	}

	private double startTime;
	private double lastRecordedTime;
	public void StartRecording(double time)
	{
		startTime = time;
		lastRecordedTime = time;
		var trs = root.GetComponentsInChildren<Transform>();
		data.Clear();
		foreach (var tr in trs)
		{
			data.Add(tr, new AnimationData(tr, 0));
		}
	}

	public void UpdateRecording(double time)
	{
		if (time < lastRecordedTime)
		{
			Debug.LogWarning("Can't record backwards in time, please avoid this.");
			return;
		}

		var currentTime = time - startTime;
		var trs = root.GetComponentsInChildren<Transform>();
		foreach (var tr in trs)
		{
			if (!data.ContainsKey(tr))
				data.Add(tr, new AnimationData(tr, currentTime));
			else
				data[tr].Update(currentTime);
		}
	}

	public void EndRecording()
	{
		// log
		Debug.Log("Tracks: " + data.Count + ", Keys: " + data.First().Value.keys.Count);
		Debug.Log("Total Keys: " + data.Sum(x => x.Value.keys.Count));

		GLTFSceneExporter.ExportDisabledGameObjects = true;

		var exporter = new GLTFSceneExporter(new Transform[] { root }, new ExportOptions()
		{
			ExportInactivePrimitives = true,
			AfterSceneExport = PostExport,
		});
		exporter.SaveGLB("Assets", "TestExport.glb");
	}

	private void PostExport(GLTFSceneExporter exporter, GLTFRoot gltfRoot)
	{
		exporter.ExportAnimationFromNode(ref root);

		GLTFAnimation anim = new GLTFAnimation();
		anim.Name = name;

		CollectAnimation(exporter, ref root, ref anim, 1f);

		if (anim.Channels.Count > 0 && anim.Samplers.Count > 0)
			gltfRoot.Animations.Add(anim);
	}

	void CollectAnimation(GLTFSceneExporter gltfSceneExporter, ref Transform root, ref GLTFAnimation anim, float speed)
	{
		foreach (var kvp in data)
		{
			if(kvp.Value.keys.Count < 1) continue;

			var times = kvp.Value.keys.Keys.Select(x => (float) x).ToArray();
			var values = kvp.Value.keys.Values.ToArray();
			var positions = values.Select(x => x.position).ToArray();
			var rotations = values.Select(x => x.rotation).Select(x => new Vector4(x.x, x.y, x.z, x.w)).ToArray();
			var scales = values.Select(x => x.scale).ToArray();
			float[] weights = null;
			int weightCount = 0;
			gltfSceneExporter.RemoveUnneededKeyframes(ref times, ref positions, ref rotations, ref scales, ref weights, ref weightCount);
			gltfSceneExporter.AddAnimationData(kvp.Key, ref anim, times, positions, rotations, scales);
		}
	}

	internal readonly struct TransformData
	{
		public readonly Vector3 position;
		public readonly Quaternion rotation;
		public readonly Vector3 scale;

		public TransformData(Transform tr)
		{
			position = tr.localPosition;
			rotation = tr.localRotation;
			scale = tr.localScale;
		}

		public bool Equals(TransformData other)
		{
			return position.Equals(other.position) && rotation.Equals(other.rotation) && scale.Equals(other.scale);
		}

		public override bool Equals(object obj)
		{
			return obj is TransformData other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = position.GetHashCode();
				hashCode = (hashCode * 397) ^ rotation.GetHashCode();
				hashCode = (hashCode * 397) ^ scale.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(TransformData left, TransformData right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(TransformData left, TransformData right)
		{
			return !left.Equals(right);
		}
	}
}
