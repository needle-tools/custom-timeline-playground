using System;
using System.Collections.Generic;
using Needle.Timeline;
using UnityEngine;

public class TestScript : MonoBehaviour, IAnimated
{
	[Animate, NonSerialized] public List<Vector3> Vecs;

	private struct ColorPoint
	{
		public Vector3 Point;
		public Color Color;
		[Range(0.01f, .5f)]
		public float Size; 
	}

	[Animate] private List<ColorPoint> colorPoints;
	[Animate] private List<ColorPoint> colorPoints2;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		if (Vecs != null)
		{
			foreach (var vec in Vecs)
				Gizmos.DrawSphere(vec, .05f);
		}

		if (colorPoints != null)
		{
			foreach (var cp in colorPoints)
			{
				Gizmos.color = cp.Color;
				Gizmos.DrawSphere(cp.Point, cp.Size + .01f);
			}
		}

		if (colorPoints2 != null)
		{
			foreach (var cp in colorPoints2)
			{
				Gizmos.color = cp.Color;
				Gizmos.DrawSphere(cp.Point, cp.Size + .01f);
			}
		}
	}
}