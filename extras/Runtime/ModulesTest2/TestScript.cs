using System;
using System.Collections.Generic;
using Needle.Timeline;
using UnityEngine;

public class TestScript : MonoBehaviour, IAnimated, IOnionSkin
{
	[Animate, NonSerialized] public List<Vector3> Vecs;

	public bool DrawGizmos = true;

	private struct ColorPoint
	{
		public Vector3 Point;
		public Color Color;
		[Range(0.01f, .5f)] public float Size;
	}

	[Animate] private List<ColorPoint> colorPoints;
	[Animate] private List<ColorPoint> colorPoints2;
	[Animate] private List<Direction> directions;

	private void OnDrawGizmos()
	{
		if (!DrawGizmos) return;
		RenderOnionSkin(OnionData.Default);
	}

	public void RenderOnionSkin(IOnionData data)
	{
		Gizmos.color = data.GetColor(Color.white);
		if (Vecs != null)
		{
			foreach (var vec in Vecs)
				Gizmos.DrawSphere(vec, .05f);
		}

		if (colorPoints != null)
		{
			foreach (var cp in colorPoints)
			{
				Gizmos.color = data.GetColor(cp.Color);
				Gizmos.DrawSphere(cp.Point, cp.Size + .01f);
			}
		}

		if (colorPoints2 != null)
		{
			foreach (var cp in colorPoints2)
			{
				Gizmos.color = data.GetColor(cp.Color);
				Gizmos.DrawSphere(cp.Point, cp.Size + .01f);
			}
		}
		
		if (directions != null)
			foreach (var dir in directions)
				dir.RenderOnionSkin(data);
	}
}