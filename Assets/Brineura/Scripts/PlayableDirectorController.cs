using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.SceneManagement;

public class PlayableDirectorController : MonoBehaviour
{
    private PlayableDirector playableDirector;
    private TimelineVideoController videoController;

    public InputActionProperty fastForward;
    public InputActionProperty rewindAction;
    public InputActionProperty playAction;
    public InputActionProperty resetAction;
    public InputActionProperty returnToMenuAction;

    [Header("Playback Settings")]
    public double rewindSpeed = 2.0;
    public double fastForwardSpeed = 2.0; 

    private void OnEnable()
    {
        fastForward.action.Enable();
        playAction.action.Enable();
        rewindAction.action.Enable();
        resetAction.action.Enable();
        returnToMenuAction.action.Enable();
    }

    private void OnDisable()
    {
        fastForward.action.Disable();
        playAction.action.Disable();
        rewindAction.action.Disable();
        resetAction.action.Disable();
        returnToMenuAction.action.Disable();
    }

    private void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        videoController = GetComponentInChildren<TimelineVideoController>();
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
            playableDirector.Evaluate();
            if (videoController != null)
            {
                videoController.ChangeTime(playableDirector.time);
            }
            if (playableDirector.time > playableDirector.duration)
                playableDirector.time = playableDirector.duration;
            
        }
        if (rewindAction.action.IsPressed())
        {
            playableDirector.time -= rewindSpeed * Time.deltaTime;
            playableDirector.Evaluate();
            if (videoController != null)
            {
                videoController.ChangeTime(playableDirector.time);
            }
            if (playableDirector.time < 0)
                playableDirector.time = 0;
           
        }

        if (resetAction.action.triggered)
        {
            Reset();
        }

        if (returnToMenuAction.action.triggered)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void Play()
    {
        if (playableDirector == null) return;
        playableDirector.Play();
        if (videoController == null) return;
        videoController.PlayVideo();
    }

    public void Pause()
    {
        if (playableDirector == null) return;
        playableDirector.Pause();
        if(videoController == null) return; 
        videoController.PauseVideo();
    }

    public void Reset()
    {
        if (playableDirector == null) return;

        playableDirector.time = 0;
        playableDirector.Evaluate();
    }
}