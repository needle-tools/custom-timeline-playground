using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Needle.Timeline
{
	public class ModulesTest : MonoBehaviour, IAnimated
	{
		[Animate, SerializeField] private List<MyType> MyTypeList;
		[Animate] private List<Vector3> Points;
		[Animate, FormerlySerializedAs("Directions1")] private List<Direction> Directions;

		[System.Serializable]
		private struct MyType 
		{ 
			public MyEnum Options;

			public enum MyEnum
			{
				Sphere,
				Cube
			}

			public Vector3 Position;
			public float Weight;
			public Color Color;
			public Color Color2;
		}

		private void OnDrawGizmos()
		{
#if UNITY_EDITOR
			if (MyTypeList != null)
			{
				var style = new GUIStyle(GUI.skin.label);
				style.alignment = TextAnchor.MiddleLeft;
				style.fontSize = 9;
				var ct = Camera.current.transform;
				var offset = ct.right * .2f + ct.up * .11f;
				for (var index = 0; index < MyTypeList.Count; index++)
				{
					var t = MyTypeList[index];
					if (t.Color.a <= 0) t.Color.a = 1;
					Handles.color = Gizmos.color = t.Color;
					if(t.Options == MyType.MyEnum.Sphere)
						Gizmos.DrawSphere(t.Position, .05f + t.Weight);
					else Gizmos.DrawCube(t.Position, Vector3.one * (.05f + t.Weight));
					style.normal.textColor = t.Color;
					// Handles.Label(t.Position + offset, t.Options.ToString(), style);
					MyTypeList[index] = t;
				}
			}
#endif

			if (Points != null)
			{
				Gizmos.color = Color.gray;
				foreach (var pt in Points)
				{
					Gizmos.DrawSphere(pt, .1f);
				}
			}

			if (Directions != null)
			{
				foreach(var dir in Directions) dir.RenderOnionSkin(OnionData.Default);
			}
		}
	}
}