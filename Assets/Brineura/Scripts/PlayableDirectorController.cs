using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine;

public class PlayableDirectorController : MonoBehaviour
{
    public PlayableDirector playableDirector;

    public TimelineVideoController videoController;

    public InputActionProperty fastForward;
    public InputActionProperty rewindAction;
    public InputActionProperty playAction;
    public InputActionProperty resetAction;

    [Header("Playback Settings")]
    public double rewindSpeed = 2.0;
    public double fastForwardSpeed = 2.0; 

    private void OnEnable()
    {
        fastForward.action.Enable();
        playAction.action.Enable();
        rewindAction.action.Enable();
        resetAction.action.Enable();
    }

    private void OnDisable()
    {
        fastForward.action.Disable();
        playAction.action.Disable();
        rewindAction.action.Disable();
        resetAction.action.Disable();
    }

    private void Update()
    {
        if (playableDirector == null) return;

        if (playAction.action.triggered)
        {
            if (playableDirector.state == PlayState.Playing)
            {
                Pause();
                return;
            }
            Play();
        }

        if (fastForward.action.IsPressed())
        {
            playableDirector.time += fastForwardSpeed * Time.deltaTime;
            videoController.ChangeTime(playableDirector.time);
            if (playableDirector.time > playableDirector.duration)
                playableDirector.time = playableDirector.duration;
            playableDirector.Evaluate();
        }
        if (rewindAction.action.IsPressed())
        {
            playableDirector.time -= rewindSpeed * Time.deltaTime;
            videoController.ChangeTime(playableDirector.time);
            if (playableDirector.time < 0)
                playableDirector.time = 0;
            playableDirector.Evaluate();
        }

        if (resetAction.action.triggered)
        {
            Reset();
        }
    }

    public void Play()
    {
        if (playableDirector == null) return;
        playableDirector.Play();
    }

    public void Pause()
    {
        if (playableDirector == null) return;
        playableDirector.Pause();
    }

    public void Reset()
    {
        if (playableDirector == null) return;

        playableDirector.time = 0;
        playableDirector.Evaluate();
    }
}