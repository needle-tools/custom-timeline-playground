using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Needle.Timeline.SongTest01
{
	[ExecuteAlways]
	public class BouncingBalls : MonoBehaviour, IAnimated, IAnimatedEvents, IOnionSkin
	{
		public PlayableDirector Director;
		public GameObject Prefab;

		private void OnEnable()
		{
			Physics.autoSimulation = false;
		}

		private void OnDisable()
		{
			Physics.autoSimulation = true;
			
			foreach (var t in GetComponentsInChildren<Transform>())
			{
				if (t == this.transform) continue;
				if (Application.isPlaying) Destroy(t.gameObject);
				else DestroyImmediate(t.gameObject);
			}
			foreach(var b in balls) b.Kill();
		}

		private void Update()
		{
			OnEvaluated(FrameInfo.Now());
			UpdatePhysics();
		}

		private double timer;

		private void UpdatePhysics()
		{
			if (Physics.autoSimulation) return;

			timer += Time.deltaTime;
			while (timer >= Time.fixedDeltaTime)
			{
				timer -= Time.fixedDeltaTime;
				Physics.Simulate(Time.fixedDeltaTime);
			}
		}
		

		public void OnReset()
		{
			
		}

		public void OnEvaluated(FrameInfo frame)
		{
			CreateAndUpdateBalls(Prefab);
		}


		
		private struct SourceData : IOnionSkin
		{
			public Vector3 Position;
			[Range(1,5)]
			public float MaxAge;

			public float MaxSize;
			
			public void RenderOnionSkin(IOnionData data)
			{
				data.SetColor(Color.Lerp(Color.green, Color.red, MaxAge / 5));
				Gizmos.DrawSphere(Position, .1f);
			}
		}

		[Animate]
		private List<SourceData> source0;

		private readonly List<BallBehaviour> balls = new List<BallBehaviour>();

		private int globalBallsId;

		private void CreateAndUpdateBalls(GameObject prefab)
		{
			if (!prefab) return;
			if (source0 == null) return;
			if(2*source0.Count > balls.Count && source0.Count > 0)
			{
				// if (source0.Count <= 0) break;
				var i = Instantiate(prefab, this.transform, false);
				i.hideFlags = HideFlags.DontSaveInEditor;// | HideFlags.HideInHierarchy;
				if(!i.activeSelf)
					i.SetActive(true);
				var b = new BallBehaviour(i, source0[globalBallsId++ % source0.Count]);
				balls.Add(b);
			}
			for (var index = 0; index < balls.Count; index++)
			{
				var b = balls[index];

				if (b.IsDead)
				{
					balls.RemoveAt(index);
					index -= 1;
					continue;
				}
				
				b.Update(this, FrameInfo.Now());
			}
		}

		public void RenderOnionSkin(IOnionData data)
		{
			data.TryRender(source0);
		}

		private class BallBehaviour
		{
			private readonly GameObject instance;
			[CanBeNull] private Rigidbody rb;
			private float age;
			private SourceData data;
			
			public BallBehaviour(GameObject instance, SourceData data)
			{
				this.instance = instance;
				this.data = data;
				instance.transform.position = data.Position;
				if (instance.TryGetComponent(out rb))
				{
					rb.velocity = Vector3.up;
				}
			}
			
			public void Update(BouncingBalls caller, FrameInfo fi)
			{
				age += fi.DeltaTime;
				if (age > data.MaxAge)
				{
					Kill();
					return;
				}
				
				var t01 = age.Remap(0, data.MaxAge, 0, 1);
				var scale = 1 - (Mathf.Sin(t01 * Mathf.PI * 2 + Mathf.PI * .5f) * .5f + .5f);
				instance.transform.localScale = Vector3.one * scale * t01 * data.MaxSize;
				
			}

			public void Kill()
			{
				if (IsDead) return; 
				if (Application.isPlaying) Destroy(instance);
				else DestroyImmediate(instance);
			}

			public bool IsDead => !instance;
		}
	}
}