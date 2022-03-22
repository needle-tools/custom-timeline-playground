using UnityEngine;

namespace UnityGLTF.Timeline
{
	[ExecuteAlways]
	public class GltfRecording : MonoBehaviour
	{
		public Transform root;

		private void OnEnable()
		{
			if (Application.isPlaying)
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

		private GltfRecorder recorder = null;

		[ContextMenu("Start Recording")]
		void StartNow()
		{
			recorder = new GltfRecorder(root);
			recorder.StartRecording(Time.realtimeSinceStartupAsDouble);
			recording = true;
		}

		[ContextMenu("Stop Recording")]
		void StopNow()
		{
			recording = false;
			recorder.EndRecording("Assets/Recording.glb");
		}

		private bool recording = false;

		private void LateUpdate()
		{
			if (!recording) return;

			recorder.UpdateRecording(Time.realtimeSinceStartupAsDouble);
		}
	}
}