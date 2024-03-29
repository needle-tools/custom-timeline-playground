﻿using Needle.Timeline;
using Needle.Timeline.CustomClipTools.ToolModule;
using Needle.Timeline.CustomClipTools.ToolModule.Implementations;
using UnityEditor;
using UnityEngine;


public struct Direction : ICustomControls, IToolEvents, IOnionSkin, IHasDirection
{
	public Vector3 Start;
	public Vector3 End;

	// the following is not an option because types should easily be set to shaders
	// [NonSerialized, JsonIgnore]
	// private Vector3 _deltaSum;

	public void OnToolEvent(ToolStage stage, IToolData data)
	{
		if (data == null) return;
		if (stage == ToolStage.BasicValuesSet)
		{
			// Debug.Log(data.DeltaWorld.GetValueOrDefault());
			End = Start + Vector3.ClampMagnitude(data.DeltaWorld.GetValueOrDefault()*5,1);
		}
		else if (stage == ToolStage.InputUpdated)
		{
			// if (stage == CreationStage.BasicValuesSet) _deltaSum = data.DeltaWorld.GetValueOrDefault().normalized;
			End += data.DeltaWorld.GetValueOrDefault() * 0.005f;
		}
	}

	public bool OnCustomControls(IToolData data, IToolModule module)
	{
#if UNITY_EDITOR
		if (data.WorldPosition != null)
		{
			var sp = data.ToScreenPoint(data.WorldPosition.Value);
			var dist = 50;
			if (Vector2.Distance(sp, data.ToScreenPoint(Start)) > dist
			    && Vector2.Distance(sp, data.ToScreenPoint(End)) > dist)
				return false;
		}
		var start = Handles.PositionHandle(Start, Quaternion.identity);
		var end = Handles.PositionHandle(End, Quaternion.identity);
		var changed = start != Start || end != End;
		Start = start;
		End = end;
		return changed;
#else
		return false;
#endif
	}

	public void RenderOnionSkin(IOnionData data)
	{
		Gizmos.color = Color.Lerp(Color.gray, data.ColorOnion, data.WeightOnion);
		Gizmos.DrawLine(Start, End);
		var dir = End - Start;
		var ort = Vector3.Cross(dir * .1f, Vector3.forward);
		Gizmos.DrawLine(End, Vector3.Lerp(Start, End + ort, .9f));
		ort *= -1;
		Gizmos.DrawLine(End, Vector3.Lerp(Start, End + ort, .9f));
	}

	Vector3 IHasDirection.Start { get => Start; set => Start = value; }
	Vector3 IHasDirection.End { get => End; set => End = value; }
}