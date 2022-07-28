using System.Collections.Generic;
using JetBrains.Annotations;
using Needle.Timeline;
using Needle.Timeline.ResourceProviders;
using UnityEngine;
using Random = UnityEngine.Random;

// this script automatically binds c# fields to compute shader
// Note that many of the fields in this c# script match names in the compute shader that is assigned from Unity
// the shader and the c# script are both automatically parsed and matching types will be automatically assigned
// the logic for this is handled in the Animated base class at the moment
// in the future I want to make this simpler but also a bit more explicit since it is VERY magic so a little bit confusing too

public class DrawLine : Animated
{ 
	// compute shaders will be found and parsed by the Animated class automatically
	[UsedImplicitly] 
	public ComputeShader Shader;
	
	// this will be animated from the timeline:
	[Animate] 
	public List<Direction> Directions; 
	
	[TextureInfo]//(256, 256, FilterMode = FilterMode.Point)]
	public RenderTexture Output; 
	public Renderer Rend;

	public Vector2Int OutputSize = new Vector2Int(512, 512);

	public bool RandomColor = true;
	public Color Color;

	[TransformInfo]
	public List<Transform> TransformList = new List<Transform>();
	public Transform[] TransformArray;

	public struct Point 
	{
		public Vector2 Pos;
		public float Size;
		public Color Color;
	}
	
	private List<Point> Points;
	
	[Animate(AllowInterpolation = true)]
	public List<Point> AnimatedPoints;

	public bool ShowPoints = true;
	public bool ShowSimulation = true;

	public int Points_Count = 100;
	public float PointSpacing = .2f;

	[Header("Gizmos")]
	public bool RenderDirectionGizmos = true;

	private void OnValidate()
	{
		foreach (var t in TransformArray)
		{
			if(t.hasChanged) OnRequestEvaluation();
			t.hasChanged = false;
		}
	}

	[ContextMenu("Reset now")]
	public override void OnReset()
	{
		base.OnReset();
		Points?.Clear(); 
	}

	protected override void OnBeforeDispatching()
	{
		// Graphics.Blit(Texture2D.blackTexture, Output); 
	}

	protected override IEnumerable<DispatchInfo> OnDispatch()
	{
		Output.SafeCreate(ref Output,
			new RenderTextureDescription() { Width = OutputSize.x, Height = OutputSize.y, FilterMode = FilterMode.Point, RandomAccess = true });
		
		if (transform.childCount != TransformArray?.Length)
		{
			// TODO: use PlayerLoopHelper to create transform changed watcher that resets changed bool at very end of every frame
			
			TransformArray = new Transform[transform.childCount];
			for(var i = 0; i < transform.childCount; i++)
			{
				var t = transform.GetChild(i);
				TransformArray[i] = t;
				if(t.hasChanged) OnRequestEvaluation();
				t.hasChanged = false;
				// t.OnHasChanged(OnRequestEvaluation);
			}
		}
		
		// yield return new DispatchInfo { KernelIndex = 1, GroupsX = 32, GroupsY = 32};
		// yield return new DispatchInfo { KernelIndex = 1, GroupsX = Directions?.Count };

		if (Points == null || Points.Count <= 0 || Points.Count != Points_Count)
		{
			Points ??= new List<Point>();
			Points.Clear();
			for (var i = 0; i < Points_Count; i++) 
			{
				Points.Add(new Point(){Pos = Random.insideUnitCircle*.05f});
			}
			// Debug.Log("Points: " + Points.Count());
			SetDirty(nameof(Points));
		}
		if (PointSpacing < .00001f)
		{
			Points_Count = (int)Random.Range(10, 1000);
		}
		if(RandomColor && Time.frameCount % 90 == 0)
			Color = Random.ColorHSV(0,1,.3f,1,.5f,1);
		
		
		yield return new DispatchInfo { KernelIndex = 0, GroupsX = 1 };
		yield return new DispatchInfo { KernelIndex = 2, GroupsX = Points?.Count }; 
		yield return new DispatchInfo { KernelName = "CSBlend"};
	}

	protected override void OnAfterEvaluation()
	{
		base.OnAfterEvaluation(); 
		Rend.SetTexture(Output);
	}

	private void OnDrawGizmos()
	{
		if(Directions == null) return;
		if (!RenderDirectionGizmos) return;
		foreach(var dir in Directions) dir.RenderOnionSkin(OnionData.Default);
	}
}