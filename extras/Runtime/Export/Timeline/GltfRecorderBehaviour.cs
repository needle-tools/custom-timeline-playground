using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GltfRecorderBehaviour : PlayableBehaviour
{
    private GltfRecorder recorder = null;
    private void BeginRecording(double getTime, Transform getExportRoot)
    {
        if (!getExportRoot)
        {
            Debug.LogError("Can't record: export root is null");
            recorder = null;
            return;
        }

        Time.captureFramerate = Clip.m_CaptureFrameRate;
        
        recorder = new GltfRecorder(getExportRoot);
        recorder.StartRecording(getTime);
    }

    private void StopRecording(double getTime)
    {
        recorder?.EndRecording(Clip.m_File);
    }
    
    private void ProcessRecording(double getTime, Transform getExportRoot)
    {
        recorder?.UpdateRecording(getTime);
    }
    
    public GltfRecorderClip Clip;
    private bool m_isPaused = false;

    private static bool IsPlaying()
    {
#if UNITY_EDITOR
        return EditorApplication.isPlaying;
#else
            return true;
#endif
    }
    
    public override void OnPlayableDestroy(Playable playable)
    {
        if (!IsPlaying())
        {
            return;
        }

        StopRecording(playable.GetTime());
    }
    
    public override void OnGraphStart(Playable playable)
    {
        if (!IsPlaying())
        {
            return;
        }

        BeginRecording(playable.GetTime(), Clip.GetExportRoot(playable.GetGraph()));
    }

    public override void OnGraphStop(Playable playable)
    {
        if (!IsPlaying())
        {
            return;
        }

        StopRecording(playable.GetTime());
    }
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!IsPlaying())
        {
            return;
        }

        var frameRate = Time.captureFramerate;
        if (frameRate < 1)
        {
            frameRate = Application.targetFrameRate;
        }

        // TODO move this to end of frame
        // UsdWaitForEndOfFrame.Add(() => OnFrameEnd(playable, info, playerData));
        OnFrameEnd(playable, info, playerData);
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        m_isPaused = false;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        m_isPaused = true;
    }

    public void OnFrameEnd(Playable playable, FrameData info, object playerData)
    {
        if (!playable.IsValid())
        {
            return;
        }

        var root = Clip.GetExportRoot(playable.GetGraph());
        if (!root || m_isPaused)
        {
            return;
        }
        
        ProcessRecording(playable.GetTime(), root);
    }
}
